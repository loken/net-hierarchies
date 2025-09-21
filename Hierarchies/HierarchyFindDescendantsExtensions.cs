namespace Loken.Hierarchies;

/// <summary>
/// Extensions for finding and searching for multiple descendants in a <see cref="Hierarchy{TItem,TId}"/>.
/// </summary>
/// <remarks>
/// There are no overloads for finding descendants by a single search ID since then you should rather call FindDescendant (singular).
/// </remarks>
public static class HierarchyFindDescendantsExtensions
{
	#region FindDescendants
	/// <summary>
	/// Find nodes matching the search which are descendants of a node with the id.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The ID of node to search from.</param>
	/// <param name="search">The search criteria - IDs to match.</param>
	/// <param name="descend">Options for controlling how we descend the graph.</param>
	/// <returns>All matching descendant nodes.</returns>
	public static IList<Node<TItem>> FindDescendants<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, IEnumerable<TId> search, Descend? descend = null)
		where TId : notnull
		where TItem : notnull
	{
		var searchSet = new HashSet<TId>(search);
		return hierarchy
			.GetNode(id)
			.FindDescendants(n => searchSet.Contains(hierarchy.Identify(n.Item)), descend);
	}

	/// <summary>
	/// Find nodes matching the search predicate which are descendants of a node with the id.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The ID of node to search from.</param>
	/// <param name="search">The predicate function to match nodes.</param>
	/// <param name="descend">Options for controlling how we descend the graph.</param>
	/// <returns>All matching descendant nodes.</returns>
	public static IList<Node<TItem>> FindDescendants<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, Func<Node<TItem>, bool> search, Descend? descend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNode(id)
			.FindDescendants(search, descend);
	}

	/// <summary>
	/// Find nodes matching the search which are descendants of a node with one of the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The IDs of nodes to search from.</param>
	/// <param name="search">The search criteria - IDs to match.</param>
	/// <param name="descend">Options for controlling how we descend the graph.</param>
	/// <returns>All matching descendant nodes.</returns>
	public static IList<Node<TItem>> FindDescendants<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, IEnumerable<TId> search, Descend? descend = null)
		where TId : notnull
		where TItem : notnull
	{
		var searchSet = new HashSet<TId>(search);
		return hierarchy
			.GetNodes(ids)
			.FindDescendants(n => searchSet.Contains(hierarchy.Identify(n.Item)), descend);
	}

	/// <summary>
	/// Find nodes matching the search predicate which are descendants of a node with one of the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The IDs of nodes to search from.</param>
	/// <param name="search">The predicate function to match nodes.</param>
	/// <param name="descend">Options for controlling how we descend the graph.</param>
	/// <returns>All matching descendant nodes.</returns>
	public static IList<Node<TItem>> FindDescendants<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, Func<Node<TItem>, bool> search, Descend? descend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNodes(ids)
			.FindDescendants(search, descend);
	}
	#endregion

	#region FindDescendantItems
	/// <summary>
	/// Find items matching the search which are descendants of a node with the id.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The ID of node to search from.</param>
	/// <param name="search">The search criteria - IDs to match.</param>
	/// <param name="descend">Options for controlling how we descend the graph.</param>
	/// <returns>All matching descendant items.</returns>
	public static IList<TItem> FindDescendantItems<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, IEnumerable<TId> search, Descend? descend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.FindDescendants(id, search, descend).ToItems();
	}

	/// <summary>
	/// Find items matching the search predicate which are descendants of a node with the id.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The ID of node to search from.</param>
	/// <param name="search">The predicate function to match nodes.</param>
	/// <param name="descend">Options for controlling how we descend the graph.</param>
	/// <returns>All matching descendant items.</returns>
	public static IList<TItem> FindDescendantItems<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, Func<Node<TItem>, bool> search, Descend? descend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.FindDescendants(id, search, descend).ToItems();
	}

	/// <summary>
	/// Find items matching the search which are descendants of a node with one of the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The IDs of nodes to search from.</param>
	/// <param name="search">The search criteria - IDs to match.</param>
	/// <param name="descend">Options for controlling how we descend the graph.</param>
	/// <returns>All matching descendant items.</returns>
	public static IList<TItem> FindDescendantItems<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, IEnumerable<TId> search, Descend? descend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.FindDescendants(ids, search, descend).ToItems();
	}

	/// <summary>
	/// Find items matching the search predicate which are descendants of a node with one of the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The IDs of nodes to search from.</param>
	/// <param name="search">The predicate function to match nodes.</param>
	/// <param name="descend">Options for controlling how we descend the graph.</param>
	/// <returns>All matching descendant items.</returns>
	public static IList<TItem> FindDescendantItems<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, Func<Node<TItem>, bool> search, Descend? descend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.FindDescendants(ids, search, descend).ToItems();
	}
	#endregion

	#region FindDescendantIds
	/// <summary>
	/// Find IDs matching the search which are descendants of a node with the id.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The ID of node to search from.</param>
	/// <param name="search">The search criteria - IDs to match.</param>
	/// <param name="includeSelf">Whether to include the starting node in the search.</param>
	/// <param name="type">The traversal type (breadth-first or depth-first).</param>
	/// <returns>All matching descendant IDs.</returns>
	public static IList<TId> FindDescendantIds<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, IEnumerable<TId> search, Descend? descend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.FindDescendants(id, search, descend).ToIds(hierarchy.Identify);
	}

	/// <summary>
	/// Find IDs matching the search predicate which are descendants of a node with the id.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The ID of node to search from.</param>
	/// <param name="search">The predicate function to match nodes.</param>
	/// <param name="includeSelf">Whether to include the starting node in the search.</param>
	/// <param name="type">The traversal type (breadth-first or depth-first).</param>
	/// <returns>All matching descendant IDs.</returns>
	public static IList<TId> FindDescendantIds<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, Func<Node<TItem>, bool> search, Descend? descend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.FindDescendants(id, search, descend).ToIds(hierarchy.Identify);
	}

	/// <summary>
	/// Find IDs matching the search which are descendants of a node with one of the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The IDs of nodes to search from.</param>
	/// <param name="search">The search criteria - IDs to match.</param>
	/// <param name="descend">Options for controlling how we descend the graph.</param>
	/// <returns>All matching descendant IDs.</returns>
	public static IList<TId> FindDescendantIds<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, IEnumerable<TId> search, Descend? descend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.FindDescendants(ids, search, descend).ToIds(hierarchy.Identify);
	}

	/// <summary>
	/// Find IDs matching the search predicate which are descendants of a node with one of the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The IDs of nodes to search from.</param>
	/// <param name="search">The predicate function to match nodes.</param>
	/// <param name="descend">Options for controlling how we descend the graph.</param>
	/// <returns>All matching descendant IDs.</returns>
	public static IList<TId> FindDescendantIds<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, Func<Node<TItem>, bool> search, Descend? descend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.FindDescendants(ids, search, descend).ToIds(hierarchy.Identify);
	}
	#endregion
}
