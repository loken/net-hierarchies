namespace Loken.Hierarchies.Data.MongoDB;

/// <summary>
/// Extensions for querying 
/// </summary>
public static class HierarchyMongoQueryExtensions
{
	public static IEnumerable<HierarchyRelation<TId>> GetRelations<TId>(this IMongoCollection<HierarchyRelation<TId>> collection, string concept, RelType type)
		where TId : notnull
	{
		Rel.AssertHasSpecific(type);

		var types = Rel.GetSpecific(type).ToArray();
		return collection
			.Find(r => r.Concept == concept && types.Contains(r.Type))
			.ToEnumerable();
	}

	public static HierarchyRelation<TId>? GetRelation<TId>(this IMongoCollection<HierarchyRelation<TId>> collection, string concept, TId id, RelType type = RelType.All)
		where TId : notnull
	{
		Rel.AssertHasSpecific(type);

		var types = Rel.GetSpecific(type).ToArray();
		return collection
			.Find(r => r.Concept == concept && types.Contains(r.Type) && r.Id.Equals(id))
			.FirstOrDefault();
	}

	public static ISet<TId> GetRelationTargets<TId>(this IMongoCollection<HierarchyRelation<TId>> collection, string concept, TId id, RelType type)
		where TId : notnull
	{
		Rel.AssertIsSpecific(type);

		var res = collection
			.Find(r => r.Concept == concept && r.Type == type && r.Id.Equals(id))
			.FirstOrDefault();
		return res?.Targets ?? new HashSet<TId>();
	}

	public static ISet<TId> GetChildren<TId>(this IMongoCollection<HierarchyRelation<TId>> collection, string concept, TId id)
		where TId : notnull
	{
		return collection.GetRelationTargets(concept, id, RelType.Children);
	}

	public static ISet<TId> GetDescendants<TId>(this IMongoCollection<HierarchyRelation<TId>> collection, string concept, TId id)
		where TId : notnull
	{
		return collection.GetRelationTargets(concept, id, RelType.Descendants);
	}

	public static ISet<TId> GetAncestors<TId>(this IMongoCollection<HierarchyRelation<TId>> collection, string concept, TId id)
		where TId : notnull
	{
		return collection.GetRelationTargets(concept, id, RelType.Ancestors);
	}
}
