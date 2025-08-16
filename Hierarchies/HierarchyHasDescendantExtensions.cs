namespace Loken.Hierarchies;

/// <summary>
/// Extensions for checking existence of descendants in a <see cref="Hierarchy{TItem,TId}"/>.
/// </summary>
public static class HierarchyHasDescendantExtensions
{
	/// <summary>
	/// Does a node with the id have a descendant node matching the search?
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The ID of node to search from.</param>
	/// <param name="search">The search criteria - ID to match.</param>
	/// <param name="includeSelf">Whether to include the starting node in the search.</param>
	/// <param name="type">The traversal type (breadth-first or depth-first).</param>
	/// <returns>True if any descendant matches the search criteria.</returns>
	public static bool HasDescendant<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, TId search, bool includeSelf = false, TraversalType type = TraversalType.BreadthFirst)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNode(id)
			.HasDescendant(n => search.Equals(hierarchy.Identify(n.Item)), includeSelf, type);
	}

	/// <summary>
	/// Does a node with the id have a descendant node matching the search?
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The ID of node to search from.</param>
	/// <param name="search">The search criteria - IDs to match.</param>
	/// <param name="includeSelf">Whether to include the starting node in the search.</param>
	/// <param name="type">The traversal type (breadth-first or depth-first).</param>
	/// <returns>True if any descendant matches the search criteria.</returns>
	public static bool HasDescendant<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, IEnumerable<TId> search, bool includeSelf = false, TraversalType type = TraversalType.BreadthFirst)
		where TId : notnull
		where TItem : notnull
	{
		var searchSet = new HashSet<TId>(search);
		return hierarchy
			.GetNode(id)
			.HasDescendant(n => searchSet.Contains(hierarchy.Identify(n.Item)), includeSelf, type);
	}

	/// <summary>
	/// Does a node with the id have a descendant node matching the search predicate?
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The ID of node to search from.</param>
	/// <param name="search">The predicate function to match nodes.</param>
	/// <param name="includeSelf">Whether to include the starting node in the search.</param>
	/// <param name="type">The traversal type (breadth-first or depth-first).</param>
	/// <returns>True if any descendant matches the search criteria.</returns>
	public static bool HasDescendant<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, NodePredicate<TItem> search, bool includeSelf = false, TraversalType type = TraversalType.BreadthFirst)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNode(id)
			.HasDescendant(search, includeSelf, type);
	}

	/// <summary>
	/// Does a node with one of the ids have a descendant node matching the search?
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The IDs of nodes to search from.</param>
	/// <param name="search">The search criteria - ID to match.</param>
	/// <param name="includeSelf">Whether to include the starting nodes in the search.</param>
	/// <param name="type">The traversal type (breadth-first or depth-first).</param>
	/// <returns>True if any descendant matches the search criteria.</returns>
	public static bool HasDescendant<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, TId search, bool includeSelf = false, TraversalType type = TraversalType.BreadthFirst)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNodes(ids)
			.HasDescendant(n => search.Equals(hierarchy.Identify(n.Item)), includeSelf, type);
	}

	/// <summary>
	/// Does a node with one of the ids have a descendant node matching the search?
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The IDs of nodes to search from.</param>
	/// <param name="search">The search criteria - IDs to match.</param>
	/// <param name="includeSelf">Whether to include the starting nodes in the search.</param>
	/// <param name="type">The traversal type (breadth-first or depth-first).</param>
	/// <returns>True if any descendant matches the search criteria.</returns>
	public static bool HasDescendant<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, IEnumerable<TId> search, bool includeSelf = false, TraversalType type = TraversalType.BreadthFirst)
		where TId : notnull
		where TItem : notnull
	{
		var searchSet = new HashSet<TId>(search);
		return hierarchy
			.GetNodes(ids)
			.HasDescendant(n => searchSet.Contains(hierarchy.Identify(n.Item)), includeSelf, type);
	}

	/// <summary>
	/// Does a node with one of the ids have a descendant node matching the search predicate?
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The IDs of nodes to search from.</param>
	/// <param name="search">The predicate function to match nodes.</param>
	/// <param name="includeSelf">Whether to include the starting nodes in the search.</param>
	/// <param name="type">The traversal type (breadth-first or depth-first).</param>
	/// <returns>True if any descendant matches the search criteria.</returns>
	public static bool HasDescendant<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, NodePredicate<TItem> search, bool includeSelf = false, TraversalType type = TraversalType.BreadthFirst)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNodes(ids)
			.HasDescendant(search, includeSelf, type);
	}
}
