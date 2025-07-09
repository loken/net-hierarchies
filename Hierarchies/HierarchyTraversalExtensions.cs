namespace Loken.Hierarchies;

/// <summary>
/// Extensions for traversing a <see cref="Hierarchy{TItem,TId}"/>.
/// </summary>
public static class HierarchyTraversalExtensions
{
	/// <summary>
	/// Get the chain of <see cref="Node{T}.Parent"/>s starting with the <see cref="Node{T}"/> for the <typeparamref name="TItem"/> matching the <paramref name="id"/>.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The identifier for the <typeparamref name="TItem"/> to start from.</param>
	/// <param name="includeSelf">Include the <see cref="Node{T}"/> with a <typeparamref name="TItem"/> matching the <paramref name="id"/>?</param>
	public static IEnumerable<Node<TItem>> GetAncestorNodes<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, bool includeSelf = false)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.GetNode(id).GetAncestors(includeSelf);
	}

	/// <summary>
	/// Get the <typeparamref name="TItem"/>s from the chain of <see cref="Node{T}.Parent"/>s for the <typeparamref name="TItem"/> matching the <paramref name="id"/>.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The identifier for the <typeparamref name="TItem"/> to start from.</param>
	/// <param name="includeSelf">Include the <typeparamref name="TItem"/> matching the <paramref name="id"/>?</param>
	public static IEnumerable<TItem> GetAncestors<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, bool includeSelf = false)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.GetAncestorNodes(id, includeSelf).ToItems();
		;
	}

	/// <summary>
	/// Get the <typeparamref name="TId"/>s from the chain of <see cref="Node{T}.Parent"/>s for the <typeparamref name="TItem"/> matching the <paramref name="id"/>.
	/// </summary>
	/// <param name="hierarchy">The <see cref="Hierarchy{TItem,TId}"/> to search.</param>
	/// <param name="id">The identifier for the <typeparamref name="TItem"/> to start from.</param>
	/// <param name="includeSelf">Include the <paramref name="id"/>?</param>
	public static IEnumerable<TId> GetAncestorIds<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id, bool includeSelf = false)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.GetAncestorNodes(id, includeSelf).ToIds(hierarchy.Identify);
	}

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