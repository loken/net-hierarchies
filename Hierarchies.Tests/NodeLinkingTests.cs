namespace Loken.Hierarchies;

public class NodeLinkingTests
{
	[Fact]
	public void Attach_LinksBothWays()
	{
		Node<string> root = "root";
		Node<string> child = "child";

		root.Attach(child);

		Assert.True(root.IsLinked);
		Assert.False(root.IsLeaf);

		Assert.True(child.IsLinked);
		Assert.False(child.IsRoot);
	}

	[Fact]
	public void Detach_UnlinksBothWays()
	{
		Node<string> root = "root";
		Node<string> child = "child";
		root.Attach(child);

		child.Detach();

		Assert.False(root.IsLinked);
		Assert.False(child.IsLinked);
	}

	[Fact]
	public void Dismantle_ExcludeAncestry_UnlinksEverything()
	{
		var branchA = Node.Create("A").Attach("a1", "a2", Node.Create("a3").Attach("a31"));
		var branchB = Node.Create("B").Attach("b1", Node.Create("b2").Attach("b21"));
		var root = Node.Create("root").Attach(branchA, branchB);

		var descendantsOfA = branchA.GetDescendants(false).ToArray();
		var other = root.GetDescendants(true).Where(n => !n.Value.StartsWith('a')).ToArray();

		Assert.Equal(4, descendantsOfA.Length);
		Assert.Equal(6, other.Length);

		branchA.Dismantle(false);

		// The branch that was dismantled is now a leaf, but not a root as it's still connected to it's parent.
		Assert.True(branchA.IsLeaf);
		Assert.False(branchA.IsRoot);
		Assert.Equal(root, branchB.Parent);

		// The descendants of the branch however are no longer linked.
		Assert.All(descendantsOfA, node => Assert.False(node.IsLinked));

		// All of the other nodes are still linked in some way.
		Assert.All(other, node => Assert.True(node.IsLinked));
	}

	[Fact]
	public void Dismantle_IncludeAncestry_UnlinksEverything()
	{
		var branchA = Node.Create("a").Attach("a1", "a2", Node.Create("a3").Attach("a31"));
		var branchB = Node.Create("b").Attach("b1", Node.Create("b2").Attach("b21"));
		var root = Node.Create("root").Attach(branchA, branchB);

		var nodes = root.GetDescendants(true).ToArray();

		Assert.Equal(10, nodes.Length);

		// By picking a branch we assert that we dismantle children, parents and siblings.
		branchA.Dismantle(true);

		Assert.All(nodes, node => Assert.False(node.IsLinked));
	}
}
