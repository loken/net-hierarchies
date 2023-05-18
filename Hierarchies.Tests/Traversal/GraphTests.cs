namespace Loken.Hierarchies.Traversal;

public class GraphTests
{
	[Fact]
	public void Traverse_BreadthFirst_YieldsCorrectOrder()
	{
		var root = Node.Create(0).Attach(
			Node.Create(1).Attach(
				Node.Create(11),
				Node.Create(12).Attach(Node.Create(121))),
			Node.Create(2),
			Node.Create(3).Attach(
				Node.Create(31),
				Node.Create(32)));

		var nodes = Traverse.Graph(root, node => node.Children, type: TraversalType.BreadthFirst);

		var expected = new[] { 0, 1, 2, 3, 11, 12, 31, 32, 121 };
		var actual = nodes.AsItems().ToArray();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void Traverse_DepthFirst_YieldsCorrectOrder()
	{
		var root = Node.Create(0).Attach(
			Node.Create(1).Attach(
				Node.Create(11),
				Node.Create(12).Attach(Node.Create(121))),
			Node.Create(2),
			Node.Create(3).Attach(
				Node.Create(31),
				Node.Create(32)));

		var nodes = Traverse.Graph(root, node => node.Children, type: TraversalType.DepthFirst);

		var expected = new[] { 0, 3, 32, 31, 2, 1, 12, 121, 11 };
		var actual = nodes.AsItems().ToArray();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void Traverse_WithSkipAndExclusion_YieldsCorrectOrder()
	{
		var root = Node.Create(0).Attach(
			Node.Create(1).Attach(
				Node.Create(11),
				Node.Create(12).Attach(Node.Create(121))),
			Node.Create(2),
			Node.Create(3).Attach(
				Node.Create(31),
				Node.Create(32)));

		var nodes = Traverse.Graph(root, (node, signal) =>
		{
			// Exclude children of 3 which is 31 and 32.
			if (node.Item != 3)
				signal.Next(node.Children);

			// Skip children of 1 which is 11 and 12.
			if (node.Parent?.Item == 1)
				signal.Skip();
		}, false);

		var expected = new[] { 0, 1, 2, 3, 121 };
		var actual = nodes.AsItems().ToArray();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void Traverse_Signal_ProvidesCorrectDepth()
	{
		var root = Node.Create(0).Attach(
			Node.Create(1).Attach(
				Node.Create(11),
				Node.Create(12).Attach(Node.Create(121))),
			Node.Create(2),
			Node.Create(3).Attach(
				Node.Create(31),
				Node.Create(32)));

		var nodes = Traverse.Graph(root, (node, signal) =>
		{
			// Due to our value scheme the depth is equal to
			// the number of digits which we can get with a bit of math.
			var expectedDepth = node.Item == 0 ? 0 : Math.Floor(Math.Log10(node.Item) + 1);
			Assert.Equal(expectedDepth, signal.Depth);
		}).ToArray();
	}

	[Fact]
	public void Traverse_SkipAndEnd_FindNode()
	{
		var root = Node.Create(0).Attach(
			Node.Create(1).Attach(
				Node.Create(11),
				Node.Create(12).Attach(Node.Create(121))));

		// Let's implement a search for a single node.
		var expected = 12;
		var actual = Traverse.Graph(root, (node, signal) =>
		{
			signal.Next(node.Children);

			// We want to stop traversal once we find the item we want
			// and to skip every other item.
			if (node.Item == expected)
				signal.End();
			else
				signal.Skip();
		}).AsItems().Single();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void Traverse_Circular_BreaksOnVisited()
	{
		var last = Node.Create(4);
		var first = Node.Create(1).Attach(
			Node.Create(2).Attach(
				Node.Create(3).Attach(
					last)));

		// Make it circular!
		last.Attach(first);

		var nodes = Traverse.Graph(first, node => node.Children, true);

		var expected = new[] { 1, 2, 3, 4 };
		var actual = nodes.AsItems().ToArray();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void Traverse_BreadthFirst_ProvidesCorrectDepth()
	{
		var hierarchy = Hierarchy.CreateMapped("""
			A:A1,A2
			A1:A11,A12
			A2:A21
			B:B1
			B1:B12
			""".ParseMultiMap());

		var traversed = new List<(string item, int depth)>();
		Traverse.Graph(hierarchy.Roots, (node, signal) =>
		{
			signal.Next(node.Children);

			traversed.Add((node.Item, signal.Depth));
		}, type: TraversalType.BreadthFirst);

		// Since the items are set up using a nomenclature such that its depth is the item.Length-1, assert it!
		Assert.All(traversed, ((string item, int depth) t) => Assert.Equal(t.item.Length - 1, t.depth));
	}

	[Fact]
	public void Traverse_DepthFirst_ProvidesCorrectDepth()
	{
		var hierarchy = Hierarchy.CreateMapped("""
			A:A1,A2
			A1:A11,A12
			A2:A21
			B:B1
			B1:B12
			""".ParseMultiMap());

		var traversed = new List<(string item, int depth)>();
		Traverse.Graph(hierarchy.Roots, (node, signal) =>
		{
			signal.Next(node.Children);

			traversed.Add((node.Item, signal.Depth));
		}, type: TraversalType.DepthFirst);

		// Since the items are set up using a nomenclature such that its depth is the item.Length-1, assert it!
		Assert.All(traversed, ((string item, int depth) t) => Assert.Equal(t.item.Length - 1, t.depth));
	}
}