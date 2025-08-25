namespace Loken.Hierarchies.Traversal;

/// <summary>
/// Describes how to traverse a graph when visiting a node using a <see cref="GraphSignal{TNode}"/>.
/// </summary>
/// <typeparam name="TNode">The type of node.</typeparam>
/// <param name="node">The source node.</param>
/// <param name="signal">Use this to signal how to traverse.</param>
public delegate void TraverseNode<TNode>(TNode node, GraphSignal<TNode> signal);

/// <summary>
/// Provides traversal for sequences, trees and graphs.
/// </summary>
public static partial class Traverse
{
	/// <summary>
	/// Traverse a graph of nodes.
	/// <para>Similar to <see cref="Flatten.Graph"/>, but yields nodes lazily.</para>
	/// </summary>
	/// <typeparam name="TNode">The type of node.</typeparam>
	/// <param name="root">The root may have a parent, but it is treated as a depth 0 node for the traversal.</param>
	/// <param name="next">Describes the next nodes, or children, of the current node, if any.</param>
	/// <returns>An enumeration of nodes.</returns>
	public static IEnumerable<TNode> Graph<TNode>(TNode? root, Func<TNode, IEnumerable<TNode>?> next, bool detectCycles = false, TraversalType type = TraversalType.BreadthFirst)
	{
		if (root is null)
			yield break;

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

		// Single-root fast path; avoid IEnumerable machinery.
		store.Attach(root);

		while (store.TryDetach(out var node))
		{
			if (visited is null || visited.Add(node))
			{
				yield return node;

				var children = next(node);
				if (children is not null)
				{
					// Prefer concrete types to avoid enumerator overhead where possible.
					if (children is IList<TNode> childList)
						store.AttachMany(childList);
					else if (children is ICollection<TNode> childCollection)
						store.AttachMany(childCollection);
					else
						store.AttachMany(children);
				}
			}
		}
	}

	/// <summary>
	/// Traverse a graph of nodes.
	/// <para>Similar to <see cref="Flatten.Graph"/>, but yields nodes lazily.</para>
	/// <para>Use the <see cref="GraphSignal{T}"/> to provide the next nodes
	/// and whether to include or skip the current node.</para>
	/// <para>By providing no next nodes you exclude any children of the current node from traversal.</para>
	/// </summary>
	/// <typeparam name="TNode">The type of node.</typeparam>
	/// <param name="root">The root may have a parent, but it is treated as a depth 0 node for the traversal.</param>
	/// <param name="traverse">The traversal action where you detail what's next and what to skip.</param>
	/// <returns>An enumeration of nodes.</returns>
	public static IEnumerable<TNode> Graph<TNode>(TNode? root, TraverseNode<TNode> traverse, bool detectCycles = false, TraversalType type = TraversalType.BreadthFirst)
	{
		return Graph(root.ToEnumerable(), traverse, detectCycles, type);
	}

	/// <summary>
	/// Traverse a graph of nodes.
	/// <para>Similar to <see cref="Flatten.Graph"/>, but yields nodes lazily.</para>
	/// </summary>
	/// <typeparam name="TNode">The type of node.</typeparam>
	/// <param name="roots">The roots may have parents, but they are treated as depth 0 nodes for the traversal.</param>
	/// <param name="next">Describes the next nodes, or children, of the current node, if any.</param>
	/// <returns>An enumeration of nodes.</returns>
	public static IEnumerable<TNode> Graph<TNode>(IEnumerable<TNode> roots, Func<TNode, IEnumerable<TNode>?> next, bool detectCycles = false, TraversalType type = TraversalType.BreadthFirst)
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
		if (roots is IList<TNode> rootList)
			store.AttachMany(rootList);
		else if (roots is ICollection<TNode> rootCollection)
			store.AttachMany(rootCollection);
		else
			store.AttachMany(roots);

		while (store.TryDetach(out var node))
		{
			if (visited is null || visited.Add(node))
			{
				yield return node;

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
	}

	/// <summary>
	/// Traverse a graph of nodes.
	/// <para>Similar to <see cref="Flatten.Graph"/>, but yields nodes lazily.</para>
	/// <para>Use the <see cref="GraphSignal{T}"/> to provide the next nodes
	/// and whether to include or skip the current node.</para>
	/// <para>By providing no next nodes you exclude any children of the current node from traversal.</para>
	/// </summary>
	/// <typeparam name="TNode">The type of node.</typeparam>
	/// <param name="roots">The roots may have parents, but they are treated as depth 0 nodes for the traversal.</param>
	/// <param name="traverse">The traversal action where you detail what's next and what to skip.</param>
	/// <returns>An enumeration of nodes.</returns>
	public static IEnumerable<TNode> Graph<TNode>(IEnumerable<TNode> roots, TraverseNode<TNode> traverse, bool detectCycles = false, TraversalType type = TraversalType.BreadthFirst)
	{
		var signal = new GraphSignal<TNode>(roots, detectCycles, type);

		while (signal.TryGetNext(out TNode? current))
		{
			traverse(current, signal);

			if (signal.ShouldYield())
				yield return current;

			signal.Cleanup();
		}
	}
}
