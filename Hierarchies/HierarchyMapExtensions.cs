namespace Loken.Hierarchies;

/// <summary>
/// Extensions for mapping between different representations of relationships in a <see cref="Hierarchy{TItem, TId}"/>.
/// </summary>
public static class HierarchyMapExtensions
{
	/// <summary>
	/// Create a sequence of relations matching the <paramref name="hierarchy"/>.
	/// </summary>
	public static IEnumerable<Relation<TId>> ToRelations<TItem, TId>(this Hierarchy<TItem, TId> hierarchy)
		where TItem : notnull
		where TId : notnull
	{
		return hierarchy.ToChildMap().ToRelations();
	}

	/// <summary>
	/// Create a child-map matching the <paramref name="hierarchy"/>.
	/// </summary>
	public static MultiMap<TId> ToMap<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, RelType type)
		where TItem : notnull
		where TId : notnull
	{
		return hierarchy.Roots.ToMap(hierarchy.Identify, type);
	}

	/// <summary>
	/// Create a child-map matching the <paramref name="hierarchy"/>.
	/// </summary>
	public static MultiMap<TId> ToChildMap<TItem, TId>(this Hierarchy<TItem, TId> hierarchy)
		where TItem : notnull
		where TId : notnull
	{
		return hierarchy.Roots.ToChildMap(hierarchy.Identify);
	}

	/// <summary>
	/// Create a descendant-map matching the <paramref name="hierarchy"/>.
	/// </summary>
	public static MultiMap<TId> ToDescendantMap<TItem, TId>(this Hierarchy<TItem, TId> hierarchy)
		where TItem : notnull
		where TId : notnull
	{
		return hierarchy.Roots.ToDescendantMap(hierarchy.Identify);
	}

	/// <summary>
	/// Create a ancestor-map matching the <paramref name="hierarchy"/>.
	/// </summary>
	public static MultiMap<TId> ToAncestorMap<TItem, TId>(this Hierarchy<TItem, TId> hierarchy)
		where TItem : notnull
		where TId : notnull
	{
		return hierarchy.Roots.ToAncestorMap(hierarchy.Identify);
	}
}
