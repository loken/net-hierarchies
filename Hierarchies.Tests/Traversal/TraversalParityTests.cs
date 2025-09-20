namespace Loken.Hierarchies.Traversal;

public class TraversalParityTests
{
	[Fact]
	public void Visited_UsesReferenceIdentity_NotItemEquality()
	{
		var a = new Node<string> { Item = "X" };
		var b1 = new Node<string> { Item = "Y" };
		var b2 = new Node<string> { Item = "Y" }; // same item value, different instance
		a.Attach(b1);
		a.Attach(b2);

		var seen = new List<Node<string>>();
		foreach (var n in Traverse.Graph(a, n => n.Children, new Descend(DetectCycles: true)))
			seen.Add(n);

		// We expect both Y nodes to appear, since they are different instances.
		Assert.Contains(b1, seen);
		Assert.Contains(b2, seen);
	}

	[Fact]
	public void Children_Enumeration_IsInsertionOrdered()
	{
		var p = new Node<int> { Item = 0 };
		var c1 = new Node<int> { Item = 1 };
		var c2 = new Node<int> { Item = 2 };
		var c3 = new Node<int> { Item = 3 };
		p.Attach(c1, c2, c3);

		var order = p.Children.Select(n => n.Item).ToArray();
		Assert.Equal(new[] { 1, 2, 3 }, order);
	}
}
