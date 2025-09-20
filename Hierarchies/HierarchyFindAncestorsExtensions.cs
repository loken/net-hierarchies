namespace Loken.Hierarchies;

/// <summary>
/// Extensions for finding and searching for multiple ancestors in a <see cref="Hierarchy{TItem,TId}"/>.
/// </summary>
/// <remarks>
/// There are no overloads for finding ancestors by a single search ID since then you should rather call FindAncestor (singular).
/// </remarks>
public static class HierarchyFindAncestorsExtensions
{
	#region FindAncestors
	/// <summary>
	/// Find nodes matching the search which are ancestors of a node with the id.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The ID of node to search from.</param>
	/// <param name="search">The search criteria - IDs to match.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>All matching ancestor nodes.</returns>
	public static IList<Node<TItem>> FindAncestors<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, IEnumerable<TId> search, Ascend? ascend = null)
		where TId : notnull
		where TItem : notnull
	{
		var searchSet = new HashSet<TId>(search);
		return hierarchy
			.GetNode(id)
			.FindAncestors(n => searchSet.Contains(hierarchy.Identify(n.Item)), ascend);
	}

	/// <summary>
	/// Find nodes matching the search predicate which are ancestors of a node with the id.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The ID of node to search from.</param>
	/// <param name="search">The predicate function to match nodes.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>All matching ancestor nodes.</returns>
	public static IList<Node<TItem>> FindAncestors<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, Func<Node<TItem>, bool> search, Ascend? ascend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNode(id)
			.FindAncestors(search, ascend);
	}

	/// <summary>
	/// Find nodes matching the search which are ancestors of a node with one of the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The IDs of nodes to search from.</param>
	/// <param name="search">The search criteria - IDs to match.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>All matching ancestor nodes.</returns>
	public static IList<Node<TItem>> FindAncestors<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, IEnumerable<TId> search, Ascend? ascend = null)
		where TId : notnull
		where TItem : notnull
	{
		var searchSet = new HashSet<TId>(search);
		return hierarchy
			.GetNodes(ids)
			.FindAncestors(n => searchSet.Contains(hierarchy.Identify(n.Item)), ascend);
	}

	/// <summary>
	/// Find nodes matching the search predicate which are ancestors of a node with one of the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The IDs of nodes to search from.</param>
	/// <param name="search">The predicate function to match nodes.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>All matching ancestor nodes.</returns>
	public static IList<Node<TItem>> FindAncestors<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, Func<Node<TItem>, bool> search, Ascend? ascend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNodes(ids)
			.FindAncestors(search, ascend);
	}
	#endregion

	#region FindAncestorItems
	/// <summary>
	/// Find items matching the search which are ancestors of a node with the id.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The ID of node to search from.</param>
	/// <param name="search">The search criteria - IDs to match.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>All matching ancestor items.</returns>
	public static IList<TItem> FindAncestorItems<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, IEnumerable<TId> search, Ascend? ascend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.FindAncestors(id, search, ascend).ToItems();
	}

	/// <summary>
	/// Find items matching the search predicate which are ancestors of a node with the id.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The ID of node to search from.</param>
	/// <param name="search">The predicate function to match nodes.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>All matching ancestor items.</returns>
	public static IList<TItem> FindAncestorItems<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, Func<Node<TItem>, bool> search, Ascend? ascend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.FindAncestors(id, search, ascend).ToItems();
	}

	/// <summary>
	/// Find items matching the search which are ancestors of a node with one of the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The IDs of nodes to search from.</param>
	/// <param name="search">The search criteria - IDs to match.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>All matching ancestor items.</returns>
	public static IList<TItem> FindAncestorItems<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, IEnumerable<TId> search, Ascend? ascend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.FindAncestors(ids, search, ascend).ToItems();
	}

	/// <summary>
	/// Find items matching the search predicate which are ancestors of a node with one of the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The IDs of nodes to search from.</param>
	/// <param name="search">The predicate function to match nodes.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>All matching ancestor items.</returns>
	public static IList<TItem> FindAncestorItems<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, Func<Node<TItem>, bool> search, Ascend? ascend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.FindAncestors(ids, search, ascend).ToItems();
	}
	#endregion

	#region FindAncestorIds
	/// <summary>
	/// Find IDs matching the search which are ancestors of a node with the id.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The ID of node to search from.</param>
	/// <param name="search">The search criteria - IDs to match.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>All matching ancestor IDs.</returns>
	public static IList<TId> FindAncestorIds<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, IEnumerable<TId> search, Ascend? ascend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.FindAncestors(id, search, ascend).ToIds(hierarchy.Identify);
	}

	/// <summary>
	/// Find IDs matching the search predicate which are ancestors of a node with the id.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The ID of node to search from.</param>
	/// <param name="search">The predicate function to match nodes.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>All matching ancestor IDs.</returns>
	public static IList<TId> FindAncestorIds<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, Func<Node<TItem>, bool> search, Ascend? ascend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.FindAncestors(id, search, ascend).ToIds(hierarchy.Identify);
	}

	/// <summary>
	/// Find IDs matching the search which are ancestors of a node with one of the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The IDs of nodes to search from.</param>
	/// <param name="search">The search criteria - IDs to match.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>All matching ancestor IDs.</returns>
	public static IList<TId> FindAncestorIds<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, IEnumerable<TId> search, Ascend? ascend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.FindAncestors(ids, search, ascend).ToIds(hierarchy.Identify);
	}

	/// <summary>
	/// Find IDs matching the search predicate which are ancestors of a node with one of the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The IDs of nodes to search from.</param>
	/// <param name="search">The predicate function to match nodes.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>All matching ancestor IDs.</returns>
	public static IList<TId> FindAncestorIds<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, Func<Node<TItem>, bool> search, Ascend? ascend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.FindAncestors(ids, search, ascend).ToIds(hierarchy.Identify);
	}
	#endregion

	#region Common Ancestors
	/// <summary>
	/// Find the common ancestor nodes for the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The identifiers for the items to find common ancestors for.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>All common ancestor nodes, or empty list if none found.</returns>
	public static IList<Node<TItem>> FindCommonAncestors<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, Ascend? ascend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNodes(ids)
			.FindCommonAncestors(ascend);
	}

	/// <summary>
	/// Find the items of common ancestor nodes for the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The identifiers for the items to find common ancestor items for.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>All common ancestor items, or empty list if none found.</returns>
	public static IList<TItem> FindCommonAncestorItems<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, Ascend? ascend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.FindCommonAncestors(ids, ascend).ToItems();
	}

	/// <summary>
	/// Find the IDs of ancestor nodes common to the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The identifiers for the items to find common ancestor IDs for.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>All common ancestor IDs, or empty list if none found.</returns>
	public static IList<TId> FindCommonAncestorIds<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, Ascend? ascend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.FindCommonAncestors(ids, ascend).ToIds(hierarchy.Identify);
	}
	#endregion
}
