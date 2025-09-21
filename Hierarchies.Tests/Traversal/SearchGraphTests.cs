namespace Loken.Hierarchies.Traversal;

public class SearchGraphTests
{
	protected static Node<int> IntRoot { get; } =
		Nodes.Create(0).Attach(
			Nodes.Create(1).Attach(
				Nodes.Create(11),
				Nodes.Create(12).Attach(Nodes.Create(121))),
			Nodes.Create(2),
			Nodes.Create(3).Attach(Nodes.CreateMany(31, 32)));

	[Fact]
	public void SearchNext_BreadthFirst_FindsFirstLeaf()
	{
		var match = Search.Graph(IntRoot, n => n.Children, n => n.IsLeaf, TraversalType.BreadthFirst);

		Assert.NotNull(match);
		Assert.Equal(2, match!.Item);
	}

	[Fact]
	public void SearchNext_DepthFirst_FindsFirstLeaf()
	{
		var match = Search.Graph(IntRoot, n => n.Children, n => n.IsLeaf, TraversalType.DepthFirst);

		Assert.NotNull(match);
		Assert.Equal(32, match!.Item);
	}

	[Fact]
	public void SearchManyNext_BreadthFirst_FindsAllLeavesInOrder()
	{
		var matches = Search.GraphMany(IntRoot, n => n.Children, n => n.IsLeaf, TraversalType.BreadthFirst);
		var actual = matches.ToItems();

		var expected = new[] { 2, 11, 31, 32, 121 };
		Assert.Equal(expected, actual);
	}

	[Fact]
	public void SearchManyNext_DepthFirst_FindsAllLeavesInOrder()
	{
		var matches = Search.GraphMany(IntRoot, n => n.Children, n => n.IsLeaf, TraversalType.DepthFirst);
		var actual = matches.ToItems();

		var expected = new[] { 32, 31, 2, 121, 11 };
		Assert.Equal(expected, actual);
	}

	[Fact]
	public void SearchNext_OnCircularGraph_DetectCycles_FindsTarget()
	{
		var last = Nodes.Create(4);
		var first = Nodes.Create(1).Attach(
			Nodes.Create(2).Attach(
				Nodes.Create(3).Attach(
					last)));

		// Make it circular!
		last.Attach(first);

		var match = Search.Graph(first, n => n.Children, n => n.Item == 4, new(DetectCycles: true));

		Assert.NotNull(match);
		Assert.Equal(4, match!.Item);
	}

	[Fact]
	public void SearchManyNext_OnCircularGraph_DetectCycles_ReturnsAll()
	{
		var last = Nodes.Create(4);
		var first = Nodes.Create(1).Attach(
			Nodes.Create(2).Attach(
				Nodes.Create(3).Attach(
					last)));

		// Make it circular!
		last.Attach(first);

		var matches = Search.GraphMany(first, n => n.Children, _ => true, new(DetectCycles: true));
		var actual = matches.ToItems();

		var expected = new[] { 1, 2, 3, 4 };
		Assert.Equal(expected, actual);
	}

	[Fact]
	public void SearchNext_IncludeSelfFalse_SkipsRootMatch()
	{
		var match = Search.Graph(IntRoot, n => n.Children, n => n.Item == 0, Descend.WithoutSelf);

		Assert.Null(match);
	}

	[Fact]
	public void SearchNext_IncludeSelfFalse_FindsDescendant()
	{
		var match = Search.Graph(IntRoot, n => n.Children, n => n.Item == 2, Descend.WithoutSelf);

		Assert.NotNull(match);
		Assert.Equal(2, match!.Item);
	}

	[Fact]
	public void SearchManyNext_IncludeSelfFalse_ExcludesRoot()
	{
		var matches = Search.GraphMany(IntRoot, n => n.Children, _ => true, Descend.WithoutSelf);
		var actual = matches.ToItems();

		var expected = new[] { 1, 2, 3, 11, 12, 31, 32, 121 };
		Assert.Equal(expected, actual);
		Assert.DoesNotContain(0, actual);
	}

	[Fact]
	public void SearchManyNext_Reverse_SiblingOrder()
	{
		var matches = Search.GraphMany(IntRoot, n => n.Children, _ => true, new(Siblings: SiblingOrder.Reverse));
		var actual = matches.ToItems();

		var expected = new[] { 0, 3, 2, 1, 32, 31, 12, 11, 121 };
		Assert.Equal(expected, actual);
	}

	[Fact]
	public void SearchManyNext_Reverse_IncludeSelfFalse_SiblingOrder()
	{
		var matches = Search.GraphMany(IntRoot, n => n.Children, _ => true, new(IncludeSelf: false, Siblings: SiblingOrder.Reverse));
		var actual = matches.ToItems();

		var expected = new[] { 3, 2, 1, 32, 31, 12, 11, 121 };
		Assert.Equal(expected, actual);
		Assert.DoesNotContain(0, actual);
	}
}
