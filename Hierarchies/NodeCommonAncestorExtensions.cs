namespace Loken.Hierarchies;

/// <summary>
/// Extension methods for finding common ancestors between nodes within a graph of <see cref="Node{TItem}"/>s.
/// </summary>
public static class NodeCommonAncestorExtensions
{
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
			var ancestors = new HashSet<Node<TItem>>(node.GetAncestors(includeSelf));

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
}
