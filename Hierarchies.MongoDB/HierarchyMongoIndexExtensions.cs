namespace Loken.Hierarchies.Data.MongoDB;

/// <summary>
/// Extensions for managing indexes for collections storing <see cref="HierarchyRelation{TId}"/>s.
/// </summary>
public static class HierarchyMongoIndexExtensions
{
	/// <summary>
	/// Create the indexes allowing us to optimally query the relations <paramref name="collection"/>.
	/// Will allow us to get a single relation for an ID, concept and relationship type,
	/// but also to fetch all relations of a given type and/or concept across IDs.
	/// </summary>
	public static IMongoCollection<HierarchyRelation<TId>> CreateRelationIndexes<TId>(this IMongoCollection<HierarchyRelation<TId>> collection, CreateIndexOptions? options = default)
		where TId : notnull
	{
		var ik = Builders<HierarchyRelation<TId>>.IndexKeys;
		options ??= new CreateIndexOptions();

		options.Name = "Id_Concept_Type";
		collection.Indexes.CreateOne(new CreateIndexModel<HierarchyRelation<TId>>(
			ik.Combine(
				ik.Ascending(rel => rel.Id),
				ik.Ascending(rel => rel.Concept),
				ik.Ascending(rel => rel.Type)),
			options));

		options.Name = "Concept_Type_Id";
		collection.Indexes.CreateOne(new CreateIndexModel<HierarchyRelation<TId>>(
			ik.Combine(
				ik.Ascending(rel => rel.Concept),
				ik.Ascending(rel => rel.Type),
				ik.Ascending(rel => rel.Id)),
			options));

		options.Name = "Type_Concept_Id";
		collection.Indexes.CreateOne(new CreateIndexModel<HierarchyRelation<TId>>(
			ik.Combine(
				ik.Ascending(rel => rel.Type),
				ik.Ascending(rel => rel.Concept),
				ik.Ascending(rel => rel.Id)),
			options));

		return collection;
	}
}
