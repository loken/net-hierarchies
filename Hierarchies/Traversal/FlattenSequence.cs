namespace Loken.Hierarchies.Traversal;

/// <summary>
/// Provides eager traversal helpers that return arrays for sequences, trees and graphs.
/// </summary>
public static partial class Flatten
{
	/// <summary>
	/// Flatten a sequence of elements.
	/// <typeparam name="TEl">The type of element.</typeparam>
	/// <param name="element">The starting element.</param>
	/// <param name="next">Describes what the next element of the sequence should be, if any.</param>
	/// <returns>A list of elements.</returns>
	/// </summary>
	public static IList<TEl> Sequence<TEl>(TEl? element, Func<TEl, TEl?> next)
	{
		if (element is null)
			return [];

		var result = new List<TEl>();
		var current = element;
		while (current is not null)
		{
			result.Add(current);
			current = next(current);
		}

		return result;
	}

	/// <summary>
	/// Flatten a sequence of elements.
	/// <para>Use the <see cref="SequenceSignal{TEl}"/> to provide a next element
	/// and whether to include or skip the current element.</para>
	/// <para>By providing no next element you end the sequence.</para>
	/// <typeparam name="TEl">The type of element.</typeparam>
	/// <param name="element">The starting element.</param>
	/// <param name="traverse">The traversal action where you detail what's next and what to skip.</param>
	/// <returns>A list of elements.</returns>
	/// </summary>
	public static IList<TEl> Sequence<TEl>(TEl? element, TraverseElement<TEl> traverse)
	{
		if (element is null)
			return [];

		var signal = new SequenceSignal<TEl>(element);
		var result = new List<TEl>();

		while (signal.TryGetNext(out TEl? current))
		{
			traverse(current, signal);

			if (signal.ShouldYield())
				result.Add(current);

			signal.Cleanup();
		}

		return result;
	}
}
