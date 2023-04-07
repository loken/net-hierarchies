namespace Loken.Hierarchies.Traversal;

/// <summary>
/// Provides traversal for sequences, trees and graphs.
/// </summary>
public static partial class Traverse
{
	/// <summary>
	/// Traverse a sequence of elements.
	/// </summary>
	/// <typeparam name="TEl">The type of element.</typeparam>
	/// <param name="element">The starting element.</param>
	/// <param name="next">Describes what the next element of the sequence should be, if any.</param>
	/// <returns>An enumeration of elements.</returns>
	public static IEnumerable<TEl> Sequence<TEl>(TEl? element, NextElement<TEl> next)
		where TEl : notnull
	{
		return Sequence(element, (el, signal) => signal.Next(next(el)));
	}

	/// <summary>
	/// Traverse a sequence of elements.
	/// <para>Use the <see cref="SequenceSignal{TEl}"/> to provide a next element
	/// and whether to include or skip the current element.</para>
	/// <para>By providing no next element you end the sequence.</para>
	/// </summary>
	/// <typeparam name="TEl">The type of element.</typeparam>
	/// <param name="element">The starting element.</param>
	/// <param name="traverse">The traversal action where you detail what's next and what to skip.</param>
	/// <returns>An enumeration of elements.</returns>
	public static IEnumerable<TEl> Sequence<TEl>(TEl? element, TraverseElement<TEl> traverse)
		where TEl : notnull
	{
		if (element is null)
			yield break;

		var signal = new SequenceSignal<TEl>(element);

		while (signal.TryGetNext(out TEl? current))
		{
			traverse(current, signal);

			if (signal.ShouldYield())
				yield return current;
		}
	}
}
