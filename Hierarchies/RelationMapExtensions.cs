namespace Loken.Hierarchies;

/// <summary>
/// Extensions for mapping between relations and maps.
/// </summary>
public static class RelationMapExtensions
{
	/// <summary>
	/// Create a sequence of relations matching the <paramref name="childMap"/>.
	/// </summary>
	public static IEnumerable<Relation<TId>> ToRelations<TId>(this MultiMap<TId> childMap)
		where TId : notnull
	{
		foreach (var (parent, children) in childMap)
		{
			if (children.Count == 0)
			{
				yield return new(parent);
			}
			else
			{
				foreach (var child in children)
					yield return new(parent, child);
			}
		}
	}

	/// <summary>
	/// Create a child-map matching the <paramref name="relations"/>.
	/// </summary>
	public static MultiMap<TId> ToChildMap<TId>(this IEnumerable<Relation<TId>> relations)
		where TId : notnull
	{
		var map = new MultiMap<TId>();

		foreach (var relation in relations)
		{
			if (relation.ParentOnly)
				map.Add(relation.Parent);
			else
				map.Add(relation.Parent, relation.Child);
		}

		return map;
	}
}
