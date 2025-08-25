using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Loken.Hierarchies.Traversal;

/// <summary>
/// Use this to signal to the traversal what's <see cref="Next"/>,
/// what to <see cref="Skip"/> and whether to <see cref="Stop"/>.
/// </summary>
public sealed class GraphSignal<TNode>
{
	/// <summary>
	/// Visited set for detecting cycles.
	/// Stores nodes directly to avoid false positives from hash collisions
	/// and to align with TS implementation semantics.
	/// </summary>
	private readonly HashSet<TNode>? Visited;

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

	/// <summary>
	/// Did the user explicitly request yielding the current node?
	/// Default behavior is to yield unless <see cref="Skip"/> is called; this flag is only used for guard-rails.
	/// </summary>
	private bool Yielded;

	/// <summary>
	/// Did the user explicitly request pruning the children of the current node?
	/// Functionally equivalent to not calling <see cref="Next(TNode[])"/> or <see cref="Next(IEnumerable{TNode})"/>,
	/// but tracked to enforce exclusivity with <see cref="Next(TNode[])"/>/ <see cref="Next(IEnumerable{TNode})"/>.
	/// </summary>
	private bool Pruned;

	/// <summary>
	/// Did the user call any <see cref="Next(TNode[])"/> or <see cref="Next(IEnumerable{TNode})"/> overload for this node?
	/// Used to enforce exclusivity with <see cref="Prune"/>.
	/// </summary>
	private bool NextSet;

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

		if (roots is IList<TNode> rootList)
			Nodes.AttachMany(rootList);
		else if (roots is ICollection<TNode> rootCollection)
			Nodes.AttachMany(rootCollection);
		else
			Nodes.AttachMany(roots);

		DepthCount = Nodes.Count;

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
			Yielded = false;
			Pruned = false;
			NextSet = false;
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
	/// <para>Mutually exclusive with <see cref="Yield"/> for the same node; attempting to call both will throw.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Skip()
	{
		if (Yielded)
			throw new InvalidOperationException($"Cannot call {nameof(Skip)}() after {nameof(Yield)}(). {nameof(Yield)} and {nameof(Skip)} are mutually exclusive for the same node.");

		Skipped = true;
	}

	/// <summary>
	/// Explicitly mark that the current node should be yielded (included in the output).
	/// <para>By default, nodes are yielded unless <see cref="Skip"/> is called; use this for clarity in complex callbacks.</para>
	/// <para>Mutually exclusive with <see cref="Skip"/> for the same node; attempting to call both will throw.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Yield()
	{
		if (Skipped)
			throw new InvalidOperationException($"Cannot call {nameof(Yield)}() after {nameof(Skip)}(). {nameof(Yield)} and {nameof(Skip)} are mutually exclusive for the same node.");

		// Idempotent for ergonomics.
		Yielded = true;
	}

	/// <summary>
	/// Call this when traversal should continue to a sub sequence of child roots.
	/// <para>Mutually exclusive with <see cref="Prune"/> for the same node; attempting to call both will throw.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Next(params TNode[] nodes)
	{
		if (Pruned)
			throw new InvalidOperationException($"Cannot call {nameof(Next)}() after {nameof(Prune)}(). {nameof(Prune)} and {nameof(Next)} are mutually exclusive for the same node.");

		// Fast-path for arrays: call the most specific Attach overload available.
		Nodes.AttachMany(nodes);
		if (IsDepthFirst && nodes.Length > 0)
			BranchPush(nodes.Length);

		NextSet = true;
	}

	/// <summary>
	/// Call this when traversal should continue to a sub sequence of child roots.
	/// <para>Mutually exclusive with <see cref="Prune"/> for the same node; attempting to call both will throw.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Next(IEnumerable<TNode> nodes)
	{
		if (Pruned)
			throw new InvalidOperationException($"Cannot call {nameof(Next)}() after {nameof(Prune)}(). {nameof(Prune)} and {nameof(Next)} are mutually exclusive for the same node.");

		var count = Nodes.Count;

		// Prefer list/collection overloads when available to avoid enumerator overhead.
		if (nodes is IList<TNode> rootList)
			Nodes.AttachMany(rootList);
		else if (nodes is ICollection<TNode> rootCollection)
			Nodes.AttachMany(rootCollection);
		else
			Nodes.AttachMany(nodes);

		var added = Nodes.Count - count;

		if (IsDepthFirst && added > 0)
			BranchPush(added);

		NextSet = true;
	}

	/// <summary>
	/// Prune the current branch by not traversing any children for this node.
	/// <para>Functionally equivalent to not calling <see cref="Next(TNode[])"/> or <see cref="Next(IEnumerable{TNode})"/>.</para>
	/// <para>Mutually exclusive with <see cref="Next(TNode[])"/> and <see cref="Next(IEnumerable{TNode})"/> for the same node; attempting to call both will throw.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Prune()
	{
		if (NextSet)
			throw new InvalidOperationException($"Cannot call {nameof(Prune)}() after {nameof(Next)}(). {nameof(Prune)} and {nameof(Next)} are mutually exclusive for the same node.");

		// Idempotent for ergonomics.
		Pruned = true;
	}

	/// <summary>
	/// Call this when all traversal should stop immediately.
	/// <para>Ending traversal of a particular branch is controlled by not calling
	/// <see cref="Next"/> for that branch.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Stop() => Nodes.Clear();

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
