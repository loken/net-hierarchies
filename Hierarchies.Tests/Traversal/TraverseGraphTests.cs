namespace Loken.Hierarchies.Traversal;

public class TraverseGraphTests
{
	protected static Node<int> IntRoot { get; } =
		Nodes.Create(0).Attach(
			Nodes.Create(1).Attach(
				Nodes.Create(11),
				Nodes.Create(12).Attach(Nodes.Create(121))),
			Nodes.Create(2),
			Nodes.Create(3).Attach(Nodes.CreateMany(31, 32)));

	protected static Node<string>[] StrRoots { get; } = [
		Nodes.Create("A").Attach(
			Nodes.Create("A1").Attach(Nodes.CreateMany("A11", "A12")),
			Nodes.Create("A2").Attach(Nodes.Create("A21"))),
		Nodes.Create("B").Attach(Nodes.Create("B1").Attach(Nodes.Create("B12"))),
	];

	[Fact]
	public void TraverseNext_BreadthFirst_YieldsCorrectOrder()
	{
		var nodes = Traverse.Graph(IntRoot, node => node.Children, type: TraversalType.BreadthFirst);

		var expected = new[] { 0, 1, 2, 3, 11, 12, 31, 32, 121 };
		var actual = nodes.ToItems().ToArray();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void TraverseNext_DepthFirst_YieldsCorrectOrder()
	{
		var nodes = Traverse.Graph(IntRoot, node => node.Children, type: TraversalType.DepthFirst);

		var expected = new[] { 0, 3, 32, 31, 2, 1, 12, 121, 11 };
		var actual = nodes.ToItems().ToArray();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void TraverseSignal_BreadthFirst_YieldsCorrectOrder()
	{
		var nodes = Traverse.Graph(IntRoot, (node, signal) =>
		{
			signal.Next(node.Children);
		}, type: TraversalType.BreadthFirst);

		var expected = new[] { 0, 1, 2, 3, 11, 12, 31, 32, 121 };
		var actual = nodes.ToItems().ToArray();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void TraverseSignal_DepthFirst_YieldsCorrectOrder()
	{
		var nodes = Traverse.Graph(IntRoot, (node, signal) =>
		{
			signal.Next(node.Children);
		}, type: TraversalType.DepthFirst);

		var expected = new[] { 0, 3, 32, 31, 2, 1, 12, 121, 11 };
		var actual = nodes.ToItems().ToArray();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void TraverseSignal_WithSkip_YieldsCorrectOrder()
	{
		var nodes = Traverse.Graph(IntRoot, (node, signal) =>
		{
			// Exclude children of 3 which is 31 and 32.
			if (node.Item != 3)
				signal.Next(node.Children);

			// Skip children of 1 which is 11 and 12.
			if (node.Parent?.Item == 1)
				signal.Skip();
		}, false);

		var expected = new[] { 0, 1, 2, 3, 121 };
		var actual = nodes.ToItems().ToArray();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void TraverseSignal_WithSkipAndEnd_YieldsWantedNode()
	{
		// Let's implement a search for a single node.
		var expected = 12;
		var nodes = Traverse.Graph(IntRoot, (node, signal) =>
		{
			signal.Next(node.Children);

			// We want to stop traversal once we find the item we want
			// and to skip every other item.
			if (node.Item == expected)
				signal.End();
			else
				signal.Skip();
		});

		var actual = nodes.ToItems().SingleOrDefaultMany();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void TraverseNext_OnCircularGraph_BreaksOnVisited()
	{
		var last = Nodes.Create(4);
		var first = Nodes.Create(1).Attach(
			Nodes.Create(2).Attach(
				Nodes.Create(3).Attach(
					last)));

		// Make it circular!
		last.Attach(first);

		var nodes = Traverse.Graph(first, node => node.Children, true);

		var expected = new[] { 1, 2, 3, 4 };
		var actual = nodes.ToItems().ToArray();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void TraverseSignal_OnCircularGraph_BreaksOnVisited()
	{
		var last = Nodes.Create(4);
		var first = Nodes.Create(1).Attach(
			Nodes.Create(2).Attach(
				Nodes.Create(3).Attach(
					last)));

		// Make it circular!
		last.Attach(first);

		var nodes = Traverse.Graph(first, (node, signal) => signal.Next(node.Children), true);

		var expected = new[] { 1, 2, 3, 4 };
		var actual = nodes.ToItems().ToArray();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void TraverseSignal_BreadthFirst_ProvidesCorrectDepth()
	{
		var traversed = new List<(string item, int depth)>();
		Traverse.Graph(StrRoots, (node, signal) =>
		{
			signal.Next(node.Children);

			traversed.Add((node.Item, signal.Depth));
		}, type: TraversalType.BreadthFirst).EnumerateAll();

		// Since the items are set up using a nomenclature such that its depth is the item.Length-1, assert it!
		Assert.All(traversed, t => Assert.Equal(t.item.Length - 1, t.depth));
	}

	[Fact]
	public void TraverseSignal_DepthFirst_ProvidesCorrectDepth()
	{
		var traversed = new List<(string item, int depth)>();
		Traverse.Graph(StrRoots, (node, signal) =>
		{
			signal.Next(node.Children);

			traversed.Add((node.Item, signal.Depth));
		}, type: TraversalType.DepthFirst).EnumerateAll();

		// Since the items are set up using a nomenclature such that its depth is the item.Length-1, assert it!
		Assert.All(traversed, t => Assert.Equal(t.item.Length - 1, t.depth));
	}
}