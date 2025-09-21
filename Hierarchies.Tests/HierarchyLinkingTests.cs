namespace Loken.Hierarchies;

public class HierarchyLinkingTests
{
	[Fact]
	public void Attach_NodesFromRootsDown_Works()
	{
		var hc = Hierarchies.Create<string>();

		hc.AttachRoot(Nodes.Create("A"));
		hc.Attach("A", Nodes.Create("A1"));
		hc.Attach("A", Nodes.Create("A2"));
		hc.Attach("A2", Nodes.Create("A21"));
		hc.AttachRoot(Nodes.Create("B"));
		hc.Attach("B", Nodes.Create("B1"));
		hc.Attach("B1", Nodes.Create("B11"));

		Assert.Equal(2, hc.Roots.Count);
		Assert.Equivalent(new[] { "A", "B" }, hc.RootItems);
	}

	[Fact]
	public void Attach_IDsFromRootsDown_Works()
	{
		var hc = Hierarchies.Create<string>();

		hc.AttachRoot("A");
		hc.Attach("A", "A1");
		hc.Attach("A", "A2");
		hc.Attach("A2", "A21");
		hc.AttachRoot("B");
		hc.Attach("B", "B1");
		hc.Attach("B1", "B11");

		Assert.Equal(2, hc.Roots.Count);
		Assert.Equivalent(new[] { "A", "B" }, hc.RootItems);
	}

	[Fact]
	public void Attach_ToNonExistentParent_Throws()
	{
		var hc = Hierarchies.Create<string>();

		Assert.Throws<ArgumentException>(() => hc.Attach("NonExistentParentId", "Node"));
	}

	[Fact]
	public void Attach_PreBuiltRoot_Works()
	{
		var node = Nodes.Create("A");
		node.Attach(Nodes.CreateMany("A1", "A2"));

		var hc = Hierarchies.Create<string>();
		hc.AttachRoot(node);

		Assert.Single(hc.Roots);
		Assert.Equivalent(new[] { "A" }, hc.RootItems);
	}

	[Fact]
	public void Attach_ToMultipleHierarchies_Throws()
	{
		var hc1 = Hierarchies.Create<string>();
		var hc2 = Hierarchies.Create<string>();

		var a = Nodes.Create("A");
		var a1 = Nodes.Create("A1");
		var a2 = Nodes.Create("A2");
		a.Attach(a1, a2);

		hc1.AttachRoot(a);
		hc2.AttachRoot("B");

		// Since both the A root and A1 child nodes are already attached to hc1,
		// we're not allowed to attach them to another hierarchy; hc2.
		Assert.Throws<InvalidOperationException>(() => hc2.AttachRoot(a));
		Assert.Throws<InvalidOperationException>(() => hc2.AttachRoot(a1));
		Assert.Throws<InvalidOperationException>(() => hc2.Attach("B", a));
		Assert.Throws<InvalidOperationException>(() => hc2.Attach("B", a1));
	}

	[Fact]
	public void NodeDetach_WhileBranded_Throws()
	{
		var hc1 = Hierarchies.Create<string>();

		var a = Nodes.Create("A");
		var a1 = Nodes.Create("A1");
		var a2 = Nodes.Create("A2");
		a.Attach(a1, a2);

		hc1.AttachRoot(a);

		// Since a1 is branded, we are not allowed to detach it using Node.Detach/DetachSelf.
		Assert.Throws<InvalidOperationException>(() => a1.DetachSelf());
		Assert.Throws<InvalidOperationException>(() => a.Detach(a1));
	}

	[Fact]
	public void HierarchyDetach_Works()
	{
		var hc1 = Hierarchies.Create<string>();
		var hc2 = Hierarchies.Create<string>();

		var a = Nodes.Create("A");
		var a1 = Nodes.Create("A1");
		var a2 = Nodes.Create("A2");
		a.Attach(a1, a2);

		hc1.AttachRoot(a);

		// By using Hierarchy.Detach which debrands a1 instead of Node.Detach,
		// we can attach a1 to another hierarchy.
		hc1.Detach(a1);
		hc2.AttachRoot(a1);
	}
}
