namespace Loken.Hierarchies;

public class NodeLinkingTests
{
	[Fact]
	public void Attach_LinksBothWays()
	{
		Node<string> root = Node.Create("root");
		Node<string> child = Node.Create("child");

		root.Attach(child);

		Assert.True(root.IsLinked);
		Assert.False(root.IsLeaf);

		Assert.True(child.IsLinked);
		Assert.False(child.IsRoot);
	}

	[Fact]
	public void Detach_UnlinksBothWays()
	{
		Node<string> root = Node.Create("root");
		Node<string> child = Node.Create("child");
		root.Attach(child);

		root.Detach(child);

		Assert.False(root.IsLinked);
		Assert.False(child.IsLinked);
	}

	[Fact]
	public void DetachSelf_UnlinksBothWays()
	{
		Node<string> root = Node.Create("root");
		Node<string> child = Node.Create("child");
		root.Attach(child);

		child.DetachSelf();

		Assert.False(root.IsLinked);
		Assert.False(child.IsLinked);
	}

	[Fact]
	public void Dismantle_ExcludeAncestry_UnlinksEverything()
	{
		var branchA = Node.Create("A").Attach(Node.Create("a1"), Node.Create("a2"), Node.Create("a3").Attach(Node.Create("a31")));
		var branchB = Node.Create("B").Attach(Node.Create("b1"), Node.Create("b2").Attach(Node.Create("b21")));
		var root = Node.Create("root").Attach(branchA, branchB);

		var descendantsOfA = branchA.GetDescendants().ToArray();
		var other = root.GetDescendants(true).Where(n => !n.Item.StartsWith('a')).ToArray();

		Assert.Equal(4, descendantsOfA.Length);
		Assert.Equal(6, other.Length);

		branchA.Dismantle(false);

		// The branch that was dismantled is now a leaf, but not a root as it's still connected to it's parent.
		Assert.True(branchA.IsLeaf);
		Assert.False(branchA.IsRoot);
		Assert.Equal(root, branchA.Parent);
		Assert.Equal(root, branchB.Parent);

		// The descendants of the branch however are no longer linked.
		Assert.All(descendantsOfA, node => Assert.False(node.IsLinked));

		// All of the other nodes are still linked in some way.
		Assert.All(other, node => Assert.True(node.IsLinked));
	}

	[Fact]
	public void Dismantle_IncludeAncestry_UnlinksEverything()
	{
		var branchA = Node.Create("a").Attach(Node.Create("a1"), Node.Create("a2"), Node.Create("a3").Attach(Node.Create("a31")));
		var branchB = Node.Create("b").Attach(Node.Create("b1"), Node.Create("b2").Attach(Node.Create("b21")));
		var root = Node.Create("root").Attach(branchA, branchB);

		var nodes = root.GetDescendants(true).ToArray();

		Assert.Equal(10, nodes.Length);

		// By picking a branch we assert that we dismantle children, parents and siblings.
		branchA.Dismantle(true);

		Assert.All(nodes, node => Assert.False(node.IsLinked));
	}
}
