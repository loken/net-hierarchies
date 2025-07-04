namespace Loken.Hierarchies;

/// <summary>
/// Extensions for mapping between different representations of relationships.
/// </summary>
public static class RelationMapExtensions
{
	/// <summary>
	/// Create a sequence of relations matching the <paramref name="childMap"/>.
	/// </summary>
	public static IEnumerable<(TId parent, TId child)> ToRelations<TId>(this MultiMap<TId> childMap)
		where TId : notnull
	{
		foreach (var (parent, children) in childMap)
		{
			foreach (var child in children)
				yield return (parent, child);
		}
	}

	/// <summary>
	/// Create a child-map matching the <paramref name="relations"/>.
	/// </summary>
	public static MultiMap<TId> ToChildMap<TId>(this IEnumerable<(TId parent, TId child)> relations)
		where TId : notnull
	{
		var map = new MultiMap<TId>();

		foreach (var (parent, child) in relations)
			map.LazySet(parent).Add(child);

		return map;
	}
}
