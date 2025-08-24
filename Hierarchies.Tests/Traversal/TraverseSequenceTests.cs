namespace Loken.Hierarchies.Traversal;

public class TraverseSequenceTests
{
	[Fact]
	public void SequenceNext_YieldsInOrder()
	{
		var expected = Enumerable.Range(0, 5).ToArray();
		var list = new LinkedList<int>(expected);

		var elements = Traverse.Sequence(list.First!, el => el.Next);

		var actual = elements.Select(el => el.Value).ToArray();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void SequenceSignal_WithSkip_YieldsCorrectValues()
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
	public void SequenceSignal_WithSkip_ProvidesCorrectIndexAndCount()
	{
		// We set it up so that index and value matches
		// and so that we exclude odd values from the result.
		var list = new LinkedList<int>([0, 1, 2, 3, 4]);
		var expected = new[] { 0, 2, 4 };
		var expectedCounts = new[] { 0, 1, 1, 2, 2 };

		var actual = Traverse.Sequence(list.First, (el, signal) =>
		{
			var expectedCount = expectedCounts[signal.Index];
			Assert.Equal(expectedCount, signal.Count);
			Assert.Equal(el.Value, signal.Index);

			if (signal.Index % 2 == 1)
				signal.Skip();

			signal.Next(el.Next);
		}).Select(el => el.Value).ToArray();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void SequenceNext_WithEmptySequence_YieldsNothing()
	{
		var elements = Traverse.Sequence<LinkedListNode<int>>(null, el => el.Next);

		var actual = elements.ToArray();

		Assert.Empty(actual);
	}

	[Fact]
	public void SequenceNext_WithSingleElement_YieldsOneItem()
	{
		var list = new LinkedList<int>([42]);

		var elements = Traverse.Sequence(list.First, el => el.Next);

		var expected = new[] { 42 };
		var actual = elements.Select(el => el.Value).ToArray();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void SequenceSignal_WithEarlyTermination_YieldsWantedElement()
	{
		// Let's implement a search for a single element by not providing next elements after we find it.
		var list = new LinkedList<int>(Enumerable.Range(0, 10));

		var elements = Traverse.Sequence(list.First, (el, signal) =>
		{
			// We want to stop traversal once we find the element we want
			// and to skip every other element.
			if (el.Value == 3)
			{
				// Don't provide next element to stop traversal
				return;
			}
			else
			{
				signal.Skip();
			}

			// Signal the next element unless we're done.
			signal.Next(el.Next);
		});

		var expected = new[] { 3 };
		var actual = elements.Select(el => el.Value).ToArray();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void SequenceSignal_WithNullFirst_YieldsNothing()
	{
		var elements = Traverse.Sequence<LinkedListNode<int>>(null, (el, signal) =>
		{
			// This should never be called
			Assert.Fail("Signal function should not be called with null first element");
		});

		var actual = elements.ToArray();

		Assert.Empty(actual);
	}

	[Fact]
	public void SequenceSignal_Throws_WhenCallingYieldThenSkip()
	{
		var list = new LinkedList<int>([1]);
		var ex = Assert.Throws<InvalidOperationException>(() =>
		{
			Traverse.Sequence(list.First!, (el, signal) =>
			{
				signal.Yield();
				signal.Skip();
			}).EnumerateAll();
		});

		Assert.Contains("Yield and Skip are mutually exclusive", ex.Message);
	}

	[Fact]
	public void SequenceSignal_Throws_WhenCallingPruneThenNext()
	{
		var list = new LinkedList<int>([1, 2]);
		var ex = Assert.Throws<InvalidOperationException>(() =>
		{
			Traverse.Sequence(list.First!, (el, signal) =>
			{
				signal.Prune();
				signal.Next(el.Next);
			}).EnumerateAll();
		});

		Assert.Contains("Prune and Next are mutually exclusive", ex.Message);
	}
}
