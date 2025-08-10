namespace Loken.Hierarchies;

/// <summary>
/// Extensions for traversing descendants of a <see cref="Hierarchy{TItem,TId}"/>.
/// </summary>
public static class HierarchyDescendantExtensions
{
	/// <summary>
	/// Recursively get <see cref="Node{T}.Children"/> of the <see cref="Node{T}"/> for the <typeparamref name="TItem"/> matching the <paramref name="id"/>.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The identifier for the <typeparamref name="TItem"/> to start from.</param>
	/// <param name="includeSelf">Include the <see cref="Node{T}"/> with a <typeparamref name="TItem"/> matching the <paramref name="id"/>?</param>
	public static IEnumerable<Node<TItem>> GetDescendantNodes<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, bool includeSelf = false, TraversalType type = TraversalType.BreadthFirst)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.GetNode(id).GetDescendants(includeSelf, type);
	}

	/// <summary>
	/// Recursively get the <typeparamref name="TItem"/>s of the <see cref="Node{T}.Children"/> for the <typeparamref name="TItem"/> matching the <paramref name="id"/>.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The identifier for the <typeparamref name="TItem"/> to start from.</param>
	/// <param name="includeSelf">Include the <typeparamref name="TItem"/> matching the <paramref name="id"/>?</param>
	public static IEnumerable<TItem> GetDescendants<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, bool includeSelf = false, TraversalType type = TraversalType.BreadthFirst)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.GetDescendantNodes(id, includeSelf, type).ToItems();
		;
	}

	/// <summary>
	/// Recursively get the <typeparamref name="TId"/>s of the <see cref="Node{T}.Children"/> for the <typeparamref name="TItem"/> matching the <paramref name="id"/>.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The identifier for the <typeparamref name="TItem"/> to start from.</param>
	/// <param name="includeSelf">Include the <paramref name="id"/>?</param>
	public static IEnumerable<TId> GetDescendantIds<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, bool includeSelf = false, TraversalType type = TraversalType.BreadthFirst)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.GetDescendantNodes(id, includeSelf, type).ToIds(hierarchy.Identify);
	}
}
