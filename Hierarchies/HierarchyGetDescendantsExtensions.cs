namespace Loken.Hierarchies;

/// <summary>
/// Extensions for retrieving descendants in a <see cref="Hierarchy{TItem,TId}"/>.
/// </summary>
public static class HierarchyGetDescendantsExtensions
{
	#region GetDescendants
	/// <summary>
	/// Get the chain of descendant nodes starting with the node for the item matching the id.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The identifier for the item to start traversal from.</param>
	/// <param name="includeSelf">Whether to include the starting node in the result.</param>
	/// <param name="type">The traversal type (breadth-first or depth-first).</param>
	/// <returns>An array of descendant nodes in specified order.</returns>
	public static IList<Node<TItem>> GetDescendants<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, bool includeSelf = false, TraversalType type = TraversalType.BreadthFirst)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.GetDescendants([id], includeSelf, type);
	}

	/// <summary>
	/// Get the chain of descendant nodes starting with the nodes for the items matching the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The identifiers for the items to start traversal from.</param>
	/// <param name="includeSelf">Whether to include the starting nodes in the result.</param>
	/// <param name="type">The traversal type (breadth-first or depth-first).</param>
	/// <returns>An array of descendant nodes in specified order.</returns>
	public static IList<Node<TItem>> GetDescendants<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, bool includeSelf = false, TraversalType type = TraversalType.BreadthFirst)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy
			.GetNodes(ids)
			.GetDescendants(includeSelf, type);
	}
	#endregion

	#region GetDescendantItems
	/// <summary>
	/// Get the items from the chain of descendant nodes starting with the node for the item matching the id.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The identifier for the item to start traversal from.</param>
	/// <param name="includeSelf">Whether to include the starting node in the result.</param>
	/// <param name="type">The traversal type (breadth-first or depth-first).</param>
	/// <returns>An array of descendant items in specified order.</returns>
	public static IList<TItem> GetDescendantItems<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, bool includeSelf = false, TraversalType type = TraversalType.BreadthFirst)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.GetDescendants(id, includeSelf, type).ToItems();
	}

	/// <summary>
	/// Get the items from the chain of descendant nodes starting with the nodes for the items matching the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The identifiers for the items to start traversal from.</param>
	/// <param name="includeSelf">Whether to include the starting nodes in the result.</param>
	/// <param name="type">The traversal type (breadth-first or depth-first).</param>
	/// <returns>An array of descendant items in specified order.</returns>
	public static IList<TItem> GetDescendantItems<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, bool includeSelf = false, TraversalType type = TraversalType.BreadthFirst)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.GetDescendants(ids, includeSelf, type).ToItems();
	}
	#endregion

	#region GetDescendantIds
	/// <summary>
	/// Get the IDs from the chain of descendant nodes starting with the node for the item matching the id.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The identifier for the item to start traversal from.</param>
	/// <param name="includeSelf">Whether to include the starting node in the result.</param>
	/// <param name="type">The traversal type (breadth-first or depth-first).</param>
	/// <returns>An array of descendant IDs in specified order.</returns>
	public static IList<TId> GetDescendantIds<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, bool includeSelf = false, TraversalType type = TraversalType.BreadthFirst)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.GetDescendants(id, includeSelf, type).ToIds(hierarchy.Identify);
	}

	/// <summary>
	/// Get the IDs from the chain of descendant nodes starting with the nodes for the items matching the ids.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="ids">The identifiers for the items to start traversal from.</param>
	/// <param name="includeSelf">Whether to include the starting nodes in the result.</param>
	/// <param name="type">The traversal type (breadth-first or depth-first).</param>
	/// <returns>An array of descendant IDs in specified order.</returns>
	public static IList<TId> GetDescendantIds<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids, bool includeSelf = false, TraversalType type = TraversalType.BreadthFirst)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.GetDescendants(ids, includeSelf, type).ToIds(hierarchy.Identify);
	}
	#endregion
}
