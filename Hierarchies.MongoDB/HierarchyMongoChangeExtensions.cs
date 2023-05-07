namespace Loken.Hierarchies.Data.MongoDB;

/// <summary>
/// Extensions for managing how a <see cref="Hierarchy{TItem, TId}"/> is stored
/// as <see cref="HierarchyRelation{TId}"/>s in MongoDB.
/// </summary>
public static class HierarchyMongoChangeExtensions
{
	/// <summary>
	/// Insert relations for each provided <see cref="RelType"/> from the <paramref name="hierarchy"/>
	/// and the <paramref name="concept"/> it represents.
	/// </summary>
	public static void InsertRelations<TItem, TId>(this IMongoCollection<HierarchyRelation<TId>> collection, Hierarchy<TItem, TId> hierarchy, string concept, RelType types = RelType.All)
		where TItem : notnull
		where TId : notnull
	{
		foreach (var type in Rel.GetSpecific(types))
			collection.InsertMany(hierarchy.ToRelations(concept, type));
	}

	/// <summary>
	/// Delete relations for each provided <see cref="RelType"/> in the <paramref name="concept"/>.
	/// </summary>
	public static void ClearRelations<TId>(this IMongoCollection<HierarchyRelation<TId>> collection, string concept, RelType types = RelType.All)
		where TId : notnull
	{
		foreach (var type in Rel.GetSpecific(types))
			collection.DeleteMany(rel => rel.Concept == concept && rel.Type == type);

	}

	/// <summary>
	/// Update the <paramref name="collection"/> with changes generated from the difference between the stored relations and the state of the <paramref name="hierarchy"/>.
	/// </summary>
	/// <remarks>
	/// This causes two round trips to the database as state needs to first be read and then updated.
	/// If you would like to have only a single round trip, consider keeping the known state and
	/// use the overload where you provide both the old and new hierarchy state.
	/// </remarks>
	public static BulkWriteResult<HierarchyRelation<TId>> UpdateRelations<TId, TItem>(
		this IMongoCollection<HierarchyRelation<TId>> collection,
		Hierarchy<TItem, TId> hierarchy,
		string concept,
		RelType types
	)
		where TId : notnull
		where TItem : notnull
	{
		Rel.AssertHasSpecific(types);

		var relations = collection.GetRelations(concept, types).ToLookup(r => r.Type);

		var writeModels = Rel.GetSpecific(types).SelectMany(specific =>
		{
			var newMap = hierarchy.ToMap(specific);
			var oldMap = (relations[specific] ?? Enumerable.Empty<HierarchyRelation<TId>>()).ToMap();
			return HierarchyChanges
				.Difference(oldMap, newMap, concept, specific)
				.ToWriteModels();
		});

		return collection.BulkWrite(writeModels);
	}

	/// <summary>
	/// Update the <paramref name="collection"/> with changes generated from the difference between the old and new hierarchy state.
	/// </summary>
	public static BulkWriteResult<HierarchyRelation<TId>> UpdateRelations<TId, TOld, TNew>(
		this IMongoCollection<HierarchyRelation<TId>> collection,
		Hierarchy<TOld, TId> oldHierarchy,
		Hierarchy<TNew, TId> newHierarchy,
		string concept,
		RelType types
	)
		where TId : notnull
		where TOld : notnull
		where TNew : notnull
	{
		var differences = HierarchyChanges.Differences(oldHierarchy, newHierarchy, concept, types);

		var writeModels = differences.SelectMany(diff => diff.ToWriteModels());

		return collection.BulkWrite(writeModels);
	}
}
