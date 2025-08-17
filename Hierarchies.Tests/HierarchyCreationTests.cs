namespace Loken.Hierarchies;

public class HierarchyCreationTests
{
	[Fact]
	public void CreateRelational_Ids_ReturnsCorrectRoots()
	{
		var hc = Hierarchy.CreateRelational<string>(
			new("A", "A1"),
			new("A", "A2"),
			new("A2", "A21"),
			new("B", "B1"),
			new("B1", "B11"),
			new("B1", "B12"),
			new("B1", "B13"));

		Assert.Equal(2, hc.Roots.Count);
		Assert.Equivalent(new[] { "A", "B" }, hc.RootItems);
	}

	[Fact]
	public void CreateRelational_Items_ReturnsCorrectRoots()
	{
		var hc = Hierarchy.CreateRelational(
			item => item.Id,
			[
				new Item<string>("A"),
				new Item<string>("A1"),
				new Item<string>("A2"),
				new Item<string>("A21"),
				new Item<string>("B"),
				new Item<string>("B1"),
				new Item<string>("B11"),
				new Item<string>("B12"),
				new Item<string>("B13"),
			],
			[
				new("A", "A1"),
				new("A", "A2"),
				new("A2", "A21"),
				new("B", "B1"),
				new("B1", "B11"),
				new("B1", "B12"),
				new("B1", "B13")
			]);

		Assert.Equal(2, hc.Roots.Count);
		Assert.Equivalent(new[] { "A", "B" }, hc.RootIds);
	}

	[Fact]
	public void CreateMapped_ReturnsCorrectRoots()
	{
		var childMap = MultiMap.Parse<string>("""
		A:A1,A2
		A1:A11,A12
		B:B1
		B1:B12
		""");

		var hc = Hierarchy.CreateMapped(childMap);

		Assert.Equal(2, hc.Roots.Count);
		Assert.Equivalent(new[] { "A", "B" }, hc.RootItems);
	}

	[Fact]
	public void CreateMatching_ReturnsTheSameStructure()
	{
		var childMap = MultiMap.Parse<string>("""
		A:A1,A2
		A2:A21
		B:B1
		B1:B11,B12,B13
		""");

		var idHierarchy = Hierarchy.CreateMapped(childMap);

		var items = new[]
		{
			new Item<string>("A"),
			new Item<string>("A1"),
			new Item<string>("A2"),
			new Item<string>("A21"),
			new Item<string>("B"),
			new Item<string>("B1"),
			new Item<string>("B11"),
			new Item<string>("B12"),
			new Item<string>("B13")
		};
		var matchingHierarchy = Hierarchy.CreateMatching(item => item.Id, items, idHierarchy);

		var idStructure = idHierarchy.ToChildMap().Render();
		var matchingStructure = matchingHierarchy.ToChildMap().Render();
		Assert.Equal(idStructure, matchingStructure);
	}


	[Fact]
	public void Clone_WithItemHierarchy_PreservesItems()
	{
		// Create hierarchy with complex items
		var items = new[]
		{
			new Item<string>("A"),
			new Item<string>("A1"),
			new Item<string>("A2"),
			new Item<string>("B"),
		};

		var childMap = new MultiMap<string>
		{
			{ "A", ["A1", "A2"] }
		};
		var originalWithItems = Hierarchy.CreateMapped(item => item.Id, items, childMap);

		// Clone it
		var cloneWithItems = originalWithItems.Clone();

		// Verify all items are preserved
		Assert.Equivalent(originalWithItems.NodeItems,
		                  cloneWithItems.NodeItems);

		// Verify nodes are new but items are same references
		var originalItem = originalWithItems.GetNode("A").Item;
		var clonedItem = cloneWithItems.GetNode("A").Item;
		Assert.Same(originalItem, clonedItem); // Same item reference
	}

	[Fact]
	public void CloneIds_CreatesIdenticalHierarchyWithNewNodes()
	{
		// Create original hierarchy
		var original = Hierarchy.CreateRelational<string>(
			new("A", "A1"),
			new("A", "A2"),
			new("A2", "A21"),
			new("B", "B1"));

		// Clone it
		var clone = original.CloneIds();

		// Verify structure is identical
		Assert.Equal(original.ToChildMap().Render(), clone.ToChildMap().Render());

		// Verify root structure
		Assert.Equal(original.Roots.Count, clone.Roots.Count);
		Assert.Equivalent(original.RootItems, clone.RootItems);

		// Verify all items are preserved
		Assert.Equivalent(original.NodeItems,
		                  clone.NodeItems);

		// Verify nodes are different instances (new nodes)
		var originalNodeA = original.GetNode("A");
		var cloneNodeA = clone.GetNode("A");
		Assert.NotSame(originalNodeA, cloneNodeA); // Different node instances
		Assert.Equal(originalNodeA.Item, cloneNodeA.Item); // Same item values

		// Verify relationships are preserved but with new nodes
		Assert.Equal(originalNodeA.Children.Count, cloneNodeA.Children.Count);
		Assert.Equivalent(originalNodeA.Children.Select(c => c.Item),
		                  cloneNodeA.Children.Select(c => c.Item));
	}
}
