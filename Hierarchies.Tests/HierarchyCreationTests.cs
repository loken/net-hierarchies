namespace Loken.Hierarchies;

public class HierarchyCreationTests
{
	private static readonly MultiMap<string> ChildMap = MultiMap.Parse<string>(
		"""
		A:A1,A2
		A2:A21
		B:B1
		B1:B11,B12,B13
		"""
	);

	private static readonly Relation<string>[] Relations =
	[
		new("A", "A1"),
		new("A", "A2"),
		new("A2", "A21"),
		new("B", "B1"),
		new("B1", "B11"),
		new("B1", "B12"),
		new("B1", "B13")
	];

	private static readonly Item<string>[] Items =
	[
		new("A"), new("A1"), new("A2"), new("A21"),
		new("B"), new("B1"), new("B11"), new("B12"), new("B13")
	];

	private sealed class ItemWithChildren
	{
		public string Id { get; set; } = string.Empty;
		public List<ItemWithChildren>? Children { get; set; }
			= null;
	}

	private sealed class ItemWithParent
	{
		public string Id { get; set; } = string.Empty;
		public ItemWithParent? Parent { get; set; }
			= null;
	}

	[Fact]
	public void CreateFromRelations_WithIds_ReturnsCorrectRoots()
	{
		var hc = Hierarchies.CreateFromRelations([.. Relations]);

		Assert.Equal(2, hc.Roots.Count);
		Assert.Equivalent(new[] { "A", "B" }, hc.RootItems);
	}

	[Fact]
	public void CreateFromRelations_WithItems_ReturnsCorrectRoots()
	{
		var hc = Hierarchies.CreateFromRelations(Items, item => item.Id, [.. Relations]);

		Assert.Equal(2, hc.Roots.Count);
		Assert.Equivalent(new[] { "A", "B" }, hc.RootIds);
	}

	[Fact]
	public void CreateFromChildMap_ReturnsCorrectRoots()
	{
		var hc = Hierarchies.CreateFromChildMap(ChildMap);

		Assert.Equal(2, hc.Roots.Count);
		Assert.Equivalent(new[] { "A", "B" }, hc.RootItems);
	}

	[Fact]
	public void CreateFromChildMap_WithItems_RendersRelations()
	{
		var hc = Hierarchies.CreateFromChildMap(Items, item => item.Id, ChildMap);

		Assert.Equivalent(Relations, hc.ToRelations());
	}

	[Fact]
	public void CreateFromHierarchy_ReturnsTheSameStructure()
	{
		var idHierarchy = Hierarchies.CreateFromChildMap(ChildMap);
		var matchingHierarchy = Hierarchies.CreateFromHierarchy(Items, item => item.Id, idHierarchy);

		Assert.Equal(idHierarchy.ToChildMap().Render(), matchingHierarchy.ToChildMap().Render());
	}

	[Fact]
	public void CreateFromChildren_WithChildrenDelegate_RendersExpectedRelations()
	{
		var itemsWithChildren = new ItemWithChildren[]
		{
			new()
			{
				Id = "A",
				Children =
				[
					new() { Id = "A1" },
					new() { Id = "A2", Children = [ new() { Id = "A21" } ] },
				],
			},
			new()
			{
				Id = "B",
				Children =
				[
					new()
					{
						Id = "B1",
						Children = [ new() { Id = "B11" }, new() { Id = "B12" }, new() { Id = "B13" } ],
					},
				],
			},
		};

		var hc = Hierarchies.CreateFromChildren(itemsWithChildren, i => i.Id, i => i.Children);

		Assert.Equivalent(Relations, hc.ToRelations());
	}

	[Fact]
	public void CreateFromChildren_WithChildIdsDelegate_RendersExpectedRelations()
	{
		var allItems = Items;
		var parentToChildren = new Dictionary<string, IEnumerable<string>>();
		foreach (var kvp in ChildMap)
			parentToChildren[kvp.Key] = kvp.Value; // MultiMap values are enumerable

		var hc = Hierarchies.CreateFromChildren(allItems, i => i.Id, i => parentToChildren.TryGetValue(i.Id, out var ids) ? ids : null);

		Assert.Equivalent(Relations, hc.ToRelations());
	}

	[Fact]
	public void CreateFromParents_WithParentDelegate_AllItems_RendersExpectedRelations()
	{
		var itemsById = new Dictionary<string, ItemWithParent>();
		foreach (var i in Items)
			itemsById[i.Id] = new ItemWithParent { Id = i.Id };
		// Set parent pointers based on Relations
		foreach (var relation in Relations)
		{
			var parent = itemsById[relation.Parent];
			if (relation.Child is { } childId)
			{
				var child = itemsById[childId];
				child.Parent ??= parent;
			}
		}

		var all = itemsById.Values;
		var hc = Hierarchies.CreateFromParents(all, i => i.Id, i => i.Parent);

		Assert.Equivalent(Relations, hc.ToRelations());
	}

	[Fact]
	public void CreateFromParents_WithParentDelegate_LeafItems_RendersExpectedRelations()
	{
		var itemsById = new Dictionary<string, ItemWithParent>();
		foreach (var i in Items)
			itemsById[i.Id] = new ItemWithParent { Id = i.Id };
		foreach (var relation in Relations)
		{
			var parent = itemsById[relation.Parent];
			if (relation.Child is { } childId)
			{
				var child = itemsById[childId];
				child.Parent ??= parent;
			}
		}

		var leaves = new List<ItemWithParent>();
		foreach (var v in itemsById.Values)
			if (!ChildMap.TryGetValue(v.Id, out _))
				leaves.Add(v);
		var hc = Hierarchies.CreateFromParents(leaves, i => i.Id, i => i.Parent);

		Assert.Equivalent(Relations, hc.ToRelations());
	}

	[Fact]
	public void CreateFromParents_WithParentIdDelegate_RendersExpectedRelations()
	{
		var hc = Hierarchies.CreateFromParents(Items, i => i.Id, i =>
		{
			// Find parent id from ChildMap by searching where this item appears as a child
			foreach (var (parent, children) in ChildMap)
				if (children.Contains(i.Id))
					return parent;
			return null;
		});

		Assert.Equivalent(Relations, hc.ToRelations());
	}

	[Fact]
	public void Clone_WithItemHierarchy_PreservesItems()
	{
		var items = new[] { new Item<string>("A"), new Item<string>("A1"), new Item<string>("A2"), new Item<string>("B") };

		var childMap = new MultiMap<string> { { "A", ["A1", "A2"] } };
		var originalWithItems = Hierarchies.CreateFromChildMap(items, item => item.Id, childMap);

		var cloneWithItems = originalWithItems.Clone();

		Assert.Equivalent(originalWithItems.NodeItems, cloneWithItems.NodeItems);
		Assert.Same(originalWithItems.GetNode("A").Item, cloneWithItems.GetNode("A").Item);
	}

	[Fact]
	public void CloneIds_CreatesIdenticalHierarchyWithNewNodes()
	{
		var original = Hierarchies.CreateFromRelations<string>(new("A", "A1"), new("A", "A2"), new("A2", "A21"), new("B", "B1"));

		var clone = original.CloneIds();

		Assert.Equal(original.ToChildMap().Render(), clone.ToChildMap().Render());
		Assert.Equal(original.Roots.Count, clone.Roots.Count);
		Assert.Equivalent(original.RootItems, clone.RootItems);
		Assert.Equivalent(original.NodeItems, clone.NodeItems);

		var originalNodeA = original.GetNode("A");
		var cloneNodeA = clone.GetNode("A");
		Assert.NotSame(originalNodeA, cloneNodeA);
		Assert.Equal(originalNodeA.Item, cloneNodeA.Item);
		Assert.Equal(originalNodeA.Children.Count, cloneNodeA.Children.Count);
		Assert.Equivalent(originalNodeA.Children.ToItems(), cloneNodeA.Children.ToItems());
	}
}
