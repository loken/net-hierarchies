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
	/// Visited set for detecting cycles.
	/// Stores nodes directly to avoid false positives from hash collisions
	/// and to align with TS implementation semantics.
	/// </summary>
	private readonly ISet<TNode>? Visited;

	/// <summary>
	/// Depth tracking depends on the <see cref="TraversalType"/>.
	/// </summary>
	[MemberNotNullWhen(true, nameof(BranchCounts))]
	private bool IsDepthFirst { get; }

	/// <summary>
	/// Future nodes to process. Will be a stack or queue depending on the <see cref="TraversalType"/>.
	/// </summary>
	private readonly ILinear<TNode> Nodes;

	/// <summary>
	/// For depth tracking with <see cref="TraversalType.DepthFirst"/>.
	/// Lightweight array-backed stack to avoid Stack<T> Push/Pop/TryPeek overhead in the hot path.
	/// Rationale:
	/// - Stack<T> is optimized, but each Push/Pop/TryPeek introduces a call boundary and range/version checks.
	/// - In the signal traversal we do this per visited node; replacing with index ops reduces branches and indirections.
	/// - We keep the same semantics: Depth is the current top index; counts track pending siblings per depth.
	/// - Also avoids one managed object (Stack<T>) and its internal indirections.
	/// </summary>
	private int[]? BranchCounts;
	private int BranchTop = -1;

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
		{
			Visited = typeof(TNode).IsValueType
				? new HashSet<TNode>()
				: new HashSet<TNode>(ReferenceEqualityComparer<TNode>.Instance);
		}

		IsDepthFirst = type == TraversalType.DepthFirst;

		Nodes = IsDepthFirst
			? new LinearStack<TNode>()
			: new LinearQueue<TNode>();

		DepthCount = Nodes.Attach(roots);

		if (IsDepthFirst)
			BranchPush(DepthCount);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool TryGetNext([MaybeNullWhen(false)] out TNode node)
	{
		if (Visited is not null)
		{
			while (TryGetNextInternal(out node))
			{
				if (Visited.Add(node))
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
				Depth = BranchTop;
				BranchCounts![BranchTop]--;
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
			// Pop empty branch frames by moving the top index left; avoids Stack<T>.TryPeek + Pop loop.
			while (BranchTop >= 0 && BranchCounts![BranchTop] == 0)
				BranchTop--;
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
		// Fast-path for arrays: call the most specific Attach overload available.
		var count = Nodes.Attach(nodes);
		if (IsDepthFirst && count > 0)
			BranchPush(count);
	}

	/// <summary>
	/// Call this when traversal should continue to a sub sequence of child roots.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Next(IEnumerable<TNode> nodes)
	{
		// Prefer array/list overloads when available to avoid enumerator overhead.
		int count = nodes is TNode[] arr
			? Nodes.Attach(arr)
			: nodes is List<TNode> list
				? Nodes.Attach(list)
				: Nodes.Attach(nodes);

		if (IsDepthFirst && count > 0)
			BranchPush(count);
	}

	/// <summary>
	/// Call this when all traversal should end immediately.
	/// <para>Ending traversal of a particular branch is controlled by not calling
	/// <see cref="Next"/> for that branch.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void End() => Nodes.Clear();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void BranchPush(int count)
	{
		// Push a new depth frame with the number of pending siblings.
		// Uses a simple doubling growth strategy to minimize allocations in long traversals.
		var arr = BranchCounts;
		if (arr is null)
		{
			BranchCounts = arr = new int[4];
		}

		int nextTop = BranchTop + 1;
		if ((uint)nextTop >= (uint)arr.Length)
		{
			// Grow by doubling; faster than Stack<T> path due to fewer checks and no version updates.
			var newArr = new int[arr.Length << 1];
			Array.Copy(arr, newArr, arr.Length);
			arr = BranchCounts = newArr;
		}

		arr[nextTop] = count;
		BranchTop = nextTop;
	}
}
