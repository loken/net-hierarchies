namespace Loken.Hierarchies.Traversal;

/// <summary>
/// Describes how to get the next element of the sequence.
/// </summary>
/// <typeparam name="TEl">The type of element.</typeparam>
/// <param name="element">The source element.</param>
/// <returns>The next element of the sequence, or null if there are no more elements.</returns>
public delegate TEl? NextElement<TEl>(TEl element)
	where TEl : notnull;

/// <summary>
/// Describes how to traverse a sequence when visiting an element using a <see cref="SequenceSignal{TEl}"/>.
/// </summary>
/// <typeparam name="TEl">The type of element.</typeparam>
/// <param name="node">The source element.</param>
/// <param name="signal">Use this to signal how to traverse.</param>
public delegate void TraverseElement<TEl>(TEl element, SequenceSignal<TEl> signal)
	where TEl : notnull;
