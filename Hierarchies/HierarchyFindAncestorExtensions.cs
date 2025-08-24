namespace Loken.Hierarchies;

/// <summary>
/// Extensions for finding ancestor nodes in a <see cref="Hierarchy{TItem,TId}"/>.
/// </summary>
/// <remarks>
/// There are no overloads mapping to the ID or Item as the caller can easily resolve those on the resulting node.
/// Also, an overload returning an optional ID causes issues with nullability where we would have to return default
/// (which could look like an ID) when there is no match.
/// </remarks>
public static class HierarchyFindAncestorExtensions
{
	#region FindAncestor
	/// <summary>
	/// Find a node matching the search which is an ancestor of the node with the id.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The ID of node to search from.</param>
	/// <param name="search">The search criteria - ID to match.</param>
	/// <param name="includeSelf">Whether to include the starting node in the search.</param>
	/// <returns>The first matching ancestor node, or null if none found.</returns>
	public static Node<TItem>? FindAncestor<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, TId search, bool includeSelf = false)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNode(id)
			.FindAncestor(n => search.Equals(hierarchy.Identify(n.Item)), includeSelf);
	}

	/// <summary>
	/// Find a node matching the search which is an ancestor of the node with the id.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The ID of node to search from.</param>
	/// <param name="search">The search criteria - IDs to match.</param>
	/// <param name="includeSelf">Whether to include the starting node in the search.</param>
	/// <returns>The first matching ancestor node, or null if none found.</returns>
	public static Node<TItem>? FindAncestor<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, IEnumerable<TId> search, bool includeSelf = false)
		where TId : notnull
		where TItem : notnull
	{
		var searchSet = new HashSet<TId>(search);
		return hierarchy
			.GetNode(id)
			.FindAncestor(n => searchSet.Contains(hierarchy.Identify(n.Item)), includeSelf);
	}

	/// <summary>
	/// Find a node matching the search predicate which is an ancestor of the node with the id.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The ID of node to search from.</param>
	/// <param name="search">The predicate function to match nodes.</param>
	/// <param name="includeSelf">Whether to include the starting node in the search.</param>
	/// <returns>The first matching ancestor node, or null if none found.</returns>
	public static Node<TItem>? FindAncestor<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, Func<Node<TItem>, bool> search, bool includeSelf = false)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNode(id)
			.FindAncestor(search, includeSelf);
	}

	/// <summary>
	/// Find a node matching the search which is an ancestor of a node with one of the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The IDs of nodes to search from.</param>
	/// <param name="search">The search criteria - ID to match.</param>
	/// <param name="includeSelf">Whether to include the starting nodes in the search.</param>
	/// <returns>The first matching ancestor node, or null if none found.</returns>
	public static Node<TItem>? FindAncestor<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, TId search, bool includeSelf = false)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNodes(ids)
			.FindAncestor(n => search.Equals(hierarchy.Identify(n.Item)), includeSelf);
	}

	/// <summary>
	/// Find a node matching the search which is an ancestor of a node with one of the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The IDs of nodes to search from.</param>
	/// <param name="search">The search criteria - IDs to match.</param>
	/// <param name="includeSelf">Whether to include the starting nodes in the search.</param>
	/// <returns>The first matching ancestor node, or null if none found.</returns>
	public static Node<TItem>? FindAncestor<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, IEnumerable<TId> search, bool includeSelf = false)
		where TId : notnull
		where TItem : notnull
	{
		var searchSet = new HashSet<TId>(search);
		return hierarchy
			.GetNodes(ids)
			.FindAncestor(n => searchSet.Contains(hierarchy.Identify(n.Item)), includeSelf);
	}

	/// <summary>
	/// Find a node matching the search predicate which is an ancestor of a node with one of the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The IDs of nodes to search from.</param>
	/// <param name="search">The predicate function to match nodes.</param>
	/// <param name="includeSelf">Whether to include the starting nodes in the search.</param>
	/// <returns>The first matching ancestor node, or null if none found.</returns>
	public static Node<TItem>? FindAncestor<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, Func<Node<TItem>, bool> search, bool includeSelf = false)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNodes(ids)
			.FindAncestor(search, includeSelf);
	}
	#endregion

	#region Common Ancestors
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
		return hierarchy
			.GetNodes(ids)
			.FindCommonAncestor(includeSelf);
	}
	#endregion
}
