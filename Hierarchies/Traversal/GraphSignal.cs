using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Loken.Hierarchies.Traversal;

/// <summary>
/// Use this to signal to the traversal what's <see cref="Next"/>,
/// what to <see cref="Skip"/> and whether to <see cref="End"/>.
/// </summary>
public sealed class GraphSignal<TNode>
	where TNode : notnull
{
	/// <summary>
	/// We use a set of hash codes instead of a set of roots for detecting cycles.
	/// This way we consume less memory. If we were to keep the roots in the visited set
	/// the GC could not clean up any enumerated roots until we are done traversing the graph.
	/// </summary>
	private readonly ISet<int>? Visited;

	private readonly Queue<TNode> Queue = new();
	private int DepthCount = 0;
	private bool Skipped;

	internal GraphSignal(IEnumerable<TNode> roots, bool detectCycles = false)
	{
		if (detectCycles)
			Visited = new HashSet<int>();

		foreach (var root in roots)
		{
			Queue.Enqueue(root);
			DepthCount++;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool TryGetNext([MaybeNullWhen(false)] out TNode node)
	{
		if (Visited is not null)
		{
			while (TryGetNextInternal(out node))
			{
				if (Visited.Add(node.GetHashCode()))
					return true;
			}

			return false;
		}
		else
		{
			return TryGetNextInternal(out node);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool TryGetNextInternal([MaybeNullWhen(false)] out TNode node)
	{
		if (Queue.TryDequeue(out node))
		{
			if (DepthCount-- == 0)
			{
				Depth++;
				DepthCount = Queue.Count;
			}

			Skipped = false;
			return true;
		}
		else
		{
			node = default;
			return false;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool ShouldYield()
	{
		if (Skipped)
			return false;
		else
		{
			Count++;
			return true;
		}
	}

	/// <summary>
	/// Depth of the current root relative to the traversal roots.
	/// </summary>
	public int Depth { get; private set; } = 0;

	/// <summary>
	/// The number of elements returned so far.
	/// </summary>
	public int Count { get; private set; } = 0;

	/// <summary>
	/// Call this when you want to signal that the current root should be skipped,
	/// meaning it will not be part of the output.
	/// <para>Traversal will still continue to whatever roots are passed to
	/// <see cref="Next"/> irrespective of calling <see cref="Skip"/>.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Skip() => Skipped = true;

	/// <summary>
	/// Call this when traversal should continue to a sub sequence of child roots.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Next(params TNode[] nodes) => Queue.Enqueue(nodes);

	/// <summary>
	/// Call this when traversal should continue to a sub sequence of child roots.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Next(IEnumerable<TNode> nodes) => Queue.Enqueue(nodes);

	/// <summary>
	/// Call this when all traversal should end immediately.
	/// <para>Ending traversal of a particular branch is controlled by not calling
	/// <see cref="Next"/> for that branch.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void End() => Queue.Clear();
}
