namespace Loken.Hierarchies.Data.MongoDB;

public static class HierarchyMongoQueryExtensions
{
	public static IEnumerable<HierarchyRelation<TId>> GetChildRelations<TId>(this IMongoDatabase database, string concept)
		where TId : notnull
	{
		return database.ChildCollection<TId>().Find(r => r.Concept == concept).ToEnumerable();
	}

	public static IEnumerable<HierarchyRelation<TId>> GetDescendantRelations<TId>(this IMongoDatabase database, string concept)
		where TId : notnull
	{
		return database.DescendantCollection<TId>().Find(r => r.Concept == concept).ToEnumerable();
	}

	public static IEnumerable<HierarchyRelation<TId>> GetAncestorRelations<TId>(this IMongoDatabase database, string concept)
		where TId : notnull
	{
		return database.AncestorCollection<TId>().Find(r => r.Concept == concept).ToEnumerable();
	}
}
