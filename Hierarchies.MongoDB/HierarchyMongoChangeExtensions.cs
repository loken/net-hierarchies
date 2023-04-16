namespace Loken.Hierarchies.Data.MongoDB;

public static class HierarchyMongoChangeExtensions
{
	public static void InsertRelations<TItem, TId>(this IMongoDatabase database, Hierarchy<TItem, TId> hierarchy, string concept, RelType type = RelType.All)
		where TItem : notnull
		where TId : notnull
	{
		if (type.HasFlag(RelType.Children))
			database.ChildCollection<TId>().InsertMany(hierarchy.ToChildRelations(concept));

		if (type.HasFlag(RelType.Descendants))
			database.DescendantCollection<TId>().InsertMany(hierarchy.ToDescendantRelations(concept));

		if (type.HasFlag(RelType.Ancestors))
			database.AncestorCollection<TId>().InsertMany(hierarchy.ToAncestorRelations(concept));
	}

	public static void ClearRelations<TId>(this IMongoDatabase database, string concept, RelType type = RelType.All)
		where TId : notnull
	{
		if (type.HasFlag(RelType.Children))
			database.ChildCollection<TId>().DeleteMany(rel => rel.Concept == concept);

		if (type.HasFlag(RelType.Descendants))
			database.DescendantCollection<TId>().DeleteMany(rel => rel.Concept == concept);

		if (type.HasFlag(RelType.Ancestors))
			database.AncestorCollection<TId>().DeleteMany(rel => rel.Concept == concept);
	}
}
