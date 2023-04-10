namespace Loken.Hierarchies;

public class HierarchyLinkingTests
{
	[Fact]
	public void Attach_FromRootsDown_Works()
	{
		var hc = Hierarchy.CreateEmpty<string>();

		hc.Attach("A");
		hc.Attach("A", "A1");
		hc.Attach("A", "A2");
		hc.Attach("A2", "A21");
		hc.Attach("B");
		hc.Attach("B", "B1");
		hc.Attach("B1", "B11");

		Assert.Equal(2, hc.Roots.Count);
		Assert.Equivalent(new[] { "A", "B" }, hc.Roots.Select(r => r.Item).ToArray());
	}

	[Fact]
	public void Attach_ToNonExistentParent_Throws()
	{
		var hc = Hierarchy.CreateEmpty<string>();

		Assert.Throws<ArgumentException>(() => hc.Attach("NonExistentParentId", "Node"));
	}

	[Fact]
	public void Attach_PreBuiltRoot_Works()
	{
		var node = Node.Create("A");
		node.Attach("A1");
		node.Attach("A2");
		node.Attach("A21");

		var hc = Hierarchy.CreateEmpty<string>();
		hc.Attach(node);

		Assert.Equal(1, hc.Roots.Count);
		Assert.Equivalent(new[] { "A" }, hc.Roots.Select(r => r.Item).ToArray());
	}
}
