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

	/// <summary>
	/// Depth tracking depends on the <see cref="TraversalType"/>.
	/// </summary>
	[MemberNotNullWhen(true, nameof(BranchCount))]
	private bool IsDepthFirst { get; }

	/// <summary>
	/// Future nodes to process. Will be a stack or queue depending on the <see cref="TraversalType"/>.
	/// </summary>
	private readonly ILinear<TNode> Nodes;

	/// <summary>
	/// For depth tracking with <see cref="TraversalType.DepthFirst"/>.
	/// </summary>
	private readonly Stack<int>? BranchCount;

	/// <summary>
	/// For depth tracking with <see cref="TraversalType.BreadthFirst"/>.
	/// </summary>
	private int DepthCount = 0;

	/// <summary>
	/// Did the user signal to skip the current node?
	/// </summary>
	private bool Skipped;

	internal GraphSignal(IEnumerable<TNode> roots, bool detectCycles = false, TraversalType type = TraversalType.BreadthFirst)
	{
		if (detectCycles)
			Visited = new HashSet<int>();

		IsDepthFirst = type == TraversalType.DepthFirst;

		Nodes = IsDepthFirst
			? new LinearStack<TNode>()
			: new LinearQueue<TNode>();

		DepthCount = Nodes.Attach(roots);

		if (IsDepthFirst)
		{
			BranchCount = new();
			BranchCount.Push(DepthCount);
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
		if (Nodes.TryDetach(out node))
		{
			if (IsDepthFirst)
			{
				Depth = BranchCount.Count - 1;
				BranchCount.Push(BranchCount.Pop() - 1);
			}
			else
			{
				if (DepthCount-- == 0)
				{
					Depth++;
					DepthCount = Nodes.Count;
				}
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
		return !Skipped;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void Cleanup()
	{
		if (IsDepthFirst)
		{
			while (BranchCount.TryPeek(out var count) && count == 0)
				BranchCount.Pop();
		}

		if (!Skipped)
			Count++;
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
	public void Next(params TNode[] nodes)
	{
		Next(nodes.AsEnumerable());
	}

	/// <summary>
	/// Call this when traversal should continue to a sub sequence of child roots.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Next(IEnumerable<TNode> nodes)
	{
		var count = Nodes.Attach(nodes);

		if (IsDepthFirst && count > 0)
			BranchCount.Push(count);
	}

	/// <summary>
	/// Call this when all traversal should end immediately.
	/// <para>Ending traversal of a particular branch is controlled by not calling
	/// <see cref="Next"/> for that branch.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void End() => Nodes.Clear();
}
