namespace Loken.Hierarchies.Data.MongoDB;

/// <summary>
/// Extensions for working with <see cref="HierarchyDiff{TId}"/>.
/// </summary>
public static class HierarchyMongoDiffExtensions
{
	/// <summary>
	/// Create write models for all changes in the <paramref name="diff"/>.
	/// </summary>
	public static IEnumerable<WriteModel<HierarchyRelation<TId>>> ToWriteModels<TId>(this HierarchyDiff<TId> diff)
		where TId : notnull
	{
		var model = new List<WriteModel<HierarchyRelation<TId>>>();
		var filter = Builders<HierarchyRelation<TId>>.Filter;
		var update = Builders<HierarchyRelation<TId>>.Update;

		if (diff.Deleted.Any())
			yield return new DeleteManyModel<HierarchyRelation<TId>>(filter.In(r => r.Id, diff.Deleted));

		foreach (var (id, targets) in diff.Inserted)
		{
			yield return new InsertOneModel<HierarchyRelation<TId>>(new HierarchyRelation<TId>
			{
				Concept = diff.Concept,
				Type = diff.Type,
				Id = id,
				Targets = targets
			});
		}

		foreach (var (id, targets) in diff.Added)
		{
			yield return new UpdateOneModel<HierarchyRelation<TId>>(
				filter.Eq(r => r.Id, id),
				update.AddToSetEach(x => x.Targets, targets));
		}

		foreach (var (id, targets) in diff.Removed)
		{
			yield return new UpdateOneModel<HierarchyRelation<TId>>(
				filter.Eq(r => r.Id, id),
				update.PullAll(x => x.Targets, targets));
		}
	}
}
