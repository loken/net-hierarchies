namespace Loken.Hierarchies.Data.MongoDB;

public static class HierarchyMongoIndexExtensions
{
	public static void CreateRelationIndex<TId>(this IMongoCollection<HierarchyRelation<TId>> collection)
		where TId : notnull
	{
		var ik = Builders<HierarchyRelation<TId>>.IndexKeys;
		collection.Indexes.CreateOne(new CreateIndexModel<HierarchyRelation<TId>>(
			ik.Combine(
				ik.Ascending(rel => rel.Concept),
				ik.Ascending(rel => rel.Id))));
	}

	public static void CreateRelationIndexes<TId>(this IMongoDatabase database)
		where TId : notnull
	{
		database.ChildCollection<TId>().CreateRelationIndex();
		database.DescendantCollection<TId>().CreateRelationIndex();
		database.AncestorCollection<TId>().CreateRelationIndex();
	}
}
