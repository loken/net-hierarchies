namespace Loken.Hierarchies.Data.MongoDB;

public static class HierarchyMongoCollectionExtensions
{
	public static IMongoCollection<HierarchyRelation<TId>> ChildCollection<TId>(this IMongoDatabase database)
	where TId : notnull
	{
		return database.GetCollection<HierarchyRelation<TId>>(HierarchyTable.Children<TId>());
	}

	public static IMongoCollection<HierarchyRelation<TId>> DescendantCollection<TId>(this IMongoDatabase database)
		where TId : notnull
	{
		return database.GetCollection<HierarchyRelation<TId>>(HierarchyTable.Descendants<TId>());
	}

	public static IMongoCollection<HierarchyRelation<TId>> AncestorCollection<TId>(this IMongoDatabase database)
		where TId : notnull
	{
		return database.GetCollection<HierarchyRelation<TId>>(HierarchyTable.Ancestors<TId>());
	}
}
