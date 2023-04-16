namespace Loken.Hierarchies;

/// <summary>
/// Extensions for managing how the <see cref="Node{TItem}"/>s of a <see cref="Hierarchy{TItem, TId}"/> are linked,
/// meaning their parent-child-relations.
/// </summary>
public static class HierarchyLinkingExtensions
{
	/// <summary>
	/// Attach the provided <paramref name="roots"/> to the <paramref name="hierarchy"/>.
	/// </summary>
	/// <param name="hierarchy">Hierarchy to attach to.</param>
	/// <param name="roots">Nodes to attach.</param>
	/// <returns>The hierarchy itself for method chaining purposes.</returns>
	/// <exception cref="InvalidOperationException">
	/// Must provide non-empty set of <paramref name="roots"/> of a compatible brand
	/// that aren't already attached to another parent.
	/// </exception>
	public static Hierarchy<TItem, TId> Attach<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<Node<TItem>> roots)
		where TItem : notnull
		where TId : notnull
	{
		return hierarchy.Attach(roots.ToArray());
	}

	/// <summary>
	/// Attach the provided <paramref name="children"/> to the node of the
	/// provided <paramref name="parentId"/> which is in the <paramref name="hierarchy"/>.
	/// </summary>
	/// <param name="hierarchy">Hierarchy to attach to.</param>
	/// <param name="parentId">The id of the node to attach to.</param>
	/// <param name="children">Nodes to attach.</param>
	/// <returns>The hierarchy itself for method chaining purposes.</returns>
	/// <exception cref="InvalidOperationException">
	/// Must provide non-empty set of <paramref name="children"/> of a compatible brand
	/// that aren't already attached to another parent.
	/// </exception>
	public static Hierarchy<TItem, TId> Attach<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId parentId, IEnumerable<Node<TItem>> children)
		where TItem : notnull
		where TId : notnull
	{
		return hierarchy.Attach(parentId, children.ToArray());
	}

	/// <summary>
	/// Detach the provided <paramref name="nodes"/>.
	/// </summary>
	/// <param name="hierarchy">Hierarchy to detach from.</param>
	/// <param name="nodes">Nodes to detach.</param>
	/// <returns>The hierarchy itself for method chaining purposes.</returns>
	/// <exception cref="InvalidOperationException">
	/// Must provide non-empty set of attached <paramref name="nodes"/>.
	/// </exception>
	public static Hierarchy<TItem, TId> Detach<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<Node<TItem>> nodes)
		where TItem : notnull
		where TId : notnull
	{
		return hierarchy.Detach(nodes.ToArray());
	}
}
