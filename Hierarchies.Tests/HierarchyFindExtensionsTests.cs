namespace Loken.Hierarchies;

/// <summary>
/// Tests for general hierarchy find operations.
/// </summary>
public class HierarchyFindExtensionsTests
{
	// Test data setup - equivalent to TypeScript test
	private readonly Hierarchy<string> hierarchy = Hierarchy.CreateMapped(
		MultiMap.Parse<string>("""
			A:A1,A2
			A1:A11,A12
			A2:A21
			B:B1
			B1:B11,B12
			"""));

	[Fact]
	public void Find_WithExistingIds_ReturnsMatchingNodes()
	{
		var foundNodes = hierarchy.Find(["A", "B"]);

		Assert.Equal(2, foundNodes.Count);
		Assert.Equal(["A", "B"], foundNodes.Select(n => n.Item).ToArray());
	}

	[Fact]
	public void Find_WithMixOfExistingAndNonExistingIds_ReturnsOnlyExisting()
	{
		var foundNodes = hierarchy.Find(["A", "NonExistent", "B"]);

		Assert.Equal(2, foundNodes.Count);
		Assert.Equal(["A", "B"], foundNodes.Select(n => n.Item).ToArray());
	}

	[Fact]
	public void Find_WithAllNonExistingIds_ReturnsEmptyArray()
	{
		var foundNodes = hierarchy.Find(["NonExistent1", "NonExistent2"]);

		Assert.Empty(foundNodes);
	}

	[Fact]
	public void Find_WithSingleExistingId_ReturnsMatchingNode()
	{
		var foundNodes = hierarchy.Find("A");

		Assert.Single(foundNodes);
		Assert.Equal("A", foundNodes[0].Item);
	}

	[Fact]
	public void Find_WithSingleNonExistingId_ReturnsEmptyArray()
	{
		var foundNodes = hierarchy.Find("NonExistent");

		Assert.Empty(foundNodes);
	}

	[Fact]
	public void Find_WithPredicate_ReturnsMatchingNodes()
	{
		var foundNodes = hierarchy.Find(node => node.Item.Length == 1);

		Assert.Equal(2, foundNodes.Count);
		Assert.Contains(foundNodes, n => n.Item == "A");
		Assert.Contains(foundNodes, n => n.Item == "B");
	}

	[Fact]
	public void Find_WithPredicateForLeafNodes_ReturnsAllLeaves()
	{
		var foundNodes = hierarchy.Find(node => node.IsLeaf);

		Assert.Equal(5, foundNodes.Count);
		var leafItems = foundNodes.Select(n => n.Item).ToArray();
		Assert.Contains("A11", leafItems);
		Assert.Contains("A12", leafItems);
		Assert.Contains("A21", leafItems);
		Assert.Contains("B11", leafItems);
		Assert.Contains("B12", leafItems);
	}

	[Fact]
	public void FindItems_WithExistingIds_ReturnsMatchingItems()
	{
		var foundItems = hierarchy.FindItems(["A", "B"]);

		Assert.Equal(2, foundItems.Count);
		Assert.Equal(["A", "B"], foundItems.ToArray());
	}

	[Fact]
	public void FindItems_WithMixOfExistingAndNonExistingIds_ReturnsOnlyExisting()
	{
		var foundItems = hierarchy.FindItems(["A", "NonExistent", "B"]);

		Assert.Equal(2, foundItems.Count);
		Assert.Equal(["A", "B"], foundItems.ToArray());
	}

	[Fact]
	public void FindItems_WithAllNonExistingIds_ReturnsEmptyArray()
	{
		var foundItems = hierarchy.FindItems(["NonExistent1", "NonExistent2"]);

		Assert.Empty(foundItems);
	}

	[Fact]
	public void FindItems_WithSingleExistingId_ReturnsMatchingItem()
	{
		var foundItems = hierarchy.FindItems("A");

		Assert.Single(foundItems);
		Assert.Equal("A", foundItems[0]);
	}

	[Fact]
	public void FindItems_WithSingleNonExistingId_ReturnsEmptyArray()
	{
		var foundItems = hierarchy.FindItems("NonExistent");

		Assert.Empty(foundItems);
	}

	[Fact]
	public void FindItems_WithPredicate_ReturnsMatchingItems()
	{
		var foundItems = hierarchy.FindItems(node => node.Item.Length == 1);

		Assert.Equal(2, foundItems.Count);
		Assert.Contains("A", foundItems);
		Assert.Contains("B", foundItems);
	}

	[Fact]
	public void FindItems_WithPredicateForLeafNodes_ReturnsAllLeafItems()
	{
		var foundItems = hierarchy.FindItems(node => node.IsLeaf);

		Assert.Equal(5, foundItems.Count);
		Assert.Contains("A11", foundItems);
		Assert.Contains("A12", foundItems);
		Assert.Contains("A21", foundItems);
		Assert.Contains("B11", foundItems);
		Assert.Contains("B12", foundItems);
	}

	[Fact]
	public void FindIds_WithExistingIds_ReturnsMatchingIds()
	{
		var foundIds = hierarchy.FindIds(["A", "B"]);

		Assert.Equal(2, foundIds.Count);
		Assert.Equal(["A", "B"], foundIds.ToArray());
	}

	[Fact]
	public void FindIds_WithMixOfExistingAndNonExistingIds_ReturnsOnlyExisting()
	{
		var foundIds = hierarchy.FindIds(["A", "NonExistent", "B"]);

		Assert.Equal(2, foundIds.Count);
		Assert.Equal(["A", "B"], foundIds.ToArray());
	}

	[Fact]
	public void FindIds_WithAllNonExistingIds_ReturnsEmptyArray()
	{
		var foundIds = hierarchy.FindIds(["NonExistent1", "NonExistent2"]);

		Assert.Empty(foundIds);
	}

	[Fact]
	public void FindIds_WithSingleExistingId_ReturnsMatchingId()
	{
		var foundIds = hierarchy.FindIds("A");

		Assert.Single(foundIds);
		Assert.Equal("A", foundIds[0]);
	}

	[Fact]
	public void FindIds_WithSingleNonExistingId_ReturnsEmptyArray()
	{
		var foundIds = hierarchy.FindIds("NonExistent");

		Assert.Empty(foundIds);
	}

	[Fact]
	public void FindIds_WithPredicate_ReturnsMatchingIds()
	{
		var foundIds = hierarchy.FindIds(node => node.Item.Length == 1);

		Assert.Equal(2, foundIds.Count);
		Assert.Contains("A", foundIds);
		Assert.Contains("B", foundIds);
	}

	[Fact]
	public void FindIds_WithPredicateForLeafNodes_ReturnsAllLeafIds()
	{
		var foundIds = hierarchy.FindIds(node => node.IsLeaf);

		Assert.Equal(5, foundIds.Count);
		Assert.Contains("A11", foundIds);
		Assert.Contains("A12", foundIds);
		Assert.Contains("A21", foundIds);
		Assert.Contains("B11", foundIds);
		Assert.Contains("B12", foundIds);
	}
}
