namespace Loken.Hierarchies.Traversal;

/// <summary>
/// Provides traversal for sequences, trees and graphs.
/// </summary>
public static partial class Traverse
{
	/// <summary>
	/// Traverse a graph of roots.
	/// <para>Similar to <see cref="Traverse.Tree"/>, but detects graph cycles.</para>
	/// </summary>
	/// <typeparam name="TNode">The type of root.</typeparam>
	/// <param name="root">The root may have a parent, but it is treated as a depth 0 root for the traversal.</param>
	/// <param name="traverse">The traversal action where you detail what's next and what to skip.</param>
	/// <returns>An enumeration of roots.</returns>
	public static IEnumerable<TNode> Graph<TNode>(TNode? root, Func<TNode, IEnumerable<TNode>> next, bool detectCycles = false)
		where TNode : notnull
	{
		return Graph(root, (n, s) => s.Next(next(n)), detectCycles);
	}

	/// <summary>
	/// Traverse a graph of roots.
	/// <para>Similar to <see cref="Traverse.Tree"/>, but detects graph cycles.</para>
	/// <para>Use the <see cref="GraphSignal{T}"/> to provide the next roots
	/// and whether to include or skip the current root.</para>
	/// <para>By providing no next roots you exclude the children of the current root from traversal.</para>
	/// </summary>
	/// <typeparam name="TNode">The type of root.</typeparam>
	/// <param name="root">The root may have a parent, but it is treated as a depth 0 node for the traversal.</param>
	/// <param name="traverse">The traversal action where you detail what's next and what to skip.</param>
	/// <returns>An enumeration of roots.</returns>
	public static IEnumerable<TNode> Graph<TNode>(TNode? root, Action<TNode, GraphSignal<TNode>> traverse, bool detectCycles = false)
		where TNode : notnull
	{
		var roots = root is not null ? new[] { root } : Enumerable.Empty<TNode>();
		return Graph(roots, traverse, detectCycles);
	}


	/// <summary>
	/// Traverse a graph of roots.
	/// <para>Similar to <see cref="Traverse.Tree"/>, but detects graph cycles.</para>
	/// <para>Use the <see cref="GraphSignal{T}"/> to provide the next roots
	/// and whether to include or skip the current root.</para>
	/// <para>By providing no next roots you exclude the children of the current root from traversal.</para>
	/// </summary>
	/// <typeparam name="TNode">The type of root.</typeparam>
	/// <param name="roots">The roots may have parents, but they are treated as depth 0 nodes for the traversal.</param>
	/// <param name="traverse">The traversal action where you detail what's next and what to skip.</param>
	/// <returns>An enumeration of roots.</returns>
	public static IEnumerable<TNode> Graph<TNode>(IEnumerable<TNode> roots, Action<TNode, GraphSignal<TNode>> traverse, bool detectCycles = false)
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
