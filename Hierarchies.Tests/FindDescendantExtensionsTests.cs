namespace Loken.Hierarchies;

public class FindDescendantExtensionsTests
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

	static FindDescendantExtensionsTests()
	{
		// Build the hierarchy
		NodeA.Attach(NodeA1, NodeA2);
		NodeA1.Attach(NodeA11, NodeA12);
		NodeB.Attach(NodeB1);
		NodeB1.Attach(NodeB12);
	}

	#region FindDescendant Tests

	[Fact]
	public void FindDescendant_FindsFirstDescendantMatchingPredicate()
	{
		var actual = Roots.FindDescendant(n => n.Item.StartsWith("A1"));
		Assert.Equal("A1", actual?.Item);
	}

	[Fact]
	public void FindDescendant_FindsFirstDescendantWithIncludeSelfTrue()
	{
		var actual = NodeA.FindDescendant(n => n.Item == "A", true);
		Assert.Equal("A", actual?.Item);
	}

	[Fact]
	public void FindDescendant_ReturnsNullWhenNoMatchFound()
	{
		var actual = Roots.FindDescendant(n => n.Item == "X");
		Assert.Null(actual);
	}

	[Fact]
	public void FindDescendant_SupportsBreadthFirstAndDepthFirstTraversal()
	{
		// Breadth-first traversal: A1, A2, B1, A11, A12, B12 => A11
		var breadthFirst = Roots.FindDescendant(n => n.Item == "A11" || n.Item == "B12", false, TraversalType.BreadthFirst);
		Assert.Equal("A11", breadthFirst?.Item);

		// Depth-first traversal: B1, B12, A2, A1, A12, A11 => B12
		var depthFirst = Roots.FindDescendant(n => n.Item == "A11" || n.Item == "B12", false, TraversalType.DepthFirst);
		Assert.Equal("B12", depthFirst?.Item);
	}

	#endregion

	#region FindDescendants Tests

	[Fact]
	public void FindDescendants_FindsAllDescendantsMatchingPredicate()
	{
		var actual = Roots.FindDescendants(n => n.Item.StartsWith("A1"));
		var items = actual.Select(n => n.Item).OrderBy(x => x).ToArray();
		Assert.Equal(new[] { "A1", "A11", "A12" }, items);
	}

	[Fact]
	public void FindDescendants_FindsAllDescendantsWithIncludeSelfTrue()
	{
		var actual = NodeA.FindDescendants(n => n.Item.StartsWith("A"), true);
		var items = actual.Select(n => n.Item).OrderBy(x => x).ToArray();
		Assert.Equal(new[] { "A", "A1", "A11", "A12", "A2" }, items);
	}

	[Fact]
	public void FindDescendants_ReturnsEmptyListWhenNoMatchFound()
	{
		var actual = Roots.FindDescendants(n => n.Item == "X");
		Assert.Empty(actual);
	}

	#endregion

	#region HasDescendant Tests

	[Fact]
	public void HasDescendant_ReturnsTrueWhenDescendantExists()
	{
		var actual = Roots.HasDescendant(n => n.Item == "A11");
		Assert.True(actual);
	}

	[Fact]
	public void HasDescendant_ReturnsFalseWhenDescendantDoesNotExist()
	{
		var actual = Roots.HasDescendant(n => n.Item == "X");
		Assert.False(actual);
	}

	[Fact]
	public void HasDescendant_WorksWithIncludeSelfTrue()
	{
		var actual = NodeA.HasDescendant(n => n.Item == "A", true);
		Assert.True(actual);
	}

	#endregion

	#region GetDescendantItems Tests

	[Fact]
	public void GetDescendantItems_GetsDescendantItemsFromNodes()
	{
		var actual = NodeA.GetDescendantItems();
		Assert.Equal(new[] { "A1", "A2", "A11", "A12" }, actual.ToArray());
	}

	[Fact]
	public void GetDescendantItems_GetsDescendantItemsWithIncludeSelfTrue()
	{
		var actual = NodeA.GetDescendantItems(true);
		Assert.Equal(new[] { "A", "A1", "A2", "A11", "A12" }, actual.ToArray());
	}

	#endregion

	#region TraverseDescendants Tests

	[Fact]
	public void TraverseDescendants_ReturnsEnumerableForDescendants()
	{
		var actual = NodeA.TraverseDescendants();
		var items = actual.Select(n => n.Item).ToArray();
		Assert.Equal(new[] { "A1", "A2", "A11", "A12" }, items);
	}

	[Fact]
	public void TraverseDescendants_WorksWithIncludeSelfTrue()
	{
		var actual = NodeA.TraverseDescendants(true);
		var items = actual.Select(n => n.Item).ToArray();
		Assert.Equal(new[] { "A", "A1", "A2", "A11", "A12" }, items);
	}

	#endregion
}
