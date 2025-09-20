namespace Loken.Hierarchies;

public class NodeAncestorExtensionsTests
{
	// Create explicit hierarchy for testing:
	// A -> A1 -> A11
	//   -> A2 -> A12
	// B -> B1 -> B12
	// C
	private static readonly Node<string> NodeA = Nodes.Create("A");
	private static readonly Node<string> NodeA1 = Nodes.Create("A1");
	private static readonly Node<string> NodeA2 = Nodes.Create("A2");
	private static readonly Node<string> NodeA11 = Nodes.Create("A11");
	private static readonly Node<string> NodeA12 = Nodes.Create("A12");
	private static readonly Node<string> NodeB = Nodes.Create("B");
	private static readonly Node<string> NodeB1 = Nodes.Create("B1");
	private static readonly Node<string> NodeB12 = Nodes.Create("B12");
	private static readonly Node<string> NodeC = Nodes.Create("C");

	private static readonly Node<string>[] Roots = { NodeA, NodeB, NodeC };

	static NodeAncestorExtensionsTests()
	{
		// Build the hierarchy
		NodeA.Attach(NodeA1, NodeA2);
		NodeA1.Attach(NodeA11, NodeA12);
		NodeB.Attach(NodeB1);
		NodeB1.Attach(NodeB12);
	}

	#region FindAncestor Tests
	[Fact]
	public void FindAncestor_FindsFirstAncestorMatchingPredicate()
	{
		var actual = NodeA11.FindAncestor(n => n.Item.StartsWith("A"));
		Assert.Equal("A1", actual?.Item);
	}

	[Fact]
	public void FindAncestor_FindsFirstAncestorWithIncludeSelfTrue()
	{
		var actual = NodeA11.FindAncestor(n => n.Item == "A11", ascend: Ascend.WithSelf);
		Assert.Equal("A11", actual?.Item);
	}

	[Fact]
	public void FindAncestor_ReturnsNullWhenNoMatchFound()
	{
		var actual = NodeA11.FindAncestor(n => n.Item == "X");
		Assert.Null(actual);
	}

	[Fact]
	public void FindAncestor_HandlesMultipleStartingNodesWithDeduplication()
	{
		var actual = new[] { NodeA11, NodeA12 }.FindAncestor(n => n.Item == "A1");
		Assert.Equal("A1", actual?.Item);
	}

	[Fact]
	public void FindAncestor_HandlesSingleNodeOptimization()
	{
		var actual = NodeA11.FindAncestor(n => n.Item == "A1");
		Assert.Equal("A1", actual?.Item);
	}
	#endregion

	#region FindAncestors Tests
	[Fact]
	public void FindAncestors_FindsAllAncestorsMatchingPredicate()
	{
		var actual = NodeA11.FindAncestors(n => n.Item.StartsWith("A"));
		var items = actual.Select(n => n.Item).OrderBy(x => x).ToArray();
		Assert.Equal(new[] { "A", "A1" }, items);
	}

	[Fact]
	public void FindAncestors_FindsAllAncestorsWithIncludeSelfTrue()
	{
		var actual = NodeA11.FindAncestors(n => n.Item.StartsWith("A"), ascend: Ascend.WithSelf);
		var items = actual.Select(n => n.Item).OrderBy(x => x).ToArray();
		Assert.Equal(new[] { "A", "A1", "A11" }, items);
	}

	[Fact]
	public void FindAncestors_ReturnsEmptyListWhenNoMatchFound()
	{
		var actual = NodeA11.FindAncestors(n => n.Item == "X");
		Assert.Empty(actual);
	}

	[Fact]
	public void FindAncestors_HandlesMultipleStartingNodesWithDeduplication()
	{
		var actual = new[] { NodeA11, NodeA12, NodeB1 }.FindAncestors(n => new[] { "A", "A1", "B" }.Contains(n.Item), ascend: Ascend.WithSelf);
		var items = actual.Select(n => n.Item).OrderBy(x => x).ToArray();
		Assert.Equal(new[] { "A", "A1", "B" }, items);
	}

	[Fact]
	public void FindAncestors_HandlesSingleNodeOptimization()
	{
		var actual = NodeA11.FindAncestors(n => n.Item.StartsWith("A"));
		var items = actual.Select(n => n.Item).OrderBy(x => x).ToArray();
		Assert.Equal(new[] { "A", "A1" }, items);
	}
	#endregion

	#region HasAncestor Tests
	[Fact]
	public void HasAncestor_ReturnsTrueWhenAncestorExists()
	{
		var actual = NodeA11.HasAncestor(n => n.Item == "A");
		Assert.True(actual);
	}

	[Fact]
	public void HasAncestor_ReturnsFalseWhenAncestorDoesNotExist()
	{
		var actual = NodeA11.HasAncestor(n => n.Item == "B");
		Assert.False(actual);
	}

	[Fact]
	public void HasAncestor_WorksWithIncludeSelfTrue()
	{
		var actual = NodeA11.HasAncestor(n => n.Item == "A11", ascend: Ascend.WithSelf);
		Assert.True(actual);
	}
	#endregion

	#region GetAncestors Tests
	[Fact]
	public void GetAncestors_GetsAllUniqueAncestorsFromSingleNode()
	{
		var actual = NodeA11.GetAncestors();
		var items = actual.Select(n => n.Item).ToArray();
		Assert.Equal(new[] { "A1", "A" }, items);
	}

	[Fact]
	public void GetAncestors_GetsAllUniqueAncestorsWithIncludeSelfTrue()
	{
		var actual = NodeA11.GetAncestors(ascend: Ascend.WithSelf);
		var items = actual.Select(n => n.Item).ToArray();
		Assert.Equal(new[] { "A11", "A1", "A" }, items);
	}

	[Fact]
	public void GetAncestors_DeduplicatesAncestorsFromMultipleNodes()
	{
		var nodes = new[] { NodeA11, NodeA12, NodeB1 };
		var nodeItems = nodes.ToItems();
		// The input nodes are in this order: A11, A12, B1
		Assert.Equal(new[] { "A11", "A12", "B1" }, nodeItems);

		// A11 is processed first: adds A1, A (bottom-up from A11)
		// A12 is processed next: A1, A already seen, so skipped
		// B1 is processed last: adds B (bottom-up from B1)
		var expected = new[] { "A1", "A", "B" };
		var actual = nodes.GetAncestors();
		var items = actual.Select(n => n.Item).ToArray();
		Assert.Equal(expected, items);
	}

	[Fact]
	public void GetAncestors_HandlesSingleNodeOptimization()
	{
		var actual = NodeA11.GetAncestors();
		var items = actual.Select(n => n.Item).ToArray();
		Assert.Equal(new[] { "A1", "A" }, items);
	}
	#endregion
}
