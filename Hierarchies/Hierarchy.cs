namespace Loken.Hierarchies;

/// <summary>
/// Create <see cref="Hierarchy{T, TId}"/> instances from various inputs.
/// May be fluently chained with instance methods when building the hierarchy.
/// </summary>
public static class Hierarchy
{
	/// <summary>
	/// Create an empty <see cref="Hierarchy{TItem, TId}"/>.
	/// </summary>
	public static Hierarchy<TItem, TId> CreateEmpty<TItem, TId>(Func<TItem, TId> identify)
		where TId : notnull
		where TItem : notnull
	{
		return new Hierarchy<TItem, TId>(identify);
	}

	/// <summary>
	/// Create a <see cref="Hierarchy{TItem, TId}"/> from the <paramref name="items"/>
	/// with relationships created by identifying the parent.
	/// </summary>
	public static Hierarchy<TItem, TId> CreateParented<TItem, TId>(Func<TItem, TId> identify, Func<TItem, TId?> identifyParent, IList<TItem> items)
		where TId : notnull
		where TItem : notnull
	{
		var relations = new List<Relation<TId>>();

		foreach (var item in items)
		{
			var parent = identifyParent(item);
			if (parent != null)
				relations.Add(new(parent, identify(item)));
		}

		return CreateRelational(identify, items, [.. relations]);
	}

	/// <summary>
	/// Create a <see cref="Hierarchy{TItem, TId}"/> from the <paramref name="items"/>
	/// with relationships inferred from the <paramref name="childMap"/>.
	/// </summary>
	public static Hierarchy<TItem, TId> CreateMapped<TItem, TId>(Func<TItem, TId> identify, IEnumerable<TItem> items, MultiMap<TId> childMap)
		where TId : notnull
		where TItem : notnull
	{
		return new Hierarchy<TItem, TId>(identify).AttachRoot(Nodes.AssembleItems(identify, items, childMap));
	}

	/// <summary>
	/// Create a <see cref="Hierarchy{TItem, TId}"/> from the <paramref name="items"/>
	/// with relationships inferred from the <paramref name="relations"/>.
	/// </summary>
	public static Hierarchy<TItem, TId> CreateRelational<TItem, TId>(Func<TItem, TId> identify, IEnumerable<TItem> items, params Relation<TId>[] relations)
		where TId : notnull
		where TItem : notnull
	{
		var childMap = relations.ToChildMap();
		return CreateMapped(identify, items, childMap);
	}

	/// <summary>
	/// Create a <see cref="Hierarchy{TItem, TId}"/> from the <paramref name="items"/>
	/// with relationships matching those found in the <paramref name="other"/> hierarchy.
	/// </summary>
	public static Hierarchy<TItem, TId> CreateMatching<TItem, TId, TOther>(Func<TItem, TId> identify, IEnumerable<TItem> items, Hierarchy<TOther, TId> other)
		where TId : notnull
		where TItem : notnull
		where TOther : notnull
	{
		var childMap = other.Roots.ToChildMap(other.Identify);
		return CreateMapped(identify, items, childMap);
	}


	/// <summary>
	/// Create an empty <see cref="Hierarchy{TId}"/>.
	/// </summary>
	public static Hierarchy<TId> CreateEmpty<TId>()
		where TId : notnull
	{
		return new Hierarchy<TId>();
	}

	/// <summary>
	/// Create a <see cref="Hierarchy{TId}"/> with nodes and relationships inferred from the <paramref name="childMap"/>.
	/// </summary>
	public static Hierarchy<TId> CreateMapped<TId>(MultiMap<TId> childMap)
		where TId : notnull
	{
		var roots = Nodes.AssembleIds(childMap);
		return (Hierarchy<TId>)new Hierarchy<TId>().AttachRoot(roots);
	}

	/// <summary>
	/// Create a <see cref="Hierarchy{TId}"/> with nodes and relationships inferred from the <paramref name="relations"/>.
	/// </summary>
	public static Hierarchy<TId> CreateRelational<TId>(params Relation<TId>[] relations)
		where TId : notnull
	{
		var childMap = relations.ToChildMap();
		return CreateMapped(childMap);
	}

	/// <summary>
	/// Create a <see cref="Hierarchy{TId}"/> with nodes and relationships inferred from the <paramref name="relations"/>.
	/// </summary>
	public static Hierarchy<TId> CreateMatching<TId, TOther>(Hierarchy<TOther, TId> other)
		where TId : notnull
		where TOther : notnull
	{
		var childMap = other.Roots.ToChildMap(other.Identify);
		return CreateMapped(childMap);
	}
}