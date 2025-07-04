namespace Loken.Hierarchies.Data;

/// <summary>
/// Process changes to hierarchies.
/// </summary>
public static class HierarchyChanges
{
	/// <summary>
	/// Create a difference between two states of a hierarchy.
	/// </summary>
	/// <typeparam name="TId">Type of ID.</typeparam>
	/// <typeparam name="TOld">The type of items in the old representation.</typeparam>
	/// <typeparam name="TNew">The type of items in the new representation.</typeparam>
	/// <param name="oldHierarchy">The old representation of the concept.</param>
	/// <param name="newHierarchy">The new representation of the concept.</param>
	/// <param name="concept">The concept of the hierarchy.</param>
	/// <param name="type">The "specific" relationship type for which we want to create a difference.</param>
	/// <returns>A <see cref="HierarchyDiff{TId}"/> which can be used for creating a batch of database updates.</returns>
	public static HierarchyDiff<TId> Difference<TId, TOld, TNew>(
		Hierarchy<TOld, TId> oldHierarchy,
		Hierarchy<TNew, TId> newHierarchy,
		string concept,
		RelType type
	)
		where TId : notnull
		where TOld : notnull
		where TNew : notnull
	{
		Rel.AssertIsSpecific(type);

		var oldMap = oldHierarchy.ToMap(type);
		var newMap = newHierarchy.ToMap(type);

		return Difference(oldMap, newMap, concept, type);
	}

	/// <summary>
	/// Create differences between two states of a hierarchy for multiple "specific" relationship types.
	/// </summary>
	/// <typeparam name="TId">Type of ID.</typeparam>
	/// <typeparam name="TOld">The type of items in the old representation.</typeparam>
	/// <typeparam name="TNew">The type of items in the new representation.</typeparam>
	/// <param name="oldHierarchy">The old representation of the concept.</param>
	/// <param name="newHierarchy">The new representation of the concept.</param>
	/// <param name="concept">The concept of the hierarchy.</param>
	/// <param name="types">The "specific" relationship types for which we want to create differences.</param>
	/// <returns>A sequence of <see cref="HierarchyDiff{TId}"/>s which can be used for creating a batch of database updates.</returns>
	public static IEnumerable<HierarchyDiff<TId>> Differences<TId, TOld, TNew>(
		Hierarchy<TOld, TId> oldHierarchy,
		Hierarchy<TNew, TId> newHierarchy,
		string concept,
		RelType types
	)
		where TId : notnull
		where TOld : notnull
		where TNew : notnull
	{
		Rel.AssertHasSpecific(types);

		foreach (var specific in Rel.GetSpecific(types))
			yield return Difference(oldHierarchy, newHierarchy, concept, specific);
	}

	/// <summary>
	/// Create a difference between two relationship representations of a hierarchy.
	/// </summary>
	/// <typeparam name="TId">Type of ID.</typeparam>
	/// <param name="oldMap">The old representation of the concept.</param>
	/// <param name="newMap">The new representation of the concept.</param>
	/// <param name="concept">The concept of the hierarchy.</param>
	/// <param name="type">The "specific" relationship type for which we want to create a difference.</param>
	/// <returns>A <see cref="HierarchyDiff{TId}"/> which can be used for creating a batch of database updates.</returns>
	public static HierarchyDiff<TId> Difference<TId>(
		MultiMap<TId> oldMap,
		MultiMap<TId> newMap,
		string concept,
		RelType type
	)
		where TId : notnull
	{
		Rel.AssertIsSpecific(type);

		var diff = new HierarchyDiff<TId> { Concept = concept, Type = type };

		foreach (var id in oldMap.Keys.Except(newMap.Keys))
		{
			diff.Deleted.Add(id);
		}

		foreach (var (id, newTargets) in newMap)
		{
			if (oldMap.TryGetValue(id, out var oldTargets))
			{
				var removed = new HashSet<TId>(oldTargets);
				removed.ExceptWith(newTargets);
				if (removed.Count > 0)
					diff.Removed.Add(id, removed);

				var added = new HashSet<TId>(newTargets);
				added.ExceptWith(oldTargets);
				if (added.Count > 0)
					diff.Added.Add(id, added);
			}
			else
			{
				diff.Inserted.Add(id, newTargets);
			}
		}

		return diff;
	}
}
