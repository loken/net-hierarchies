namespace Loken.Hierarchies.Traversal;

/// <summary>
/// Provides eager traversal helpers that return arrays for sequences, trees and graphs.
/// </summary>
public static partial class Flatten
{
	/// <summary>
	/// Flatten a graph of nodes.
	/// <para>Similar to <see cref="Traverse.Graph"/>, but collects eagerly.</para>
	/// <typeparam name="TNode">The type of node.</typeparam>
	/// <param name="root">The root may have a parent, but it is treated as a depth 0 node for the traversal.</param>
	/// <param name="next">Describes the next nodes, or children, of the current node, if any.</param>
	/// <returns>A list of nodes.</returns>
	/// </summary>
	public static IList<TNode> Graph<TNode>(TNode? root, NextNodes<TNode> next, bool detectCycles = false, TraversalType type = TraversalType.BreadthFirst)
		where TNode : notnull
	{
		if (root is null)
			return [];

		HashSet<TNode>? visited = null;
		if (detectCycles)
		{
			// Align with TS semantics: nodes are considered unique by identity, not by item equality.
			// If TNode is a reference type, use a reference comparer; otherwise use default comparer.
			visited = typeof(TNode).IsValueType
				? new HashSet<TNode>()
				: new HashSet<TNode>(ReferenceEqualityComparer<TNode>.Instance);
		}
		ILinear<TNode> store = type == TraversalType.DepthFirst
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
					// Prefer concrete types to avoid enumerator overhead where possible.
					if (children is TNode[] childArray)
						store.Attach(childArray);
					else if (children is List<TNode> childList)
						store.Attach(childList);
					else
						store.Attach(children);
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Flatten a graph of nodes.
	/// <para>Uses a <see cref="GraphSignal{T}"/> to control traversal and yielding.</para>
	/// <typeparam name="TNode">The type of node.</typeparam>
	/// <param name="root">The root may have a parent, but it is treated as a depth 0 node for the traversal.</param>
	/// <param name="traverse">The traversal action where you detail what's next and what to skip.</param>
	/// <returns>A list of nodes.</returns>
	/// </summary>
	public static IList<TNode> Graph<TNode>(TNode? root, TraverseNode<TNode> traverse, bool detectCycles = false, TraversalType type = TraversalType.BreadthFirst)
		where TNode : notnull
	{
		if (root is null)
			return [];

		return Graph(root.ToEnumerable(), traverse, detectCycles, type);
	}

	/// <summary>
	/// Flatten a graph of nodes.
	/// <para>Similar to <see cref="Traverse.Graph"/>, but collects eagerly.</para>
	/// <typeparam name="TNode">The type of node.</typeparam>
	/// <param name="roots">The roots may have parents, but they are treated as depth 0 nodes for the traversal.</param>
	/// <param name="next">Describes the next nodes, or children, of the current node, if any.</param>
	/// <returns>A list of nodes.</returns>
	/// </summary>
	public static IList<TNode> Graph<TNode>(IEnumerable<TNode> roots, NextNodes<TNode> next, bool detectCycles = false, TraversalType type = TraversalType.BreadthFirst)
		where TNode : notnull
	{
		HashSet<TNode>? visited = null;
		if (detectCycles)
		{
			visited = typeof(TNode).IsValueType
				? new HashSet<TNode>()
				: new HashSet<TNode>(ReferenceEqualityComparer<TNode>.Instance);
		}
		ILinear<TNode> store = type == TraversalType.DepthFirst
			? new LinearStack<TNode>()
			: new LinearQueue<TNode>();

		// Prefer concrete types to avoid enumerator overhead when seeding roots.
		if (roots is TNode[] arr)
			store.Attach(arr);
		else if (roots is List<TNode> list)
			store.Attach(list);
		else
			store.Attach(roots);

		var result = new List<TNode>();
		while (store.TryDetach(out var node))
		{
			if (visited is null || visited.Add(node))
			{
				result.Add(node);

				var children = next(node);
				if (children is not null)
				{
					if (children is TNode[] childArray)
						store.Attach(childArray);
					else if (children is List<TNode> childList)
						store.Attach(childList);
					else
						store.Attach(children);
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Flatten a graph of nodes.
	/// <para>Uses a <see cref="GraphSignal{T}"/> to control traversal and yielding.</para>
	/// <typeparam name="TNode">The type of node.</typeparam>
	/// <param name="roots">The roots may have parents, but they are treated as depth 0 nodes for the traversal.</param>
	/// <param name="traverse">The traversal action where you detail what's next and what to skip.</param>
	/// <returns>A list of nodes.</returns>
	/// </summary>
	public static IList<TNode> Graph<TNode>(IEnumerable<TNode> roots, TraverseNode<TNode> traverse, bool detectCycles = false, TraversalType type = TraversalType.BreadthFirst)
		where TNode : notnull
	{
		var signal = new GraphSignal<TNode>(roots, detectCycles, type);
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
