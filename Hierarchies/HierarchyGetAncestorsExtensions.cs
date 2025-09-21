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
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>An array of ancestor nodes in order from nearest to farthest.</returns>
	public static IList<Node<TItem>> GetAncestors<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, Ascend? ascend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNode(id)
			.GetAncestors(ascend);
	}

	/// <summary>
	/// Get the chain of ancestor nodes starting with the nodes for the items matching the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The identifiers for the items to start traversal from.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>An array of ancestor nodes in order from nearest to farthest.</returns>
	public static IList<Node<TItem>> GetAncestors<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, Ascend? ascend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNodes(ids)
			.GetAncestors(ascend);
	}
	#endregion

	#region GetAncestorItems
	/// <summary>
	/// Get the items from the chain of ancestor nodes for the item matching the id.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The identifier for the item to start traversal from.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>An array of ancestor items in order from nearest to farthest.</returns>
	public static IList<TItem> GetAncestorItems<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, Ascend? ascend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNode(id)
			.GetAncestors(ascend)
			.ToItems();
	}

	/// <summary>
	/// Get the items from the chain of ancestor nodes starting with the nodes for the items matching the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The identifiers for the items to start traversal from.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>An array of ancestor items in order from nearest to farthest.</returns>
	public static IList<TItem> GetAncestorItems<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, Ascend? ascend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNodes(ids)
			.GetAncestors(ascend)
			.ToItems();
	}
	#endregion

	#region GetAncestorIds
	/// <summary>
	/// Get the IDs from the chain of ancestor nodes for the item matching the id.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The identifier for the item to start traversal from.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>An array of ancestor IDs in order from nearest to farthest.</returns>
	public static IList<TId> GetAncestorIds<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, Ascend? ascend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNode(id)
			.GetAncestors(ascend)
			.ToIds(hierarchy.Identify);
	}

	/// <summary>
	/// Get the IDs from the chain of ancestor nodes starting with the nodes for the items matching the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The identifiers for the items to start traversal from.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>An array of ancestor IDs in order from nearest to farthest.</returns>
	public static IList<TId> GetAncestorIds<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, Ascend? ascend = null)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNodes(ids)
			.GetAncestors(ascend)
			.ToIds(hierarchy.Identify);
	}
	#endregion
}
