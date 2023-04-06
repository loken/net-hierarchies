namespace Loken.Hierarchies;

/// <summary>
/// Create <see cref="Hierarchy{T, TId}"/> instances from various inputs.
/// May be fluently chained with instance methods when building the hierarchy.
/// </summary>
public static class Hierarchy
{
	/// <summary>
	/// Create an unrooted <see cref="Hierarchy{TItem, TId}"/> with loose <see cref="Node{T}"/>s from the <paramref name="items"/>.
	/// Chain this with a <c>Using*</c> method in order to build the relationships.
	/// </summary>
	public static Hierarchy<TItem, TId> Create<TItem, TId>(Func<TItem, TId> identify, params TItem[] items)
		where TId : notnull
		where TItem : notnull
	{
		return new Hierarchy<TItem, TId>(identify, items);
	}

	/// <summary>
	/// Create an unrooted <see cref="Hierarchy{T, TId}"/> with loose <see cref="Node{T}"/>s from the <paramref name="items"/>.
	/// Chain this with a <c>Using*</c> method in order to build the relationships.
	/// </summary>
	public static Hierarchy<TItem, TId> Create<TItem, TId>(Func<TItem, TId> identify, IEnumerable<TItem> items)
		where TId : notnull
		where TItem : notnull
	{
		return new Hierarchy<TItem, TId>(identify, items);
	}

	/// <summary>
	/// Create an unrooted <see cref="Hierarchy{TId}"/> with loose <see cref="Node{TId}"/>s from the <paramref name="ids"/>.
	/// Chain this with a <c>Using*</c> method in order to build the relationships.
	/// </summary>
	public static Hierarchy<TId> Create<TId>(params TId[] ids)
		where TId : notnull
	{
		return new Hierarchy<TId>(ids);
	}

	/// <summary>
	/// Create an unrooted <see cref="Hierarchy{TId}"/> with loose <see cref="Node{TId}"/>s from the <paramref name="ids"/>.
	/// Chain this with a <c>Using*</c> method in order to build the relationships.
	/// </summary>
	public static Hierarchy<TId> Create<TId>(IEnumerable<TId> ids)
		where TId : notnull
	{
		return new Hierarchy<TId>(ids);
	}


	/// <summary>
	/// Create a rooted <see cref="Hierarchy{TId}"/> with linked <see cref="Node{TId}"/>s inferred from the <paramref name="relations"/>.
	/// </summary>
	public static Hierarchy<TId> FromRelations<TId>(params (TId parent, TId child)[] relations)
		where TId : notnull
	{
		var items = new HashSet<TId>();
		foreach (var relation in relations)
		{
			items.Add(relation.parent);
			items.Add(relation.child);
		}

		var hierarchy = Create(items.ToArray());
		hierarchy.UsingRelations(relations);
		return hierarchy;
	}

	/// <summary>
	/// Create a rooted <see cref="Hierarchy{TId}"/> with linked <see cref="Node{TId}"/>s inferred from the <paramref name="childMap"/>.
	/// </summary>
	public static Hierarchy<TId> FromChildMap<TId>(IDictionary<TId, ISet<TId>> childMap)
		where TId : notnull
	{
		var items = new HashSet<TId>();
		foreach (var pair in childMap)
		{
			items.Add(pair.Key);
			foreach (var child in pair.Value)
				items.Add(child);
		}

		var hierarchy = Create(items.ToArray());
		hierarchy.UsingChildMap(childMap);
		return hierarchy;
	}
}