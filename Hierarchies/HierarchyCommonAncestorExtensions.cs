namespace Loken.Hierarchies;

/// <summary>
/// Extensions for traversing common ancestors of a <see cref="Hierarchy{TItem,TId}"/>.
/// </summary>
public static class HierarchyCommonAncestorExtensions
{
	/// <summary>
	/// Find the common ancestor node which is the closest to the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The identifiers for the items to find common ancestor for.</param>
	/// <param name="includeSelf">Whether to include the starting nodes themselves as potential ancestors.</param>
	/// <returns>The closest common ancestor node, or null if none found.</returns>
	public static Node<TItem>? FindCommonAncestor<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, bool includeSelf = false)
		where TId : notnull
		where TItem : notnull
	{
		var nodes = ids.Select(id => hierarchy.GetNode(id)).ToArray();
		return nodes.FindCommonAncestor(includeSelf);
	}

	/// <summary>
	/// Find the common ancestor nodes for the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The identifiers for the items to find common ancestors for.</param>
	/// <param name="includeSelf">Whether to include the starting nodes themselves as potential ancestors.</param>
	/// <returns>All common ancestor nodes, or empty list if none found.</returns>
	public static IList<Node<TItem>> FindCommonAncestors<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, bool includeSelf = false)
		where TId : notnull
		where TItem : notnull
	{
		var nodes = ids.Select(id => hierarchy.GetNode(id)).ToArray();
		return nodes.FindCommonAncestors(includeSelf) ?? [];
	}

	/// <summary>
	/// Find the IDs of ancestor nodes common to the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The identifiers for the items to find common ancestor IDs for.</param>
	/// <param name="includeSelf">Whether to include the starting nodes themselves as potential ancestors.</param>
	/// <returns>All common ancestor IDs, or empty list if none found.</returns>
	public static IList<TId> FindCommonAncestorIds<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, bool includeSelf = false)
		where TId : notnull
		where TItem : notnull
	{
		var commonAncestors = hierarchy.FindCommonAncestors(ids, includeSelf);
		return commonAncestors.ToIds(hierarchy.Identify).ToList();
	}
}
