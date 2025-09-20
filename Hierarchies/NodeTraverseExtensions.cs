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
	public static IEnumerable<Node<TItem>> TraverseDescendants<TItem>(this Node<TItem>? root, Descend? descend = null)
		where TItem : notnull
	{
		if (root is null)
			return [];
		return new[] { root }.TraverseDescendants(descend);
	}

	/// <summary>
	/// Generate a sequence of descendant nodes by traversing according to the options.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="roots">The root nodes to traverse from.</param>
	/// <param name="includeSelf">Whether to include the root nodes themselves.</param>
	/// <param name="traversalType">The type of traversal to use (breadth-first or depth-first).</param>
	/// <returns>Enumerable of descendant nodes in traversal order.</returns>
	public static IEnumerable<Node<TItem>> TraverseDescendants<TItem>(this IEnumerable<Node<TItem>> roots, Descend? descend = null)
		where TItem : notnull
	{
		var opts = Descend.Normalize(descend, includeSelfDefault: false);
		var rootsToTraverse = opts.IncludeSelf == true ? roots : roots.SelectMany(r => r.Children);
		return Traverse.Graph(rootsToTraverse, n => n.Children, opts);
	}
	#endregion
}
