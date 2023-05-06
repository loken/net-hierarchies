namespace Loken.Hierarchies.Data.MongoDB;

/// <summary>
/// Extensions for accessing mongo collections for <see cref="HierarchyRelation{TId}"/>s.
/// </summary>
public static class HierarchyMongoCollectionExtensions
{
	/// <summary>
	/// Get the <see cref="IMongoCollection{TDocument}"/> for relations of a certain type of <typeparamref name="TId"/>.
	/// </summary>
	public static IMongoCollection<HierarchyRelation<TId>> GetHierarchies<TId>(this IMongoDatabase database)
		where TId : notnull
	{
		return database.GetCollection<HierarchyRelation<TId>>(HierarchyName.For<TId>());
	}
}
