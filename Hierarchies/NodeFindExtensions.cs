namespace Loken.Hierarchies;

/// <summary>
/// Extension methods for finding and searching nodes within a graph of <see cref="Node{TItem}"/>s.
/// </summary>
public static class NodeFindExtensions
{
	#region Descendants
	/// <summary>
	/// Find the first descendant node matching the predicate.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="root">The root node to search from.</param>
	/// <param name="predicate">The predicate to match nodes.</param>
	/// <param name="includeSelf">Whether to include the root node itself in the search.</param>
	/// <param name="traversalType">The type of traversal to use (breadth-first or depth-first).</param>
	/// <returns>The first matching descendant node, or null if none found.</returns>
	public static Node<TItem>? FindDescendant<TItem>(this Node<TItem>? root, Func<Node<TItem>, bool> predicate, bool includeSelf = false, TraversalType traversalType = TraversalType.BreadthFirst)
		where TItem : notnull
	{
		if (root == null)
			return null;

		return includeSelf
			? Search.Graph(root, n => n.Children, predicate, false, traversalType)
			: Search.Graph(root.Children, n => n.Children, predicate, false, traversalType);
	}

	/// <summary>
	/// Find the first descendant node matching the predicate.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="roots">The root nodes to search from.</param>
	/// <param name="predicate">The predicate to match nodes.</param>
	/// <param name="includeSelf">Whether to include the root nodes themselves in the search.</param>
	/// <param name="traversalType">The type of traversal to use (breadth-first or depth-first).</param>
	/// <returns>The first matching descendant node, or null if none found.</returns>
	public static Node<TItem>? FindDescendant<TItem>(this IEnumerable<Node<TItem>> roots, Func<Node<TItem>, bool> predicate, bool includeSelf = false, TraversalType traversalType = TraversalType.BreadthFirst)
		where TItem : notnull
	{
		return includeSelf
			? Search.Graph(roots, n => n.Children, predicate, false, traversalType)
			: Search.Graph(roots.SelectMany(r => r.Children), n => n.Children, predicate, false, traversalType);
	}

	/// <summary>
	/// Find all descendant nodes matching the predicate.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="root">The root node to search from.</param>
	/// <param name="predicate">The predicate to match nodes.</param>
	/// <param name="includeSelf">Whether to include the root node itself in the search.</param>
	/// <param name="traversalType">The type of traversal to use (breadth-first or depth-first).</param>
	/// <returns>All matching descendant nodes.</returns>
	public static IList<Node<TItem>> FindDescendants<TItem>(this Node<TItem>? root, Func<Node<TItem>, bool> predicate, bool includeSelf = false, TraversalType traversalType = TraversalType.BreadthFirst)
		where TItem : notnull
	{
		if (root == null)
			return [];

		return includeSelf
			? Search.GraphMany(root, n => n.Children, predicate, false, traversalType)
			: Search.GraphMany(root.Children, n => n.Children, predicate, false, traversalType);
	}

	/// <summary>
	/// Find all descendant nodes matching the predicate.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="roots">The root nodes to search from.</param>
	/// <param name="predicate">The predicate to match nodes.</param>
	/// <param name="includeSelf">Whether to include the root nodes themselves in the search.</param>
	/// <param name="traversalType">The type of traversal to use (breadth-first or depth-first).</param>
	/// <returns>All matching descendant nodes.</returns>
	public static IList<Node<TItem>> FindDescendants<TItem>(this IEnumerable<Node<TItem>> roots, Func<Node<TItem>, bool> predicate, bool includeSelf = false, TraversalType traversalType = TraversalType.BreadthFirst)
		where TItem : notnull
	{
		return includeSelf
			? Search.GraphMany(roots, n => n.Children, predicate, false, traversalType)
			: Search.GraphMany(roots.SelectMany(r => r.Children), n => n.Children, predicate, false, traversalType);
	}
	#endregion

	#region Ancestors
	/// <summary>
	/// Find the first ancestor node matching the predicate.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="node">The starting node.</param>
	/// <param name="predicate">The predicate to match nodes.</param>
	/// <param name="includeSelf">Whether to include the starting node itself in the search.</param>
	/// <returns>The first matching ancestor node, or null if none found.</returns>
	public static Node<TItem>? FindAncestor<TItem>(this Node<TItem>? node, Func<Node<TItem>, bool> predicate, bool includeSelf = false)
		where TItem : notnull
	{
		return includeSelf
			? Search.Sequence(node, n => n.Parent, predicate)
			: Search.Sequence(node?.Parent, n => n.Parent, predicate);
	}

	/// <summary>
	/// Find the first ancestor node matching the predicate.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="nodes">The starting nodes.</param>
	/// <param name="predicate">The predicate to match nodes.</param>
	/// <param name="includeSelf">Whether to include the starting nodes themselves in the search.</param>
	/// <returns>The first matching ancestor node, or null if none found.</returns>
	public static Node<TItem>? FindAncestor<TItem>(this IEnumerable<Node<TItem>> nodes, Func<Node<TItem>, bool> predicate, bool includeSelf = false)
		where TItem : notnull
	{
		var seen = new HashSet<Node<TItem>>(ReferenceEqualityComparer.Instance);

		foreach (var node in nodes)
		{
			var current = includeSelf ? node : node.Parent;

			while (current != null && !seen.Contains(current))
			{
				seen.Add(current);

				if (predicate(current))
					return current;

				current = current.Parent;
			}
		}

		return null;
	}

	/// <summary>
	/// Find all ancestor nodes matching the predicate.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="node">The starting node.</param>
	/// <param name="predicate">The predicate to match nodes.</param>
	/// <param name="includeSelf">Whether to include the starting node itself in the search.</param>
	/// <returns>All matching ancestor nodes.</returns>
	public static IList<Node<TItem>> FindAncestors<TItem>(this Node<TItem>? node, Func<Node<TItem>, bool> predicate, bool includeSelf = false)
		where TItem : notnull
	{
		return Search.SequenceMany(includeSelf ? node : node?.Parent, n => n.Parent, predicate);
	}

	/// <summary>
	/// Find all ancestor nodes matching the predicate.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="nodes">The starting nodes.</param>
	/// <param name="predicate">The predicate to match nodes.</param>
	/// <param name="includeSelf">Whether to include the starting nodes themselves in the search.</param>
	/// <returns>All matching ancestor nodes.</returns>
	public static IList<Node<TItem>> FindAncestors<TItem>(this IEnumerable<Node<TItem>> nodes, Func<Node<TItem>, bool> predicate, bool includeSelf = false)
		where TItem : notnull
	{
		var seen = new HashSet<Node<TItem>>(ReferenceEqualityComparer.Instance);
		var results = new List<Node<TItem>>();

		foreach (var node in nodes)
		{
			var current = includeSelf ? node : node.Parent;

			while (current != null && !seen.Contains(current))
			{
				seen.Add(current);

				if (predicate(current))
					results.Add(current);

				current = current.Parent;
			}
		}

		return results;
	}
	#endregion

	#region Common Ancestors
	/// <summary>
	/// Find the common ancestor node which is the closest to the node.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="node">The node (when includeSelf=true, returns the node itself).</param>
	/// <param name="includeSelf">Whether to include the node itself as potential ancestor.</param>
	/// <returns>The node itself if includeSelf=true, null otherwise.</returns>
	public static Node<TItem>? FindCommonAncestor<TItem>(this Node<TItem>? node, bool includeSelf = false)
		where TItem : notnull
	{
		if (node == null)
			return null;

		return includeSelf ? node : null;
	}

	/// <summary>
	/// Find the common ancestor node which is the closest to the nodes.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="nodes">The nodes to find the common ancestor for.</param>
	/// <param name="includeSelf">Whether to include the nodes themselves as potential ancestors.</param>
	/// <returns>The closest common ancestor, or null if none exists.</returns>
	public static Node<TItem>? FindCommonAncestor<TItem>(this IEnumerable<Node<TItem>> nodes, bool includeSelf = false)
		where TItem : notnull
	{
		var commonAncestors = nodes.FindCommonAncestorSet(includeSelf);
		return commonAncestors?.FirstOrDefault();
	}

	/// <summary>
	/// Find the ancestor nodes common to the nodes.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="nodes">The nodes to find common ancestors for.</param>
	/// <param name="includeSelf">Whether to include the nodes themselves as potential ancestors.</param>
	/// <returns>All common ancestor nodes, or empty list if none exist.</returns>
	public static IList<Node<TItem>> FindCommonAncestors<TItem>(this IEnumerable<Node<TItem>> nodes, bool includeSelf = false)
		where TItem : notnull
	{
		var commonAncestors = nodes.FindCommonAncestorSet(includeSelf);
		return commonAncestors?.ToList() ?? [];
	}

	/// <summary>
	/// Find the ancestor items common to the nodes.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="nodes">The nodes to find common ancestor items for.</param>
	/// <param name="includeSelf">Whether to include the nodes themselves as potential ancestors.</param>
	/// <returns>Items from all common ancestor nodes, or empty list if none exist.</returns>
	public static IList<TItem> FindCommonAncestorItems<TItem>(this IEnumerable<Node<TItem>> nodes, bool includeSelf = false)
		where TItem : notnull
	{
		return nodes.FindCommonAncestors(includeSelf).Select(n => n.Item).ToList();
	}

	/// <summary>
	/// Find the set of ancestor nodes common to the nodes.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="nodes">The nodes to find common ancestors for.</param>
	/// <param name="includeSelf">Whether to include the nodes themselves as potential ancestors.</param>
	/// <returns>Set of common ancestor nodes, or null if no common ancestors exist.</returns>
	public static HashSet<Node<TItem>>? FindCommonAncestorSet<TItem>(this IEnumerable<Node<TItem>> nodes, bool includeSelf = false)
		where TItem : notnull
	{
		HashSet<Node<TItem>>? commonAncestors = null;

		foreach (var node in nodes)
		{
			var ancestors = new HashSet<Node<TItem>>(node.GetAncestors(includeSelf), ReferenceEqualityComparer.Instance);

			if (commonAncestors == null)
			{
				commonAncestors = ancestors;
			}
			else
			{
				commonAncestors.IntersectWith(ancestors);
				if (commonAncestors.Count == 0)
					return [];
			}
		}

		return commonAncestors;
	}
	#endregion
}
