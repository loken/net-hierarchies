namespace Loken.Hierarchies;

public class NodeAssemblyTests
{
	private const string Input = """
		A:A1,A2
		B:B1
		C
		A1:A11,A12
		B1:B12
		""";

	private static readonly string[] InputRoots = ["A", "B", "C"];
	private static readonly MultiMap<string> InputMap = MultiMap.Parse<string>(Input);

	[Fact]
	public void Assemble_Ids()
	{
		var roots = Nodes.AssembleIds(InputMap);

		Assert.Equal(InputRoots, roots.ToItems());

		var output = roots.ToChildMap(id => id).Render();

		Assert.Equal(Input, output);
	}

	[Fact]
	public void Assemble_Items()
	{
		var items = "A,B,C,A1,A2,B1,A11,A12,B12".Split(',').Select(s => new Item<string>(s)).ToArray();

		var roots = Nodes.AssembleItems(item => item.Id, items, InputMap);

		Assert.Equal(InputRoots, roots.ToIds(item => item.Id));

		var output = roots.ToChildMap(item => item.Id).Render();

		Assert.Equal(Input, output);
	}

	[Fact]
	public void Assemble_Items_ShouldIgnoreItemsNotInRelationshipSpec()
	{
		var items = "A,B,C,A1,A2,B1,A11,A12,B12,IGNORED".Split(',').Select(s => new Item<string>(s)).ToArray();

		var roots = Nodes.AssembleItems(item => item.Id, items, InputMap);
;
		Assert.Equal(InputRoots, roots.ToIds(item => item.Id));

		var output = roots.ToChildMap(item => item.Id).Render();

		Assert.Equal(Input, output);
	}

	private class ItemWithChildren
	{
		public required string Id { get; set; }
		public ItemWithChildren[]? Children { get; set; }
	}

	private class ItemWithParent
	{
		public required string Id { get; set; }
		public ItemWithParent? Parent { get; set; }
	}

	[Fact]
	public void AssembleItemsWithChildren()
	{
		// Represents the same structure as the relations
		var itemsWithChildren = new[]
		{
			new ItemWithChildren
			{
				Id = "A",
				Children =
				[
					new ItemWithChildren
					{
						Id = "A1",
						Children =
						[
							new ItemWithChildren { Id = "A11" },
							new ItemWithChildren { Id = "A12" }
						]
					},
					new ItemWithChildren { Id = "A2" }
				]
			},
			new ItemWithChildren
			{
				Id = "B",
				Children =
				[
					new ItemWithChildren
					{
						Id = "B1",
						Children =
						[
							new ItemWithChildren { Id = "B12" }
						]
					}
				]
			},
			new ItemWithChildren { Id = "C" }
		};

		var roots = Nodes.AssembleItemsWithChildren(itemsWithChildren, item => item.Children);

		Assert.Equal(InputRoots, roots.ToIds(item => item.Id));

		var actual = roots.ToChildMap(item => item.Id).Render();

		Assert.Equal(Input, actual);
	}

	[Fact]
	public void AssembleItemsWithParents()
	{
		// Create a map of items.
		var itemsWithParents = new Dictionary<string, ItemWithParent>();
		foreach (var id in InputMap.GetAll())
			itemsWithParents[id] = new ItemWithParent { Id = id };

		// Set the parent references.
		foreach (var (parentId, childIds) in InputMap)
		{
			var parent = itemsWithParents[parentId];
			foreach (var childId in childIds)
			{
				var child = itemsWithParents[childId];
				child.Parent ??= parent;
			}
		}

		// Ensure we can pass it all of the items.
		var allItems = itemsWithParents.Values.ToArray();
		var rootsFromAll = Nodes.AssembleItemsWithParents(allItems, item => item.Parent);

		Assert.Equal(InputRoots, rootsFromAll.Select(r => r.Item.Id));

		var actualFromAll = rootsFromAll.ToChildMap(item => item.Id);
		Assert.Equal(InputMap, actualFromAll);

		// Ensure we can pass it the leaf items only.
		var leafItems = itemsWithParents.Values.Where(item => (InputMap.Get(item.Id)?.Count ?? 0) == 0).ToArray();
		var rootsFromLeaves = Nodes.AssembleItemsWithParents(leafItems, item => item.Parent);

		var actualFromLeaves = rootsFromLeaves.ToChildMap(item => item.Id);
		Assert.Equal(InputMap, actualFromLeaves);
	}
}
