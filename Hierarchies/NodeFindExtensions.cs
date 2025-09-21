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
	public static Node<TItem>? FindDescendant<TItem>(this Node<TItem>? root, Func<Node<TItem>, bool> predicate, Descend? descend = null)
		where TItem : notnull
	{
		if (root is null)
			return null;
		var opts = Descend.Normalize(descend, includeSelfDefault: false);
		return Search.Graph(root, n => n.Children, predicate, opts);
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
	public static Node<TItem>? FindDescendant<TItem>(this IEnumerable<Node<TItem>> roots, Func<Node<TItem>, bool> predicate, Descend? descend = null)
		where TItem : notnull
	{
		var opts = Descend.Normalize(descend, includeSelfDefault: false);
		return Search.Graph(roots, n => n.Children, predicate, opts);
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
	public static IList<Node<TItem>> FindDescendants<TItem>(this Node<TItem>? root, Func<Node<TItem>, bool> predicate, Descend? descend = null)
		where TItem : notnull
	{
		if (root is null)
			return [];
		var opts = Descend.Normalize(descend, includeSelfDefault: false);
		return Search.GraphMany(root, n => n.Children, predicate, opts);
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
	public static IList<Node<TItem>> FindDescendants<TItem>(this IEnumerable<Node<TItem>> roots, Func<Node<TItem>, bool> predicate, Descend? descend = null)
		where TItem : notnull
	{
		var opts = Descend.Normalize(descend, includeSelfDefault: false);
		return Search.GraphMany(roots, n => n.Children, predicate, opts);
	}
	#endregion

	#region Ancestors
	/// <summary>
	/// Find the first ancestor node (optionally including the node itself) matching the predicate.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="node">The starting node.</param>
	/// <param name="predicate">The predicate to match nodes.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>The first matching ancestor node, or null if none found.</returns>
	public static Node<TItem>? FindAncestor<TItem>(this Node<TItem>? node, Func<Node<TItem>, bool> predicate, Ascend? ascend = null)
		where TItem : notnull
	{
		var opts = Ascend.Normalize(ascend, includeSelfDefault: false);
		return opts.IncludeSelf == true
			? Search.Sequence(node, n => n.Parent, predicate)
			: Search.Sequence(node?.Parent, n => n.Parent, predicate);
	}

	/// <summary>
	/// Find the first ancestor node across multiple starting nodes (optionally including each start) matching the predicate.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="nodes">The starting nodes.</param>
	/// <param name="predicate">The predicate to match nodes.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>The first matching ancestor node, or null if none found.</returns>
	public static Node<TItem>? FindAncestor<TItem>(this IEnumerable<Node<TItem>> nodes, Func<Node<TItem>, bool> predicate, Ascend? ascend = null)
		where TItem : notnull
	{
		var opts = Ascend.Normalize(ascend, includeSelfDefault: false);
		var seen = new HashSet<Node<TItem>>(ReferenceEqualityComparer.Instance);
		foreach (var node in nodes)
		{
			var current = opts.IncludeSelf == true ? node : node.Parent;
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
	/// Find all ancestor nodes (optionally including the node itself) matching the predicate.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="node">The starting node.</param>
	/// <param name="predicate">The predicate to match nodes.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>All matching ancestor nodes.</returns>
	public static IList<Node<TItem>> FindAncestors<TItem>(this Node<TItem>? node, Func<Node<TItem>, bool> predicate, Ascend? ascend = null)
		where TItem : notnull
	{
		var opts = Ascend.Normalize(ascend, includeSelfDefault: false);
		return Search.SequenceMany(opts.IncludeSelf == true ? node : node?.Parent, n => n.Parent, predicate);
	}

	/// <summary>
	/// Find all ancestor nodes across multiple starting nodes (optionally including each start) matching the predicate.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="nodes">The starting nodes.</param>
	/// <param name="predicate">The predicate to match nodes.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>All matching ancestor nodes.</returns>
	public static IList<Node<TItem>> FindAncestors<TItem>(this IEnumerable<Node<TItem>> nodes, Func<Node<TItem>, bool> predicate, Ascend? ascend = null)
		where TItem : notnull
	{
		var opts = Ascend.Normalize(ascend, includeSelfDefault: false);
		var seen = new HashSet<Node<TItem>>(ReferenceEqualityComparer.Instance);
		var results = new List<Node<TItem>>();
		foreach (var node in nodes)
		{
			var current = opts.IncludeSelf == true ? node : node.Parent;
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
	/// Find the closest common ancestor node (optionally the node itself).
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="node">The node.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>The node itself if included, otherwise the first common ancestor, or null.</returns>
	public static Node<TItem>? FindCommonAncestor<TItem>(this Node<TItem>? node, Ascend? ascend = null)
		where TItem : notnull
	{
		if (node == null)
			return null;
		var opts = Ascend.Normalize(ascend, includeSelfDefault: false);
		return opts.IncludeSelf == true ? node : null;
	}

	/// <summary>
	/// Find the closest common ancestor for multiple nodes.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="nodes">The nodes to inspect.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>The closest common ancestor, or null if none exists.</returns>
	public static Node<TItem>? FindCommonAncestor<TItem>(this IEnumerable<Node<TItem>> nodes, Ascend? ascend = null)
		where TItem : notnull
	{
		var opts = Ascend.Normalize(ascend, includeSelfDefault: false);
		var commonAncestors = nodes.FindCommonAncestorSet(opts);
		return commonAncestors?.FirstOrDefault();
	}

	/// <summary>
	/// Find all common ancestor nodes for multiple nodes.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="nodes">The nodes to inspect.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>All common ancestor nodes, or empty list if none exist.</returns>
	public static IList<Node<TItem>> FindCommonAncestors<TItem>(this IEnumerable<Node<TItem>> nodes, Ascend? ascend = null)
		where TItem : notnull
	{
		var opts = Ascend.Normalize(ascend, includeSelfDefault: false);
		var commonAncestors = nodes.FindCommonAncestorSet(opts);
		return commonAncestors?.ToList() ?? [];
	}

	/// <summary>
	/// Find items from all common ancestor nodes for multiple nodes.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="nodes">The nodes to inspect.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>Items from all common ancestor nodes, or empty list if none exist.</returns>
	public static IList<TItem> FindCommonAncestorItems<TItem>(this IEnumerable<Node<TItem>> nodes, Ascend? ascend = null)
		where TItem : notnull
	{
		return nodes.FindCommonAncestors(ascend).Select(n => n.Item).ToList();
	}

	/// <summary>
	/// Find the set of ancestor nodes common to the nodes.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="nodes">The nodes to inspect.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>Set of common ancestor nodes, or null if no common ancestors exist.</returns>
	public static HashSet<Node<TItem>>? FindCommonAncestorSet<TItem>(this IEnumerable<Node<TItem>> nodes, Ascend? ascend = null)
		where TItem : notnull
	{
		var opts = Ascend.Normalize(ascend, includeSelfDefault: false);
		HashSet<Node<TItem>>? commonAncestors = null;
		foreach (var node in nodes)
		{
			var ancestors = new HashSet<Node<TItem>>(node.GetAncestors(opts), ReferenceEqualityComparer.Instance);
			if (commonAncestors == null)
				commonAncestors = ancestors;
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
