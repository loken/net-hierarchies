namespace Loken.Hierarchies;

/// <summary>
/// Extensions for checking existence of ancestors in a <see cref="Hierarchy{TItem,TId}"/>.
/// </summary>
public static class HierarchyHasAncestorExtensions
{
	#region HasAncestor
	/// <summary>
	/// Does a node with the id have an ancestor node matching the search?
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The ID of node to search from.</param>
	/// <param name="search">The search criteria - ID to match.</param>
	/// <param name="includeSelf">Whether to include the starting node in the search.</param>
	/// <returns>True if any ancestor matches the search criteria.</returns>
	public static bool HasAncestor<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, TId search, bool includeSelf = false)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNode(id)
			.HasAncestor(n => search.Equals(hierarchy.Identify(n.Item)), includeSelf);
	}

	/// <summary>
	/// Does a node with the id have an ancestor node matching the search?
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The ID of node to search from.</param>
	/// <param name="search">The search criteria - IDs to match.</param>
	/// <param name="includeSelf">Whether to include the starting node in the search.</param>
	/// <returns>True if any ancestor matches the search criteria.</returns>
	public static bool HasAncestor<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, IEnumerable<TId> search, bool includeSelf = false)
		where TId : notnull
		where TItem : notnull
	{
		var searchSet = new HashSet<TId>(search);
		return hierarchy
			.GetNode(id)
			.HasAncestor(n => searchSet.Contains(hierarchy.Identify(n.Item)), includeSelf);
	}

	/// <summary>
	/// Does a node with the id have an ancestor node matching the search predicate?
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The ID of node to search from.</param>
	/// <param name="search">The predicate function to match nodes.</param>
	/// <param name="includeSelf">Whether to include the starting node in the search.</param>
	/// <returns>True if any ancestor matches the search criteria.</returns>
	public static bool HasAncestor<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, Func<Node<TItem>, bool> search, bool includeSelf = false)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNode(id)
			.HasAncestor(search, includeSelf);
	}


	/// <summary>
	/// Does a node with one of the ids have an ancestor node matching the search?
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The IDs of nodes to search from.</param>
	/// <param name="search">The search criteria - either IDs or a predicate function.</param>
	/// <param name="includeSelf">Whether to include the starting nodes in the search.</param>
	/// <returns>True if any ancestor matches the search criteria.</returns>
	public static bool HasAncestor<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, TId search, bool includeSelf = false)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNodes(ids)
			.HasAncestor(n => search.Equals(hierarchy.Identify(n.Item)), includeSelf);
	}

	/// <summary>
	/// Does a node with one of the ids have an ancestor node matching the search?
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The IDs of nodes to search from.</param>
	/// <param name="search">The search criteria - either IDs or a predicate function.</param>
	/// <param name="includeSelf">Whether to include the starting nodes in the search.</param>
	/// <returns>True if any ancestor matches the search criteria.</returns>
	public static bool HasAncestor<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, IEnumerable<TId> search, bool includeSelf = false)
		where TId : notnull
		where TItem : notnull
	{
		var searchSet = new HashSet<TId>(search);
		return hierarchy
			.GetNodes(ids)
			.HasAncestor(n => searchSet.Contains(hierarchy.Identify(n.Item)), includeSelf);
	}

	/// <summary>
	/// Does a node with one of the ids have an ancestor node matching the search predicate?
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The IDs of nodes to search from.</param>
	/// <param name="search">The predicate function to match nodes.</param>
	/// <param name="includeSelf">Whether to include the starting nodes in the search.</param>
	/// <returns>True if any ancestor matches the search criteria.</returns>
	public static bool HasAncestor<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, Func<Node<TItem>, bool> search, bool includeSelf = false)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNodes(ids)
			.HasAncestor(search, includeSelf);
	}
	#endregion

	#region Common Ancestors
	/// <summary>
	/// Does a hierarchy have a common ancestor for the given ids?
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The identifiers for the items to check for common ancestor.</param>
	/// <param name="includeSelf">Whether to include the starting nodes themselves as potential ancestors.</param>
	/// <returns>True if a common ancestor exists, false otherwise.</returns>
	public static bool HasCommonAncestor<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, bool includeSelf = false)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.FindCommonAncestor(ids, includeSelf) != null;
	}
	#endregion
}
