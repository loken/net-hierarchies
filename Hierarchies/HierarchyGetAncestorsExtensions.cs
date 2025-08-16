namespace Loken.Hierarchies;

/// <summary>
/// Extensions for retrieving ancestors in a <see cref="Hierarchy{TItem,TId}"/>.
/// </summary>
public static class HierarchyGetAncestorsExtensions
{
	#region GetAncestors
	/// <summary>
	/// Get the chain of ancestor nodes for the item matching the id.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The identifier for the item to start traversal from.</param>
	/// <param name="includeSelf">Whether to include the starting node in the result.</param>
	/// <returns>An array of ancestor nodes in order from immediate parent to root.</returns>
	public static IList<Node<TItem>> GetAncestors<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, bool includeSelf = false)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNode(id)
			.GetAncestors(includeSelf);
	}

	/// <summary>
	/// Get the chain of ancestor nodes starting with the nodes for the items matching the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The identifiers for the items to start traversal from.</param>
	/// <param name="includeSelf">Whether to include the starting nodes in the result.</param>
	/// <returns>An array of ancestor nodes in order from immediate parent to root.</returns>
	public static IList<Node<TItem>> GetAncestors<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, bool includeSelf = false)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNodes(ids)
			.GetAncestors(includeSelf);
	}
	#endregion

	#region GetAncestorItems
	/// <summary>
	/// Get the items from the chain of ancestor nodes for the item matching the id.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The identifier for the item to start traversal from.</param>
	/// <param name="includeSelf">Whether to include the starting node in the result.</param>
	/// <returns>An array of ancestor items in order from immediate parent to root.</returns>
	public static IList<TItem> GetAncestorItems<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, bool includeSelf = false)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNode(id)
			.GetAncestors(includeSelf)
			.ToItems();
	}

	/// <summary>
	/// Get the items from the chain of ancestor nodes starting with the nodes for the items matching the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The identifiers for the items to start traversal from.</param>
	/// <param name="includeSelf">Whether to include the starting nodes in the result.</param>
	/// <returns>An array of ancestor items in order from immediate parent to root.</returns>
	public static IList<TItem> GetAncestorItems<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, bool includeSelf = false)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNodes(ids)
			.GetAncestors(includeSelf)
			.ToItems();
	}
	#endregion

	#region GetAncestorIds
	/// <summary>
	/// Get the IDs from the chain of ancestor nodes for the item matching the id.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The identifier for the item to start traversal from.</param>
	/// <param name="includeSelf">Whether to include the starting node in the result.</param>
	/// <returns>An array of ancestor IDs in order from immediate parent to root.</returns>
	public static IList<TId> GetAncestorIds<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, bool includeSelf = false)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNode(id)
			.GetAncestors(includeSelf)
			.ToIds(hierarchy.Identify);
	}

	/// <summary>
	/// Get the IDs from the chain of ancestor nodes starting with the nodes for the items matching the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The identifiers for the items to start traversal from.</param>
	/// <param name="includeSelf">Whether to include the starting nodes in the result.</param>
	/// <returns>An array of ancestor IDs in order from immediate parent to root.</returns>
	public static IList<TId> GetAncestorIds<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, bool includeSelf = false)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNodes(ids)
			.GetAncestors(includeSelf)
			.ToIds(hierarchy.Identify);
	}
	#endregion
}
