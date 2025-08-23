namespace Loken.Hierarchies.Data.MongoDB;

/// <summary>
/// Extensions for querying a collection for hierarchy relations.
/// </summary>
public static class HierarchyMongoQueryExtensions
{
	/// <summary>
	/// Get relations for a <paramref name="concept"/> and one or more <paramref name="types"/>.
	/// </summary>
	public static IList<HierarchyRelation<TId>> GetRelations<TId>(this IMongoCollection<HierarchyRelation<TId>> collection, string concept, RelType types)
		where TId : notnull
	{
		Rel.AssertHasSpecific(types);

		var specific = Rel.GetSpecific(types).ToArray();
		return collection
			.Find(r => r.Concept == concept && specific.Contains(r.Type))
			.ToList();
	}

	/// <summary>
	/// Get the relation for a <paramref name="concept"/>, <paramref name="id"/> and "specific" <paramref name="type"/>.
	/// </summary>
	public static HierarchyRelation<TId>? GetRelation<TId>(this IMongoCollection<HierarchyRelation<TId>> collection, string concept, TId id, RelType type)
		where TId : notnull
	{
		Rel.AssertIsSpecific(type);

		return collection
			.Find(r => r.Concept == concept && r.Type == type && r.Id.Equals(id))
			.FirstOrDefault();
	}

	/// <summary>
	/// Get the relation targets for a <paramref name="concept"/>, <paramref name="id"/> and "specific" <paramref name="type"/>.
	/// </summary>
	public static ISet<TId> GetRelationTargets<TId>(this IMongoCollection<HierarchyRelation<TId>> collection, string concept, TId id, RelType type)
		where TId : notnull
	{
		var res = collection.GetRelation(concept, id, type);
		return res?.Targets ?? new HashSet<TId>();
	}

	/// <summary>
	/// Get the child IDs for a <paramref name="concept"/>, <paramref name="id"/> and "specific" <paramref name="type"/>.
	/// </summary>
	public static ISet<TId> GetChildren<TId>(this IMongoCollection<HierarchyRelation<TId>> collection, string concept, TId id)
		where TId : notnull
	{
		return collection.GetRelationTargets(concept, id, RelType.Children);
	}

	/// <summary>
	/// Get the descendant IDs for a <paramref name="concept"/>, <paramref name="id"/> and "specific" <paramref name="type"/>.
	/// </summary>
	public static ISet<TId> GetDescendants<TId>(this IMongoCollection<HierarchyRelation<TId>> collection, string concept, TId id)
		where TId : notnull
	{
		return collection.GetRelationTargets(concept, id, RelType.Descendants);
	}

	/// <summary>
	/// Get the ancestor IDs for a <paramref name="concept"/>, <paramref name="id"/> and "specific" <paramref name="type"/>.
	/// </summary>
	public static ISet<TId> GetAncestors<TId>(this IMongoCollection<HierarchyRelation<TId>> collection, string concept, TId id)
		where TId : notnull
	{
		return collection.GetRelationTargets(concept, id, RelType.Ancestors);
	}

	/// <summary>
	/// Create a hierarchy of IDs for the <paramref name="concept"/> mapped from the stored child relations.
	/// </summary>
	public static Hierarchy<TId> ReadHierarchy<TId>(this IMongoCollection<HierarchyRelation<TId>> collection, string concept)
		where TId : notnull
	{
		var childMap = collection.GetRelations(concept, RelType.Children).ToMap();
		return Hierarchies.CreateFromChildMap(childMap);
	}
}
