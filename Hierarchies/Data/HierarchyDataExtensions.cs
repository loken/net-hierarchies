namespace Loken.Hierarchies.Data;

public static class HierarchyDataExtensions
{
	/// <summary>
	/// Create a <see cref="Hierarchy{TId}"/> from the <paramref name="relations"/>.
	/// </summary>
	public static Hierarchy<TId> ToHierarchy<TId>(this IEnumerable<HierarchyRelation<TId>> relations)
		where TId : notnull
	{
		return Hierarchy.CreateMapped(relations.ToMap());
	}

	/// <summary>
	/// Create a multi-map from the <paramref name="relations"/>.
	/// </summary>
	public static MultiMap<TId> ToMap<TId>(this IEnumerable<HierarchyRelation<TId>> relations)
		where TId : notnull
	{
		var map = new MultiMap<TId>();

		foreach (var relation in relations)
		{
			if (relation.Targets == null || !relation.Targets.Any())
				map.Add(relation.Id);
			else
				map.Add(relation.Id, relation.Targets);
		}

		return map;
	}

	/// <summary>
	/// Create <see cref="HierarchyRelation{TId}"/>s from the <paramref name="multiMap"/> and <paramref name="concept"/>.
	/// </summary>
	public static IEnumerable<HierarchyRelation<TId>> ToRelations<TId>(this MultiMap<TId> multiMap, string concept, RelType type)
		where TId : notnull
	{
		return multiMap.Select(x => new HierarchyRelation<TId>
		{
			Concept = concept,
			Type = type,
			Id = x.Key,
			Targets = x.Value,
		});
	}

	/// <summary>
	/// Create a sequence of <see cref="HierarchyRelation{TId}"/>s for the node-to-chilren relations
	/// of the <paramref name="hierarchy"/> and <paramref name="concept"/>.
	/// </summary>
	public static IEnumerable<HierarchyRelation<TId>> ToRelations<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, string concept, RelType type)
		where TItem : notnull
		where TId : notnull
	{
		return hierarchy.ToMap(type).ToRelations(concept, type);
	}

	/// <summary>
	/// Create a sequence of <see cref="HierarchyRelation{TId}"/>s for the node-to-chilren relations
	/// of the <paramref name="hierarchy"/> and <paramref name="concept"/>.
	/// </summary>
	public static IEnumerable<HierarchyRelation<TId>> ToChildRelations<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, string concept)
		where TItem : notnull
		where TId : notnull
	{
		return hierarchy.ToChildMap().ToRelations(concept, RelType.Children);
	}

	/// <summary>
	/// Create a sequence of <see cref="HierarchyRelation{TId}"/>s for the node-to-descendants relations
	/// of the <paramref name="hierarchy"/> and <paramref name="concept"/>.
	/// </summary>
	public static IEnumerable<HierarchyRelation<TId>> ToDescendantRelations<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, string concept)
		where TItem : notnull
		where TId : notnull
	{
		return hierarchy.ToDescendantMap().ToRelations(concept, RelType.Descendants);
	}

	/// <summary>
	/// Create a sequence of <see cref="HierarchyRelation{TId}"/>s for the node-to-ancestors relations
	/// of the <paramref name="hierarchy"/> and <paramref name="concept"/>.
	/// </summary>
	public static IEnumerable<HierarchyRelation<TId>> ToAncestorRelations<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, string concept)
		where TItem : notnull
		where TId : notnull
	{
		return hierarchy.ToAncestorMap().ToRelations(concept, RelType.Ancestors);
	}
}
