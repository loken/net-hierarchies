namespace Loken.Hierarchies.Traversal;

/// <summary>
/// Provides eager traversal helpers that return arrays for sequences, trees and graphs.
/// </summary>
public static partial class Flatten
{
	/// <summary>
	/// Eagerly traverses a graph from a single <paramref name="root"/> using a <c>next</c> delegate.
	/// </summary>
	/// <typeparam name="TNode">The node type.</typeparam>
	/// <param name="root">Starting node; if <c>null</c> returns empty.</param>
	/// <param name="next">Returns children for a node or <c>null</c>.</param>
	/// <param name="descend">Options for controlling how we descend the graph.</param>
	/// <returns>Nodes in visitation order.</returns>
	public static IList<TNode> Graph<TNode>(TNode? root, Func<TNode, IEnumerable<TNode>?> next, Descend? descend = null)
	{
		if (root is null)
			return [];
		var opts = Descend.Normalize(descend, includeSelfDefault: true);
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
		var result = new List<TNode>();

		while (store.TryDetach(out var node))
		{
			if (visited is null || visited.Add(node))
			{
				result.Add(node);

				var children = next(node);
				if (children is not null)
				{
					if (children is IList<TNode> childList)
						store.AttachMany(childList);
					else if (children is ICollection<TNode> childCollection)
						store.AttachMany(childCollection);
					else
						store.AttachMany(children);
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Eagerly traverses a graph from multiple <paramref name="roots"/> using a <c>next</c> delegate.
	/// </summary>
	/// <typeparam name="TNode">The node type.</typeparam>
	/// <param name="roots">Seed nodes; order influences initial visitation.</param>
	/// <param name="next">Returns children for a node or <c>null</c>.</param>
	/// <param name="descend">Options for controlling how we descend the graph.</param>
	/// <returns>Nodes in visitation order.</returns>
	public static IList<TNode> Graph<TNode>(IEnumerable<TNode> roots, Func<TNode, IEnumerable<TNode>?> next, Descend? descend = null)
	{
		var opts = Descend.Normalize(descend, includeSelfDefault: true);
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

		if (roots is IList<TNode> rootList)
			store.AttachMany(rootList);
		else if (roots is ICollection<TNode> rootCollection)
			store.AttachMany(rootCollection);
		else
			store.AttachMany(roots);

		var result = new List<TNode>();
		while (store.TryDetach(out var node))
		{
			if (visited is null || visited.Add(node))
			{
				result.Add(node);

				var children = next(node);
				if (children is not null)
				{
					if (children is IList<TNode> childList)
						store.AttachMany(childList);
					else if (children is ICollection<TNode> childCollection)
						store.AttachMany(childCollection);
					else
						store.AttachMany(children);
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Eagerly traverses a graph from a single <paramref name="root"/> using a signal-driven <paramref name="traverse"/> delegate.
	/// </summary>
	/// <typeparam name="TNode">The node type.</typeparam>
	/// <param name="root">Starting node; if <c>null</c> returns empty.</param>
	/// <param name="traverse">Delegate per node; use the signal to enqueue, prune, skip or stop.</param>
	/// <param name="descend">Options for controlling how we descend the graph.</param>
	/// <returns>Nodes in visitation order (excluding skipped).</returns>
	public static IList<TNode> Graph<TNode>(TNode? root, TraverseNode<TNode> traverse, Descend? descend = null)
	{
		if (root is null)
			return [];

		var opts = Descend.Normalize(descend, includeSelfDefault: true);
		var signal = new GraphSignal<TNode>(root.ToEnumerable(), opts.DetectCycles, opts.Type);
		var result = new List<TNode>();

		while (signal.TryGetNext(out TNode? current))
		{
			traverse(current, signal);
			if (signal.ShouldYield())
				result.Add(current);
			signal.Cleanup();
		}
		return result;
	}

	/// <summary>
	/// Eagerly traverses a graph from multiple <paramref name="roots"/> using a signal-driven <paramref name="traverse"/> delegate.
	/// </summary>
	/// <typeparam name="TNode">The node type.</typeparam>
	/// <param name="roots">Seed nodes; order influences initial visitation.</param>
	/// <param name="traverse">Delegate per node; use the signal to enqueue, prune, skip or stop.</param>
	/// <param name="descend">Options for controlling how we descend the graph.</param>
	/// <returns>Nodes in visitation order (excluding skipped).</returns>
	public static IList<TNode> Graph<TNode>(IEnumerable<TNode> roots, TraverseNode<TNode> traverse, Descend? descend = null)
	{
		var opts = Descend.Normalize(descend, includeSelfDefault: true);
		var signal = new GraphSignal<TNode>(roots, opts.DetectCycles, opts.Type);
		var result = new List<TNode>();

		while (signal.TryGetNext(out TNode? current))
		{
			traverse(current, signal);

			if (signal.ShouldYield())
				result.Add(current);

			signal.Cleanup();
		}

		return result;
	}
}
