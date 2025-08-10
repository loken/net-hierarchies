namespace Loken.Hierarchies;

/// <summary>
/// Extension methods for finding and traversing ancestor nodes within a graph of <see cref="Node{TItem}"/>s.
/// </summary>
public static class NodeAncestorExtensions
{
	/// <summary>
	/// Find the first ancestor node matching the predicate.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="nodes">The starting nodes.</param>
	/// <param name="predicate">The predicate to match nodes.</param>
	/// <param name="includeSelf">Whether to include the starting nodes themselves in the search.</param>
	/// <returns>The first matching ancestor node, or null if none found.</returns>
	public static Node<TItem>? FindAncestor<TItem>(this IEnumerable<Node<TItem>> nodes, NodePredicate<TItem> predicate, bool includeSelf = false)
		where TItem : notnull
	{
		var seen = new HashSet<Node<TItem>>();

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
	/// Find the first ancestor node matching the predicate.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="node">The starting node.</param>
	/// <param name="predicate">The predicate to match nodes.</param>
	/// <param name="includeSelf">Whether to include the starting node itself in the search.</param>
	/// <returns>The first matching ancestor node, or null if none found.</returns>
	public static Node<TItem>? FindAncestor<TItem>(this Node<TItem>? node, NodePredicate<TItem> predicate, bool includeSelf = false)
		where TItem : notnull
	{
		if (node == null)
			return null;

		return new[] { node }.FindAncestor(predicate, includeSelf);
	}

	/// <summary>
	/// Find all ancestor nodes matching the predicate.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="nodes">The starting nodes.</param>
	/// <param name="predicate">The predicate to match nodes.</param>
	/// <param name="includeSelf">Whether to include the starting nodes themselves in the search.</param>
	/// <returns>All matching ancestor nodes.</returns>
	public static IList<Node<TItem>> FindAncestors<TItem>(this IEnumerable<Node<TItem>> nodes, NodePredicate<TItem> predicate, bool includeSelf = false)
		where TItem : notnull
	{
		var seen = new HashSet<Node<TItem>>();
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

	/// <summary>
	/// Find all ancestor nodes matching the predicate.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="node">The starting node.</param>
	/// <param name="predicate">The predicate to match nodes.</param>
	/// <param name="includeSelf">Whether to include the starting node itself in the search.</param>
	/// <returns>All matching ancestor nodes.</returns>
	public static IList<Node<TItem>> FindAncestors<TItem>(this Node<TItem>? node, NodePredicate<TItem> predicate, bool includeSelf = false)
		where TItem : notnull
	{
		if (node == null)
			return [];

		return new[] { node }.FindAncestors(predicate, includeSelf);
	}

	/// <summary>
	/// Check if any ancestor node matches the predicate.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="nodes">The starting nodes.</param>
	/// <param name="predicate">The predicate to match nodes.</param>
	/// <param name="includeSelf">Whether to include the starting nodes themselves in the search.</param>
	/// <returns>True if any ancestor matches, false otherwise.</returns>
	public static bool HasAncestor<TItem>(this IEnumerable<Node<TItem>> nodes, NodePredicate<TItem> predicate, bool includeSelf = false)
		where TItem : notnull
	{
		return nodes.FindAncestor(predicate, includeSelf) != null;
	}

	/// <summary>
	/// Check if any ancestor node matches the predicate.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="node">The starting node.</param>
	/// <param name="predicate">The predicate to match nodes.</param>
	/// <param name="includeSelf">Whether to include the starting node itself in the search.</param>
	/// <returns>True if any ancestor matches, false otherwise.</returns>
	public static bool HasAncestor<TItem>(this Node<TItem>? node, NodePredicate<TItem> predicate, bool includeSelf = false)
		where TItem : notnull
	{
		return node.FindAncestor(predicate, includeSelf) != null;
	}

	/// <summary>
	/// Walks up the <see cref="Parent"/> links yielding each node.
	/// </summary>
	/// <param name="includeSelf">
	/// Should the node itself be yielded (true) or should it start
	/// with its <see cref="Parent"/> (false)?
	/// </param>
	/// <returns>An enumeration of nodes.</returns>
	public static IEnumerable<Node<TItem>> GetAncestors<TItem>(this Node<TItem> node, bool includeSelf = false)
		where TItem : notnull
	{
		var first = includeSelf ? node : node.Parent;
		return Traverse.Sequence(first, n => n.Parent);
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

	/// <summary>
	/// Get ancestor items from unique ancestor nodes.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="nodes">The starting nodes.</param>
	/// <param name="includeSelf">Whether to include the starting nodes themselves.</param>
	/// <returns>Items from all unique ancestor nodes.</returns>
	public static IList<TItem> GetAncestorItems<TItem>(this IEnumerable<Node<TItem>> nodes, bool includeSelf = false)
		where TItem : notnull
	{
		var seen = new HashSet<Node<TItem>>();
		var ancestors = new List<TItem>();

		foreach (var node in nodes)
		{
			var current = includeSelf ? node : node.Parent;

			while (current != null && !seen.Contains(current))
			{
				seen.Add(current);
				ancestors.Add(current.Item);
				current = current.Parent;
			}
		}

		return ancestors;
	}

	/// <summary>
	/// Get ancestor items from unique ancestor nodes.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="node">The starting node.</param>
	/// <param name="includeSelf">Whether to include the starting node itself.</param>
	/// <returns>Items from all unique ancestor nodes.</returns>
	public static IList<TItem> GetAncestorItems<TItem>(this Node<TItem>? node, bool includeSelf = false)
		where TItem : notnull
	{
		if (node == null)
			return [];

		return new[] { node }.GetAncestorItems(includeSelf);
	}
}
