using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Loken.Hierarchies.Traversal;

/// <summary>
/// Use this to signal to the traversal what's <see cref="Next"/> and what to <see cref="Skip"/>.
/// </summary>
public sealed class SequenceSignal<TEl>
	where TEl : notnull
{
	private TEl? NextEl;
	private bool Skipped;

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
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Skip() => Skipped = true;

	/// <summary>
	/// Call this when traversal should continue to a sub sequence of child roots.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Next(TEl? element)
	{
		NextEl = element;
	}
}
