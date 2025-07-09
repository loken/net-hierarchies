namespace Loken.Hierarchies;

public class FindCommonAncestorExtensionsTests
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

	static FindCommonAncestorExtensionsTests()
	{
		// Build the hierarchy
		NodeA.Attach(NodeA1, NodeA2);
		NodeA1.Attach(NodeA11, NodeA12);
		NodeB.Attach(NodeB1);
		NodeB1.Attach(NodeB12);
	}

	#region FindCommonAncestor Tests

	[Fact]
	public void FindCommonAncestor_ReturnsTheClosestCommonAncestor()
	{
		var actual = new[] { NodeA11, NodeA2 }.FindCommonAncestor();
		Assert.Equal(NodeA, actual);
	}

	[Fact]
	public void FindCommonAncestor_ReturnsNullWhenThereIsNoCommonAncestor()
	{
		var actual = new[] { NodeA1, NodeB1 }.FindCommonAncestor();
		Assert.Null(actual);
	}

	[Fact]
	public void FindCommonAncestor_WorksWithIncludeSelfTrue()
	{
		var actual = new[] { NodeA1, NodeA2 }.FindCommonAncestor(true);
		Assert.Equal(NodeA, actual);
	}

	[Fact]
	public void FindCommonAncestor_ReturnsTheNodeItselfWhenSingleNodePassed()
	{
		var actual = NodeA11.FindCommonAncestor(true);
		Assert.Equal(NodeA11, actual);
	}

	#endregion

	#region FindCommonAncestors Tests

	[Fact]
	public void FindCommonAncestors_ReturnsAllCommonAncestors()
	{
		var actual = new[] { NodeA11, NodeA12 }.FindCommonAncestors();
		var items = actual.Select(n => n.Item).OrderBy(x => x).ToArray();
		Assert.Equal(new[] { "A", "A1" }, items);
	}

	[Fact]
	public void FindCommonAncestors_ReturnsEmptyListWhenThereAreNoCommonAncestors()
	{
		var actual = new[] { NodeA1, NodeB1 }.FindCommonAncestors();
		Assert.Empty(actual);
	}

	#endregion

	#region FindCommonAncestorItems Tests

	[Fact]
	public void FindCommonAncestorItems_ReturnsCommonAncestorItems()
	{
		var actual = new[] { NodeA11, NodeA12 }.FindCommonAncestorItems();
		var items = actual.OrderBy(x => x).ToArray();
		Assert.Equal(new[] { "A", "A1" }, items);
	}

	[Fact]
	public void FindCommonAncestorItems_ReturnsEmptyListWhenThereAreNoCommonAncestors()
	{
		var actual = new[] { NodeA1, NodeB1 }.FindCommonAncestorItems();
		Assert.Empty(actual);
	}

	#endregion

	#region FindCommonAncestorSet Tests

	[Fact]
	public void FindCommonAncestorSet_ReturnsSetOfCommonAncestors()
	{
		var actual = new[] { NodeA11, NodeA12 }.FindCommonAncestorSet();
		var items = actual!.Select(n => n.Item).OrderBy(x => x).ToArray();
		Assert.Equal(new[] { "A", "A1" }, items);
	}

	[Fact]
	public void FindCommonAncestorSet_ReturnsEmptySetWhenThereAreNoCommonAncestors()
	{
		var actual = new[] { NodeA1, NodeB1 }.FindCommonAncestorSet();
		Assert.Empty(actual!);
	}

	#endregion
}
