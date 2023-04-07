using System.Buffers;

namespace Loken.Hierarchies;

public class TraverseTests
{
	[Fact]
	public void Sequence_Simple_YieldsValuesInOrder()
	{
		var expected = Enumerable.Range(0, 5).ToArray();
		var list = new LinkedList<int>(expected);

		var elements = Traverse.Sequence(list.First!, el => el?.Next);

		var actual = elements.Select(el => el.Value).ToArray();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void Sequence_WithSkip_YieldsCorrectValues()
	{
		var list = new LinkedList<int>(Enumerable.Range(1, 4));

		var elements = Traverse.Sequence(list.First!, (el, signal) =>
		{
			// Skip odd numbers.
			if (el.Value % 2 == 1)
				signal.Skip();

			// By not providing the child as a next value at el 3
			// we don't iterate into el 4 which would otherwise not be skipped.
			if (el.Value < 3)
				signal.Next(el.Next);
		});

		var expected = new[] { 2 };
		var actual = elements.Select(el => el.Value).ToArray();

		// We only want el 3 to be returned because we only want even numbers,
		// and we stop the iteration at el 3.
		Assert.Equal(expected, actual);
	}

	[Fact]
	public void Sequence_WithSkip_ReportsCorrectIndexAndCount()
	{
		// We set it up so that index and value matches
		// and so that we exclude odd values from the result.
		var expectedCounts = new[] { 0, 1, 1, 2, 2 };
		var list = new LinkedList<int>(new[] { 0, 1, 2, 3, 4 });

		var elements = Traverse.Sequence(list.First!, (el, signal) =>
		{
			var expectedCount = expectedCounts[signal.Index];
			Assert.Equal(expectedCount, signal.Count);
			Assert.Equal(el.Value, signal.Index);

			if (signal.Index % 2 == 1)
				signal.Skip();

			signal.Next(el.Next);
		}).ToArray();
	}

	[Fact]
	public void Tree_Simple_YieldsCorrectOrder()
	{
		var root = Node.Create(0).Attach(
			Node.Create(1).Attach(
				Node.Create(11),
				Node.Create(12).Attach(121)),
			Node.Create(2),
			Node.Create(3).Attach(
				Node.Create(31),
				Node.Create(32)));

		var nodes = Traverse.Tree(root, node => node.Children);

		var expected = new[] { 0, 1, 2, 3, 11, 12, 31, 32, 121 };
		var actual = nodes.ToItems().ToArray();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void Tree_WithSkipAndExclusion_YieldsCorrectOrder()
	{
		var root = Node.Create(0).Attach(
			Node.Create(1).Attach(
				Node.Create(11),
				Node.Create(12).Attach(121)),
			Node.Create(2),
			Node.Create(3).Attach(
				Node.Create(31),
				Node.Create(32)));

		var nodes = Traverse.Tree(root, (node, signal) =>
		{
			// Exclude children of 3 which is 31 and 32.
			if (node.Item != 3)
				signal.Next(node.Children);

			// Skip children of 1 which is 11 and 12.
			if (node.Parent?.Item == 1)
				signal.Skip();
		});

		var expected = new[] { 0, 1, 2, 3, 121 };
		var actual = nodes.ToItems().ToArray();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void Tree_Signal_ProvidesCorrectDepth()
	{
		var root = Node.Create(0).Attach(
			Node.Create(1).Attach(
				Node.Create(11),
				Node.Create(12).Attach(121)),
			Node.Create(2),
			Node.Create(3).Attach(
				Node.Create(31),
				Node.Create(32)));

		var nodes = Traverse.Tree(root, (node, signal) =>
		{
			// Due to our value scheme the depth is equal to
			// the number of digits which we can get with a bit of math.
			var expectedDepth = node.Item == 0 ? 0 : Math.Floor(Math.Log10(node.Item) + 1);
			Assert.Equal(expectedDepth, signal.Depth);
		}).ToArray();
	}

	[Fact]
	public void Tree_SkipAndEnd_FindNode()
	{
		var root = Node.Create(0).Attach(
			Node.Create(1).Attach(
				Node.Create(11),
				Node.Create(12).Attach(121)));

		// Let's implement a search for a single node.
		int expected = 12;
		int actual = Traverse.Tree(root, (node, signal) =>
		{
			signal.Next(node.Children);

			// We want to stop traversal once we find the item we want
			// and to skip every other item.
			if (node == expected)
				signal.End();
			else
				signal.Skip();
		}).Single();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void Graph_Circular_BreaksOnVisited()
	{
		var last = Node.Create(4);
		var first = Node.Create(1).Attach(
			Node.Create(2).Attach(
				Node.Create(3).Attach(
					last)));

		// Make it circular!
		last.Attach(first);

		var nodes = Traverse.Graph(first, node => node.Children);

		var expected = new[] { 1, 2, 3, 4 };
		var actual = nodes.ToItems().ToArray();

		Assert.Equal(expected, actual);
	}
}