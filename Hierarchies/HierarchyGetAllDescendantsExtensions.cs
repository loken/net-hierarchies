namespace Loken.Hierarchies;

/// <summary>
/// Extensions for retrieving descendants in a <see cref="Hierarchy{TItem,TId}"/>.
/// </summary>
public static class HierarchyGetAllDescendantsExtensions
{
	/// <summary>
	/// Get the chain of descendant nodes starting with the hierarchy roots.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="includeSelf">Whether to include the root nodes in the result.</param>
	/// <param name="type">The traversal type (breadth-first or depth-first).</param>
	/// <returns>An array of descendant nodes in specified order.</returns>
	public static IList<Node<TItem>> GetAllDescendants<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, bool includeSelf = false, TraversalType type = TraversalType.BreadthFirst)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.Roots
			.GetDescendants(includeSelf, type);
	}

	/// <summary>
	/// Get the items from the chain of descendant nodes starting with the hierarchy roots.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="includeSelf">Whether to include the root nodes in the result.</param>
	/// <param name="type">The traversal type (breadth-first or depth-first).</param>
	/// <returns>An array of descendant items in specified order.</returns>
	public static IList<TItem> GetAllDescendantItems<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, bool includeSelf = false, TraversalType type = TraversalType.BreadthFirst)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.Roots
			.GetDescendants(includeSelf, type)
			.ToItems();
	}

	/// <summary>
	/// Get the IDs from the chain of descendant nodes starting with the hierarchy roots.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="includeSelf">Whether to include the root nodes in the result.</param>
	/// <param name="type">The traversal type (breadth-first or depth-first).</param>
	/// <returns>An array of descendant IDs in specified order.</returns>
	public static IList<TId> GetAllDescendantIds<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, bool includeSelf = false, TraversalType type = TraversalType.BreadthFirst)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.Roots
			.GetDescendants(includeSelf, type)
			.ToIds(hierarchy.Identify);
	}
}
