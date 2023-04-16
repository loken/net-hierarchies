namespace Loken.Hierarchies;

public class HierarchyLinkingTests
{
	[Fact]
	public void Attach_FromRootsDown_Works()
	{
		var hc = Hierarchy.CreateEmpty<string>();

		hc.Attach(Node.Create("A"));
		hc.Attach("A", Node.Create("A1"));
		hc.Attach("A", Node.Create("A2"));
		hc.Attach("A2", Node.Create("A21"));
		hc.Attach(Node.Create("B"));
		hc.Attach("B", Node.Create("B1"));
		hc.Attach("B1", Node.Create("B11"));

		Assert.Equal(2, hc.Roots.Count);
		Assert.Equivalent(new[] { "A", "B" }, hc.Roots.Select(r => r.Item).ToArray());
	}

	[Fact]
	public void Attach_ToNonExistentParent_Throws()
	{
		var hc = Hierarchy.CreateEmpty<string>();

		Assert.Throws<ArgumentException>(() => hc.Attach("NonExistentParentId", Node.Create("Node")));
	}

	[Fact]
	public void Attach_PreBuiltRoot_Works()
	{
		var node = Node.Create("A");
		node.Attach(Node.CreateMany("A1", "A2"));

		var hc = Hierarchy.CreateEmpty<string>();
		hc.Attach(node);

		Assert.Equal(1, hc.Roots.Count);
		Assert.Equivalent(new[] { "A" }, hc.Roots.Select(r => r.Item).ToArray());
	}

	[Fact]
	public void Attach_ToMultipleHierarchies_Throws()
	{
		var hc1 = Hierarchy.CreateEmpty<string>();
		var hc2 = Hierarchy.CreateEmpty<string>();

		var a = Node.Create("A");
		var a1 = Node.Create("A1");
		var a2 = Node.Create("A2");
		a.Attach(a1, a2);

		hc1.Attach(a);

		// Since both the A root and A1 child nodes are already attached to hc1,
		// we're not allowed to attach them to another hierarchy; hc2.
		Assert.Throws<InvalidOperationException>(() => hc2.Attach(hc1.GetNode("A")));
		Assert.Throws<InvalidOperationException>(() => hc2.Attach(hc1.GetNode("A1")));
	}

	[Fact]
	public void NodeDetach_WhileBranded_Throws()
	{
		var hc1 = Hierarchy.CreateEmpty<string>();
		var hc2 = Hierarchy.CreateEmpty<string>();

		var a = Node.Create("A");
		var a1 = Node.Create("A1");
		var a2 = Node.Create("A2");
		a.Attach(a1, a2);

		hc1.Attach(a);

		// Since a1 is branded, we are not allowed to detach it using Node.Detach/DetachSelf.
		Assert.Throws<InvalidOperationException>(() => a1.DetachSelf());
		Assert.Throws<InvalidOperationException>(() => a.Detach(a1));
	}

	[Fact]
	public void HierarchyDetach_Works()
	{
		var hc1 = Hierarchy.CreateEmpty<string>();
		var hc2 = Hierarchy.CreateEmpty<string>();

		var a = Node.Create("A");
		var a1 = Node.Create("A1");
		var a2 = Node.Create("A2");
		a.Attach(a1, a2);

		hc1.Attach(a);

		// By using Hierarchy.Detach which debrands a1 instead of Node.Detach,
		// we can attach a1 to another hierarchy.
		hc1.Detach(a1);
		hc2.Attach(a1);
	}
}
