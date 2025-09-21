namespace Loken.Hierarchies.Traversal;

/// <summary>
/// Describes how to traverse a graph when visiting a node using a <see cref="GraphSignal{TNode}"/>.
/// </summary>
/// <typeparam name="TNode">The type of node.</typeparam>
/// <param name="node">The source node.</param>
/// <param name="signal">Use this to signal how to traverse.</param>
public delegate void TraverseNode<TNode>(TNode node, IGraphSignal<TNode> signal);

/// <summary>
/// Provides traversal for sequences, trees and graphs.
/// </summary>
public static partial class Traverse
{
	/// <summary>
	/// Lazily traverses a graph from a single <paramref name="root"/> using a <c>next</c> delegate.
	/// </summary>
	/// <typeparam name="TNode">The node type.</typeparam>
	/// <param name="root">Starting node; if <c>null</c> yields none.</param>
	/// <param name="next">Returns children for a node or <c>null</c>.</param>
	/// <param name="descend">Options for controlling how we descend the graph.</param>
	/// <returns>Nodes in visitation order.</returns>
	public static IEnumerable<TNode> Graph<TNode>(TNode? root, Func<TNode, IEnumerable<TNode>?> next, Descend? descend = null)
	{
		if (root is null)
			yield break;
		var opts = Descend.Normalize(descend, includeSelfDefault: true);
		var reverse = opts.Siblings == SiblingOrder.Reverse;
		HashSet<TNode>? visited = null;

		if (opts.DetectCycles)
		{
			visited = typeof(TNode).IsValueType
				? new HashSet<TNode>()
				: new HashSet<TNode>(ReferenceEqualityComparer<TNode>.Instance);
		}

		ILinear<TNode> store = opts.Type == TraversalType.DepthFirst
			? new LinearStack<TNode>()
			: new LinearQueue<TNode>();

		store.Attach(root);

		while (store.TryDetach(out var node))
		{
			if (visited is null || visited.Add(node))
			{
				yield return node;

				store.AttachManySpecific(next(node), reverse);
			}
		}
	}

	/// <summary>
	/// Lazily traverses a graph from multiple <paramref name="roots"/> using a <c>next</c> delegate.
	/// </summary>
	/// <typeparam name="TNode">The node type.</typeparam>
	/// <param name="roots">Seed nodes; order influences initial visitation.</param>
	/// <param name="next">Returns children for a node or <c>null</c>.</param>
	/// <param name="descend">Options for controlling how we descend the graph.</param>
	/// <returns>Nodes in visitation order.</returns>
	public static IEnumerable<TNode> Graph<TNode>(IEnumerable<TNode> roots, Func<TNode, IEnumerable<TNode>?> next, Descend? descend = null)
	{
		var opts = Descend.Normalize(descend, includeSelfDefault: true);
		var reverse = opts.Siblings == SiblingOrder.Reverse;
		HashSet<TNode>? visited = null;

		if (opts.DetectCycles)
		{
			visited = typeof(TNode).IsValueType
				? new HashSet<TNode>()
				: new HashSet<TNode>(ReferenceEqualityComparer<TNode>.Instance);
		}

		ILinear<TNode> store = opts.Type == TraversalType.DepthFirst
			? new LinearStack<TNode>()
			: new LinearQueue<TNode>();

		store.AttachManySpecific(roots, reverse);

		while (store.TryDetach(out var node))
		{
			if (visited is null || visited.Add(node))
			{
				yield return node;

				store.AttachManySpecific(next(node), reverse);
			}
		}
	}

	/// <summary>
	/// Lazily traverses a graph from a single <paramref name="root"/> using a signal-driven <paramref name="traverse"/> delegate.
	/// </summary>
	/// <typeparam name="TNode">The node type.</typeparam>
	/// <param name="root">Starting node; if <c>null</c> yields none.</param>
	/// <param name="traverse">Delegate invoked per node; use the signal to enqueue children, prune, skip or stop.</param>
	/// <param name="descend">Options for controlling how we descend the graph.</param>
	/// <returns>Nodes in visitation order (excluding skipped).</returns>
	public static IEnumerable<TNode> Graph<TNode>(TNode? root, TraverseNode<TNode> traverse, Descend? descend = null)
	{
		if (root is null)
			yield break;

		var opts = Descend.Normalize(descend, includeSelfDefault: true);
		GraphSignal<TNode> signal;
		if (opts.IncludeSelf == false)
		{
			var seeding = new GraphSignalSeeding<TNode>();
			traverse(root, seeding);
			signal = new GraphSignal<TNode>(seeding.Seeds, opts.DetectCycles, opts.Type, opts.Siblings == SiblingOrder.Reverse);
		}
		else
		{
			signal = new GraphSignal<TNode>(root.ToEnumerable(), opts.DetectCycles, opts.Type, opts.Siblings == SiblingOrder.Reverse);
		}

		while (signal.TryGetNext(out TNode? current))
		{
			traverse(current, signal);

			if (signal.ShouldYield())
				yield return current;

			signal.Cleanup();
		}
	}

	/// <summary>
	/// Lazily traverses a graph from multiple <paramref name="roots"/> using a signal-driven <paramref name="traverse"/> delegate.
	/// </summary>
	/// <typeparam name="TNode">The node type.</typeparam>
	/// <param name="roots">Seed nodes; order influences initial visitation.</param>
	/// <param name="traverse">Delegate invoked per node; use the signal to enqueue children, prune, skip or stop.</param>
	/// <param name="descend">Options for controlling how we descend the graph.</param>
	/// <returns>Nodes in visitation order (excluding skipped).</returns>
	public static IEnumerable<TNode> Graph<TNode>(IEnumerable<TNode> roots, TraverseNode<TNode> traverse, Descend? descend = null)
	{
		var opts = Descend.Normalize(descend, includeSelfDefault: true);
		GraphSignal<TNode> signal;
		if (opts.IncludeSelf == false)
		{
			var seeding = new GraphSignalSeeding<TNode>();
			var rootList = roots as ICollection<TNode> ?? roots.ToList();
			foreach (var r in rootList)
				traverse(r, seeding);
			signal = new GraphSignal<TNode>(seeding.Seeds, opts.DetectCycles, opts.Type, opts.Siblings == SiblingOrder.Reverse);
		}
		else
		{
			signal = new GraphSignal<TNode>(roots, opts.DetectCycles, opts.Type, opts.Siblings == SiblingOrder.Reverse);
		}

		while (signal.TryGetNext(out TNode? current))
		{
			traverse(current, signal);

			if (signal.ShouldYield())
				yield return current;

			signal.Cleanup();
		}
	}
}
