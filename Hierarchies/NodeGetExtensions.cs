namespace Loken.Hierarchies;

/// <summary>
/// Extension methods for retrieving nodes within a graph of <see cref="Node{TItem}"/>s.
/// </summary>
public static class NodeGetExtensions
{
	#region Descendants
	/// <summary>
	/// Get descendant nodes by traversing according to the options.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="root">The root node to traverse from.</param>
	/// <param name="includeSelf">Whether to include the root node itself.</param>
	/// <param name="type">The type of traversal to use (breadth-first or depth-first).</param>
	/// <returns>An enumeration of nodes.</returns>
	public static IList<Node<TItem>> GetDescendants<TItem>(this Node<TItem>? root, bool includeSelf = false, TraversalType type = TraversalType.BreadthFirst)
		where TItem : notnull
	{
		if (root == null)
			return [];

		return includeSelf
			? [.. Traverse.Graph(root, node => node.Children, false, type)]
			: [.. Traverse.Graph(root.Children, node => node.Children, false, type)];
	}

	/// <summary>
	/// Get descendant nodes by traversing according to the options.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="roots">The root nodes to traverse from.</param>
	/// <param name="includeSelf">Whether to include the root nodes themselves.</param>
	/// <param name="type">The type of traversal to use (breadth-first or depth-first).</param>
	/// <returns>An enumeration of nodes.</returns>
	public static IList<Node<TItem>> GetDescendants<TItem>(this IEnumerable<Node<TItem>> roots, bool includeSelf = false, TraversalType type = TraversalType.BreadthFirst)
		where TItem : notnull
	{
		return includeSelf
			? [.. Traverse.Graph(roots, node => node.Children, false, type)]
			: [.. Traverse.Graph(roots.SelectMany(r => r.Children), node => node.Children, false, type)];
	}
	#endregion

	#region Ancestors
	/// <summary>
	/// Walks up the <see cref="Parent"/> links yielding each node.
	/// </summary>
	/// <param name="includeSelf">
	/// Should the node itself be yielded (true) or should it start
	/// with its <see cref="Parent"/> (false)?
	/// </param>
	/// <returns>An enumeration of nodes.</returns>
	public static IList<Node<TItem>> GetAncestors<TItem>(this Node<TItem>? node, bool includeSelf = false)
		where TItem : notnull
	{
		if (node == null)
			return [];

		var first = includeSelf ? node : node.Parent;
		return [.. Traverse.Sequence(first, n => n.Parent)];
	}

	/// <summary>
	/// Get all unique ancestor nodes from multiple starting nodes with deduplication.
	/// Matches TypeScript behavior where ancestors are collected bottom-up with deduplication.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="nodes">The starting nodes.</param>
	/// <param name="includeSelf">Whether to include the starting nodes themselves.</param>
	/// <returns>All unique ancestor nodes in the order they were first encountered.</returns>
	public static IList<Node<TItem>> GetAncestors<TItem>(this IEnumerable<Node<TItem>> nodes, bool includeSelf = false)
		where TItem : notnull
	{
		var seen = new HashSet<Node<TItem>>();
		var ancestors = new List<Node<TItem>>();

		foreach (var node in nodes)
		{
			var current = includeSelf ? node : node.Parent;

			while (current != null && !seen.Contains(current))
			{
				seen.Add(current);
				ancestors.Add(current);
				current = current.Parent;
			}
		}

		return ancestors;
	}
	#endregion
}
