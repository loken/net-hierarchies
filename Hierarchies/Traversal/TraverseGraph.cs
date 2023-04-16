using Loken.System.Collections;

namespace Loken.Hierarchies.Traversal;

/// <summary>
/// Provides traversal for sequences, trees and graphs.
/// </summary>
public static partial class Traverse
{
	/// <summary>
	/// Traverse a graph of nodes.
	/// <para>Similar to <see cref="Traverse.Tree"/>, but detects graph cycles.</para>
	/// </summary>
	/// <typeparam name="TNode">The type of node.</typeparam>
	/// <param name="root">The root may have a parent, but it is treated as a depth 0 node for the traversal.</param>
	/// <param name="next">Describes the next nodes, or children, of the current node, if any.</param>
	/// <returns>An enumeration of nodes.</returns>
	public static IEnumerable<TNode> Graph<TNode>(TNode? root, NextNodes<TNode> next, bool detectCycles = false)
		where TNode : notnull
	{
		return Graph(root, (n, s) => s.Next(next(n)), detectCycles);
	}

	/// <summary>
	/// Traverse a graph of nodes.
	/// <para>Similar to <see cref="Traverse.Tree"/>, but detects graph cycles.</para>
	/// <para>Use the <see cref="GraphSignal{T}"/> to provide the next nodes
	/// and whether to include or skip the current node.</para>
	/// <para>By providing no next nodes you exclude any children of the current node from traversal.</para>
	/// </summary>
	/// <typeparam name="TNode">The type of node.</typeparam>
	/// <param name="root">The root may have a parent, but it is treated as a depth 0 node for the traversal.</param>
	/// <param name="traverse">The traversal action where you detail what's next and what to skip.</param>
	/// <returns>An enumeration of nodes.</returns>
	public static IEnumerable<TNode> Graph<TNode>(TNode? root, TraverseNode<TNode> traverse, bool detectCycles = false)
		where TNode : notnull
	{
		return Graph(root.ToEnumerable(), traverse, detectCycles);
	}

	/// <summary>
	/// Traverse a graph of nodes.
	/// <para>Similar to <see cref="Traverse.Tree"/>, but detects graph cycles.</para>
	/// </summary>
	/// <typeparam name="TNode">The type of node.</typeparam>
	/// <param name="roots">The roots may have parents, but they are treated as depth 0 nodes for the traversal.</param>
	/// <param name="next">Describes the next nodes, or children, of the current node, if any.</param>
	/// <returns>An enumeration of nodes.</returns>
	public static IEnumerable<TNode> Graph<TNode>(IEnumerable<TNode> roots, NextNodes<TNode> next, bool detectCycles = false)
		where TNode : notnull
	{
		return Graph(roots, (n, s) => s.Next(next(n)), detectCycles);
	}

	/// <summary>
	/// Traverse a graph of nodes.
	/// <para>Similar to <see cref="Traverse.Tree"/>, but detects graph cycles.</para>
	/// <para>Use the <see cref="GraphSignal{T}"/> to provide the next nodes
	/// and whether to include or skip the current node.</para>
	/// <para>By providing no next nodes you exclude any children of the current node from traversal.</para>
	/// </summary>
	/// <typeparam name="TNode">The type of node.</typeparam>
	/// <param name="roots">The roots may have parents, but they are treated as depth 0 nodes for the traversal.</param>
	/// <param name="traverse">The traversal action where you detail what's next and what to skip.</param>
	/// <returns>An enumeration of nodes.</returns>
	public static IEnumerable<TNode> Graph<TNode>(IEnumerable<TNode> roots, TraverseNode<TNode> traverse, bool detectCycles = false)
		where TNode : notnull
	{
		var signal = new GraphSignal<TNode>(roots, detectCycles);

		while (signal.TryGetNext(out TNode? current))
		{
			traverse(current, signal);

			if (signal.ShouldYield())
				yield return current;
		}
	}
}
