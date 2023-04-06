using System.Buffers;

namespace Loken.Hierarchies;

public class TraverseTests
{
	[Fact]
	public void Sequence_Simple_YieldsValuesInOrder()
	{
		var expected = Enumerable.Range(0, 5).ToArray();
		var list = new LinkedList<int>(expected);

		var nodes = Traverse.Sequence(list.First!, node => node?.Next);

		var actual = nodes.Select(n => n.Value).ToArray();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void Sequence_WithSkipAndExclusion_YieldsCorrectValues()
	{
		var root = Node.Create(1).Attach(
			Node.Create(2).Attach(
				Node.Create(3).Attach(
					Node.Create(4))));

		var nodes = Traverse.Sequence(root, (node, signal) =>
		{
			// Skip odd numbers.
			if (node.Value % 2 == 1)
				signal.Skip();

			// By not providing the child as a next value at node 3
			// we don't iterate into node 4 which would otherwise not be skipped.
			if (node.Value < 3)
				signal.Next(node.Children.SingleOrDefault());
		});

		var expected = new[] { 2 };
		var actual = nodes.Select(n => n.Value).ToArray();

		// We only want node 3 to be returned because we only want even numbers,
		// and we stop the iteration at node 3.
		Assert.Equal(expected, actual);
	}

	[Fact]
	public void Tree_BreadthFirst_YieldsCorrectOrder()
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
		var actual = nodes.Select(n => n.Value).ToArray();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void Tree_BreadthFirstWithSkipAndExclusion_YieldsCorrectOrder()
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
			// Do not include children of 3 which is 31 and 32.
			if (node.Value != 3)
				signal.Next(node.Children);

			// Skip children of 1 which is 11 and 12.
			if (node.Parent?.Value == 1)
				signal.Skip();
		});

		var expected = new[] { 0, 1, 2, 3, 121 };
		var actual = nodes.Select(n => n.Value).ToArray();

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
		var actual = nodes.Select(n => n.Value).ToArray();

		Assert.Equal(expected, actual);
	}
}