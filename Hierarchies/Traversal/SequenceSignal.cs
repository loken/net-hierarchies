using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Loken.Hierarchies.Traversal;

/// <summary>
/// Use this to signal to the traversal what's <see cref="Next"/> and what to <see cref="Skip"/>.
/// </summary>
public sealed class SequenceSignal<TEl>
{
	private TEl? NextEl;
	private bool Skipped;
	private bool Yielded;
	private bool Pruned;
	private bool NextSet;

	internal SequenceSignal(TEl element)
	{
		NextEl = element;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool TryGetNext([MaybeNullWhen(false)] out TEl element)
	{
		if (NextEl is not null)
		{
			element = NextEl;

			Skipped = false;
			Yielded = false;
			Pruned = false;
			NextSet = false;
			NextEl = default;
			Index++;

			return true;
		}
		else
		{
			element = default;
			return false;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool ShouldYield()
	{
		return !Skipped;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void Cleanup()
	{
		if (!Skipped)
			Count++;
	}

	/// <summary>
	/// The number of elements returned so far.
	/// </summary>
	public int Count { get; private set; } = 0;

	/// <summary>
	/// The source index of the current element.
	/// </summary>
	public int Index { get; private set; } = -1;

	/// <summary>
	/// Call this when you want to signal that the current element should be skipped,
	/// meaning it will not be part of the output.
	/// <para>Traversal will still continue to an element passed to
	/// <see cref="Next"/> irrespective of calling <see cref="Skip"/>.</para>
	/// <para>Mutually exclusive with <see cref="Yield"/> for the same element; attempting to call both will throw.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Skip()
	{
		if (Yielded)
			throw new InvalidOperationException($"Cannot call {nameof(Skip)}() after {nameof(Yield)}(). {nameof(Yield)} and {nameof(Skip)} are mutually exclusive for the same element.");

		Skipped = true;
	}

	/// <summary>
	/// Explicitly mark that the current element should be yielded (included in the output).
	/// <para>By default, elements are yielded unless <see cref="Skip"/> is called; use this for clarity in complex callbacks.</para>
	/// <para>Mutually exclusive with <see cref="Skip"/> for the same element; attempting to call both will throw.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Yield()
	{
		if (Skipped)
			throw new InvalidOperationException($"Cannot call {nameof(Yield)}() after {nameof(Skip)}(). {nameof(Yield)} and {nameof(Skip)} are mutually exclusive for the same element.");

		Yielded = true;
	}

	/// <summary>
	/// Call this when traversal should continue to the next element.
	/// <para>Mutually exclusive with <see cref="Prune"/> for the same element; attempting to call both will throw.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Next(TEl? element)
	{
		if (Pruned)
			throw new InvalidOperationException($"Cannot call {nameof(Next)}() after {nameof(Prune)}(). {nameof(Prune)} and {nameof(Next)} are mutually exclusive for the same element.");

		NextEl = element;
		NextSet = element is not null;
	}

	/// <summary>
	/// Prune the sequence by not traversing to a next element for this iteration.
	/// <para>Functionally equivalent to not calling <see cref="Next(TEl)"/>.</para>
	/// <para>Mutually exclusive with <see cref="Next(TEl)"/> for the same element; attempting to call both will throw.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Prune()
	{
		if (NextSet)
			throw new InvalidOperationException($"Cannot call {nameof(Prune)}() after {nameof(Next)}(). {nameof(Prune)} and {nameof(Next)} are mutually exclusive for the same element.");

		Pruned = true;
	}
}
