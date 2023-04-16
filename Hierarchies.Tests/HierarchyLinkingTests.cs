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
}
