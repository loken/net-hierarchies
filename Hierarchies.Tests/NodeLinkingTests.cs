namespace Loken.Hierarchies;

public class NodeLinkingTests
{
	[Fact]
	public void NodeAttach_LinksBothWays()
	{
		var root = Nodes.Create("root");
		var child = Nodes.Create("child");

		root.Attach(child);

		Assert.True(root.IsLinked);
		Assert.False(root.IsLeaf);

		Assert.True(child.IsLinked);
		Assert.False(child.IsRoot);
	}

	[Fact]
	public void NodeDetach_UnlinksBothWays()
	{
		var root = Nodes.Create("root");
		var child = Nodes.Create("child");

		root.Attach(child);
		root.Detach(child);

		Assert.False(root.IsLinked);
		Assert.False(child.IsLinked);
	}

	[Fact]
	public void NodeDetachSelf_UnlinksBothWays()
	{
		var root = Nodes.Create("root");
		var child = Nodes.Create("child");

		root.Attach(child);
		child.DetachSelf();

		Assert.False(root.IsLinked);
		Assert.False(child.IsLinked);
	}

	[Fact]
	public void NodeDismantle_ExcludeAncestry_UnlinksEverythingUnderTheNode()
	{
		var branchA = Nodes.Create("A").Attach(Nodes.Create("a1"), Nodes.Create("a2"), Nodes.Create("a3").Attach(Nodes.Create("a31")));
		var branchB = Nodes.Create("B").Attach(Nodes.Create("b1"), Nodes.Create("b2").Attach(Nodes.Create("b21")));
		var root = Nodes.Create("root").Attach(branchA, branchB);

		var descendantsOfA = branchA.GetDescendants().ToArray();
		var other = root.GetDescendants(Descend.WithSelf).Where(n => !n.Item.StartsWith('a')).ToArray();

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
	public void NodeDismantle_IncludeAncestry_UnlinksEverythingIncludingTheAncestryAndItsBranches()
	{
		var branchA = Nodes.Create("A").Attach(Nodes.Create("a1"), Nodes.Create("a2"), Nodes.Create("a3").Attach(Nodes.Create("a31")));
		var branchB = Nodes.Create("B").Attach(Nodes.Create("b1"), Nodes.Create("b2").Attach(Nodes.Create("b21")));
		var root = Nodes.Create("root").Attach(branchA, branchB);

		var nodes = root.GetDescendants(Descend.WithSelf).ToArray();

		Assert.Equal(10, nodes.Length);

		// By picking a branch we assert that we dismantle children, parents and siblings.
		branchA.Dismantle(true);

		Assert.All(nodes, node => Assert.False(node.IsLinked));
	}
}
