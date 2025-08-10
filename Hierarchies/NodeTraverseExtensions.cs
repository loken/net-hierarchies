namespace Loken.Hierarchies;

/// <summary>
/// Extension methods for lazy traversal of nodes within a graph of <see cref="Node{TItem}"/>s.
/// </summary>
public static class NodeTraverseExtensions
{
	#region Descendants
	/// <summary>
	/// Generate a sequence of descendant nodes by traversing according to the options.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="root">The root node to traverse from.</param>
	/// <param name="includeSelf">Whether to include the root node itself.</param>
	/// <param name="traversalType">The type of traversal to use (breadth-first or depth-first).</param>
	/// <returns>Enumerable of descendant nodes in traversal order.</returns>
	public static IEnumerable<Node<TItem>> TraverseDescendants<TItem>(this Node<TItem>? root, bool includeSelf = false, TraversalType traversalType = TraversalType.BreadthFirst)
		where TItem : notnull
	{
		if (root == null)
			return [];

		return new[] { root }.TraverseDescendants(includeSelf, traversalType);
	}

	/// <summary>
	/// Generate a sequence of descendant nodes by traversing according to the options.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="roots">The root nodes to traverse from.</param>
	/// <param name="includeSelf">Whether to include the root nodes themselves.</param>
	/// <param name="traversalType">The type of traversal to use (breadth-first or depth-first).</param>
	/// <returns>Enumerable of descendant nodes in traversal order.</returns>
	public static IEnumerable<Node<TItem>> TraverseDescendants<TItem>(this IEnumerable<Node<TItem>> roots, bool includeSelf = false, TraversalType traversalType = TraversalType.BreadthFirst)
		where TItem : notnull
	{
		var rootsToTraverse = includeSelf ? roots : roots.SelectMany(r => r.Children);

		return Traverse.Graph(rootsToTraverse, node => node.Children, false, traversalType);
	}
	#endregion
}
