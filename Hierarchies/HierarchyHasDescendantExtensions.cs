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
	/// <param name="descend">Options for controlling how we descend the graph.</param>
	/// <param name="type">The traversal type (breadth-first or depth-first).</param>
	/// <returns>True if any descendant matches the search criteria.</returns>
	public static bool HasDescendant<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, TId search, Descend? descend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNode(id)
			.FindDescendant(n => search.Equals(hierarchy.Identify(n.Item)), descend) != null;
	}

	/// <summary>
	/// Does a node with the id have a descendant node matching the search?
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The ID of node to search from.</param>
	/// <param name="search">The search criteria - IDs to match.</param>
	/// <param name="descend">Options for controlling how we descend the graph.</param>
	/// <param name="type">The traversal type (breadth-first or depth-first).</param>
	/// <returns>True if any descendant matches the search criteria.</returns>
	public static bool HasDescendant<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, IEnumerable<TId> search, Descend? descend = null)
		where TId : notnull
		where TItem : notnull
	{
		var searchSet = new HashSet<TId>(search);
		return hierarchy
			.GetNode(id)
			.FindDescendant(n => searchSet.Contains(hierarchy.Identify(n.Item)), descend) != null;
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
	public static bool HasDescendant<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, Func<Node<TItem>, bool> search, Descend? descend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNode(id)
			.FindDescendant(search, descend) != null;
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
	public static bool HasDescendant<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, TId search, Descend? descend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNodes(ids)
			.FindDescendant(n => search.Equals(hierarchy.Identify(n.Item)), descend) != null;
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
	public static bool HasDescendant<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, IEnumerable<TId> search, Descend? descend = null)
		where TId : notnull
		where TItem : notnull
	{
		var searchSet = new HashSet<TId>(search);
		return hierarchy
			.GetNodes(ids)
			.FindDescendant(n => searchSet.Contains(hierarchy.Identify(n.Item)), descend) != null;
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
	public static bool HasDescendant<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, Func<Node<TItem>, bool> search, Descend? descend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNodes(ids)
			.FindDescendant(search, descend) != null;
	}
}
