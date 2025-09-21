namespace Loken.Hierarchies;

public class NodeCommonAncestorExtensionsTests
{
	// Create hierarchy of individually attached nodes so we have references.
	// A -> A1 -> A11
	//   -> A2 -> A12
	// B -> B1 -> B12
	// C
	private static readonly Node<string> NodeRoot = Nodes.Create("Root");
	private static readonly Node<string> NodeA = Nodes.Create("A");
	private static readonly Node<string> NodeB = Nodes.Create("B");
	private static readonly Node<string> NodeA1 = Nodes.Create("A1");
	private static readonly Node<string> NodeA2 = Nodes.Create("A2");
	private static readonly Node<string> NodeA11 = Nodes.Create("A11");
	private static readonly Node<string> NodeA12 = Nodes.Create("A12");
	private static readonly Node<string> NodeA21 = Nodes.Create("A21");
	private static readonly Node<string> NodeA22 = Nodes.Create("A22");
	private static readonly Node<string> NodeB1 = Nodes.Create("B1");
	private static readonly Node<string> NodeB2 = Nodes.Create("B2");
	private static readonly Node<string> NodeB11 = Nodes.Create("B11");

	static NodeCommonAncestorExtensionsTests()
	{
		// Build the hierarchy
		NodeRoot.Attach(NodeA, NodeB);
		NodeA.Attach(NodeA1, NodeA2);
		NodeA1.Attach(NodeA11, NodeA12);
		NodeA2.Attach(NodeA21, NodeA22);
		NodeB.Attach(NodeB1, NodeB2);
		NodeB1.Attach(NodeB11);
	}

	#region FindCommonAncestor Tests
	[Fact]
	public void FindCommonAncestor_FindsClosestCommonAncestor()
	{
		// A11 and A12 should have A1 as common ancestor
		var actual = new[] { NodeA11, NodeA12 }.FindCommonAncestor();
		Assert.Equal(NodeA1, actual);

		// A11 and A21 should have A as common ancestor
		var actual2 = new[] { NodeA11, NodeA21 }.FindCommonAncestor();
		Assert.Equal(NodeA, actual2);

		// A11 and B11 should have Root as common ancestor
		var actual3 = new[] { NodeA11, NodeB11 }.FindCommonAncestor();
		Assert.Equal(NodeRoot, actual3);
	}

	[Fact]
	public void FindCommonAncestor_WithSingleNodeReturnsFirstAncestor()
	{
		// A11's first ancestor should be A1
		var actual = new[] { NodeA11 }.FindCommonAncestor();
		// Updated traversal now returns the immediate parent (A1) unchanged
		Assert.Equal(NodeA1, actual);
	}

	[Fact]
	public void FindCommonAncestor_WithSingleRootNodeReturnsNull()
	{
		// Root has no ancestors
		var actual = new[] { NodeRoot }.FindCommonAncestor();
		Assert.Null(actual);
	}

	[Fact]
	public void FindCommonAncestor_WithNonExistentNodesReturnsNull()
	{
		var nonExistent1 = Nodes.Create("NonExistent1");
		var nonExistent2 = Nodes.Create("NonExistent2");
		var actual = new[] { nonExistent1, nonExistent2 }.FindCommonAncestor();
		Assert.Null(actual);
	}

	[Fact]
	public void FindCommonAncestor_WithIncludeSelfFindsAncestorIncludingTargetNodes()
	{
		// A1 and A11 with includeSelf should return A1 (since A1 is ancestor of A11 and includeSelf includes A1)
		var actual = new[] { NodeA1, NodeA11 }.FindCommonAncestor(Ascend.WithSelf);
		Assert.Equal(NodeA1, actual);
	}
	#endregion

	#region FindCommonAncestors Tests
	[Fact]
	public void FindCommonAncestors_ReturnsAllCommonAncestors()
	{
		var actual = new[] { NodeA11, NodeA12 }.FindCommonAncestors();
		Assert.Equal([NodeA1, NodeA, NodeRoot], actual);
	}

	[Fact]
	public void FindCommonAncestors_ReturnsAllAncestorsForSingleNode()
	{
		var actual = new[] { NodeA11 }.FindCommonAncestors();
		// New ordering yields deepest first including the node itself? Current implementation includes only ancestors; adjust if needed.
		Assert.Equal([NodeA1, NodeA, NodeRoot], actual);
	}

	[Fact]
	public void FindCommonAncestors_ReturnsEmptyForSingleRoot()
	{
		var actual = new[] { NodeRoot }.FindCommonAncestors();
		Assert.Empty(actual);
	}

	[Fact]
	public void FindCommonAncestors_WithIncludeSelfReturnsQueriedRootItem()
	{
		var actual = new[] { NodeRoot }.FindCommonAncestors(Ascend.WithSelf);
		Assert.Equal([NodeRoot], actual);
	}

	[Fact]
	public void FindCommonAncestors_WithUnrelatedBranchesReturnsRoot()
	{
		var actual = new[] { NodeA11, NodeB11 }.FindCommonAncestors();
		Assert.Equal([NodeRoot], actual);
	}

	[Fact]
	public void FindCommonAncestors_WithNoCommonAncestorsReturnsEmpty()
	{
		var nonExistent1 = Nodes.Create("NonExistent1");
		var nonExistent2 = Nodes.Create("NonExistent2");
		var actual = new[] { nonExistent1, nonExistent2 }.FindCommonAncestors();
		Assert.Empty(actual);
	}
	#endregion

	#region FindCommonAncestorSet Tests
	[Fact]
	public void FindCommonAncestorSet_ReturnsSetOfCommonAncestors()
	{
		var actual = new[] { NodeA11, NodeA12 }.FindCommonAncestorSet();
		Assert.Equal([ NodeA1, NodeA, NodeRoot ], actual);
	}

	[Fact]
	public void FindCommonAncestorSet_ReturnsEmptySetWhenThereAreNoCommonAncestors()
	{
		var nonExistent1 = Nodes.Create("NonExistent1");
		var nonExistent2 = Nodes.Create("NonExistent2");
		var actual = new[] { nonExistent1, nonExistent2 }.FindCommonAncestorSet();
		Assert.Empty(actual!);
	}
	#endregion
}
