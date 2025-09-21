namespace Loken.Hierarchies.Traversal;

public class SearchSequenceTests
{
	[Fact]
	public void Sequence_FindsFirstMatchingElement()
	{
		var list = new LinkedList<int>(Enumerable.Range(0, 5));

		var found = Search.Sequence(list.First, el => el!.Next, el => el.Value == 3);

		Assert.NotNull(found);
		Assert.Equal(3, found!.Value);
	}

	[Fact]
	public void SequenceMany_FindsAllMatchingElements()
	{
		var expected = new[] { 0, 2, 4 };
		var list = new LinkedList<int>(Enumerable.Range(0, 5));

		var found = Search.SequenceMany(list.First, el => el!.Next, el => el.Value % 2 == 0);
		var actual = found.Select(n => n.Value).ToArray();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void Sequence_WithEmptySequence_ReturnsNull()
	{
		var found = Search.Sequence<LinkedListNode<int>>(null, el => el!.Next, _ => true);

		Assert.Null(found);
	}

	[Fact]
	public void SequenceMany_WithEmptySequence_ReturnsEmpty()
	{
		var found = Search.SequenceMany<LinkedListNode<int>>(null, el => el!.Next, _ => true);

		Assert.Empty(found);
	}
}
