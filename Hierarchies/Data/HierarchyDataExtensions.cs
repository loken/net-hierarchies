﻿namespace Loken.Hierarchies.Data;

public static class HierarchyDataExtensions
{
	/// <summary>
	/// Create a <see cref="Hierarchy{TId}"/> from the <paramref name="relations"/>.
	/// </summary>
	public static Hierarchy<TId> ToHierarchy<TId>(this IEnumerable<HierarchyRelation<TId>> relations)
		where TId : notnull
	{
		return Hierarchy.CreateMapped(relations.ToMultiMap());
	}

	/// <summary>
	/// Create a multi-map from the <paramref name="relations"/>.
	/// </summary>
	public static IDictionary<TId, ISet<TId>> ToMultiMap<TId>(this IEnumerable<HierarchyRelation<TId>> relations)
		where TId : notnull
	{
		return relations.ToDictionary(r => r.Id, r => r.Targets);
	}

	/// <summary>
	/// Create <see cref="HierarchyRelation{TId}"/>s from the <paramref name="multiMap"/> and <paramref name="concept"/>.
	/// </summary>
	public static IEnumerable<HierarchyRelation<TId>> ToRelations<TId>(this IDictionary<TId, ISet<TId>> multiMap, string concept)
		where TId : notnull
	{
		return multiMap.Select(x => new HierarchyRelation<TId>
		{
			Concept = concept,
			Id = x.Key,
			Targets = x.Value,
		});
	}

	/// <summary>
	/// Create a sequence of <see cref="HierarchyRelation{TId}"/>s for the node-to-chilren relations
	/// of the <paramref name="hierarchy"/> and <paramref name="concept"/>.
	/// </summary>
	public static IEnumerable<HierarchyRelation<TId>> ToChildRelations<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, string concept)
		where TItem : notnull
		where TId : notnull
	{
		return hierarchy.ToChildMap().ToRelations(concept);
	}

	/// <summary>
	/// Create a sequence of <see cref="HierarchyRelation{TId}"/>s for the node-to-descendants relations
	/// of the <paramref name="hierarchy"/> and <paramref name="concept"/>.
	/// </summary>
	public static IEnumerable<HierarchyRelation<TId>> ToDescendantRelations<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, string concept)
		where TItem : notnull
		where TId : notnull
	{
		return hierarchy.ToDescendantMap().ToRelations(concept);
	}

	/// <summary>
	/// Create a sequence of <see cref="HierarchyRelation{TId}"/>s for the node-to-ancestors relations
	/// of the <paramref name="hierarchy"/> and <paramref name="concept"/>.
	/// </summary>
	public static IEnumerable<HierarchyRelation<TId>> ToAncestorRelations<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, string concept)
		where TItem : notnull
		where TId : notnull
	{
		return hierarchy.ToAncestorMap().ToRelations(concept);
	}
}
