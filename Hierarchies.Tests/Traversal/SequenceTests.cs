namespace Loken.Hierarchies.Traversal;

public class SequenceTests
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
}
