namespace Loken.Hierarchies;

/// <summary>
/// Create <see cref="Hierarchy{T, TId}"/> instances from various inputs.
/// May be fluently chained with instance methods when building the hierarchy.
/// </summary>
public static class Hierarchies
{
	/// <summary>
	/// Create an empty <see cref="Hierarchy{TId}"/>.
	/// </summary>
	public static Hierarchy<TId> Create<TId>()
		where TId : notnull
	{
		return new Hierarchy<TId>();
	}

	/// <summary>
	/// Create an empty <see cref="Hierarchy{TItem, TId}"/>.
	/// </summary>
	public static Hierarchy<TItem, TId> Create<TItem, TId>(Func<TItem, TId> identify)
		where TId : notnull
		where TItem : notnull
	{
		return new Hierarchy<TItem, TId>(identify);
	}


	/// <summary>
	/// Create a <see cref="Hierarchy{TId}"/> with nodes and relationships inferred from the <paramref name="childMap"/>.
	/// </summary>
	public static Hierarchy<TId> CreateFromChildMap<TId>(MultiMap<TId> childMap)
		where TId : notnull
	{
		var roots = Nodes.FromChildMap(childMap);
		return (Hierarchy<TId>)new Hierarchy<TId>().AttachRoot(roots);
	}

	/// <summary>
	/// Create a <see cref="Hierarchy{TItem, TId}"/> from the <paramref name="items"/>
	/// with relationships inferred from the <paramref name="childMap"/>.
	/// </summary>
	public static Hierarchy<TItem, TId> CreateFromChildMap<TItem, TId>(
		IEnumerable<TItem> items,
		Func<TItem, TId> identify,
		MultiMap<TId> childMap)
		where TId : notnull
		where TItem : notnull
	{
		return new Hierarchy<TItem, TId>(identify).AttachRoot(Nodes.FromItemsWithChildMap(items, identify, childMap));
	}

	/// <summary>
	/// Create a <see cref="Hierarchy{TId}"/> with nodes and relationships inferred from the <paramref name="relations"/>.
	/// </summary>
	public static Hierarchy<TId> CreateFromRelations<TId>(params Relation<TId>[] relations)
		where TId : notnull
	{
		var childMap = relations.ToChildMap();
		return CreateFromChildMap(childMap);
	}

	/// <summary>
	/// Create a <see cref="Hierarchy{TItem, TId}"/> from the <paramref name="items"/>
	/// with relationships inferred from the <paramref name="relations"/>.
	/// </summary>
	public static Hierarchy<TItem, TId> CreateFromRelations<TItem, TId>(
		IEnumerable<TItem> items,
		Func<TItem, TId> identify,
		params Relation<TId>[] relations)
		where TId : notnull
		where TItem : notnull
	{
		var childMap = relations.ToChildMap();
		return CreateFromChildMap(items, identify, childMap);
	}

	/// <summary>
	/// Create a <see cref="Hierarchy{TId}"/> with nodes and relationships inferred from the <paramref name="relations"/>.
	/// </summary>
	public static Hierarchy<TId> CreateFromHierarchy<TId, TOther>(Hierarchy<TOther, TId> other)
		where TId : notnull
		where TOther : notnull
	{
		var childMap = other.ToChildMap();
		return CreateFromChildMap(childMap);
	}

	/// <summary>
	/// Create a <see cref="Hierarchy{TItem, TId}"/> from the <paramref name="items"/>
	/// with relationships matching those found in the <paramref name="other"/> hierarchy.
	/// </summary>
	public static Hierarchy<TItem, TId> CreateFromHierarchy<TItem, TId, TOther>(
		IEnumerable<TItem> items,
		Func<TItem, TId> identify,
		Hierarchy<TOther, TId> other)
		where TId : notnull
		where TItem : notnull
		where TOther : notnull
	{
		var childMap = other.ToChildMap();
		return CreateFromChildMap(items, identify, childMap);
	}

	/// <summary>
	/// Create a <see cref="Hierarchy{TItem, TId}"/> from the <paramref name="items"/>
	/// with relationships created by identifying the parent.
	/// </summary>
	public static Hierarchy<TItem, TId> CreateFromParents<TItem, TId>(
		IEnumerable<TItem> items,
		Func<TItem, TId> identify,
		Func<TItem, TId?> getParentId)
		where TId : notnull
		where TItem : notnull
	{
		var relations = new List<Relation<TId>>();
		if (items is not ICollection<TItem> collection)
			items = items.ToList();

		foreach (var item in items)
		{
			var parentId = getParentId(item);
			if (parentId != null)
				relations.Add(new(parentId, identify(item)));
		}

		return CreateFromRelations(items, identify, [.. relations]);
	}

	/// <summary>
	/// Create a <see cref="Hierarchy{TItem, TId}"/> from the <paramref name="leaves"/>
	/// with relationships inferred using the provided <paramref name="getParent"/> delegate.
	/// </summary>
	public static Hierarchy<TItem, TId> CreateFromParents<TItem, TId>(
		IEnumerable<TItem> leaves,
		Func<TItem, TId> identify,
		Func<TItem, TItem?> getParent)
		where TId : notnull
		where TItem : notnull
	{
		var rootNodes = Nodes.FromItemsWithParents(leaves, getParent);
		return new Hierarchy<TItem, TId>(identify).AttachRoot(rootNodes);
	}

	/// <summary>
	/// Create a <see cref="Hierarchy{TItem, TId}"/> from the <paramref name="items"/>
	/// with relationships inferred using the provided <paramref name="getChildIds"/> delegate.
	/// </summary>
	public static Hierarchy<TItem, TId> CreateFromChildren<TItem, TId>(
		IEnumerable<TItem> items,
		Func<TItem, TId> identify,
		Func<TItem, IEnumerable<TId>?> getChildIds)
		where TId : notnull
		where TItem : notnull
	{
		var relations = new List<Relation<TId>>();

		foreach (var parentItem in items)
		{
			var parentId = identify(parentItem);
			var childIds = getChildIds(parentItem);
			if (childIds is null)
				continue;

			foreach (var childId in childIds)
				relations.Add(new Relation<TId>(parentId, childId));
		}

		return CreateFromRelations(items, identify, [.. relations]);
	}

	/// <summary>
	/// Create a <see cref="Hierarchy{TItem, TId}"/> from the <paramref name="roots"/>
	/// with relationships inferred using the provided <paramref name="getChildren"/> delegate.
	/// </summary>
	public static Hierarchy<TItem, TId> CreateFromChildren<TItem, TId>(
		IEnumerable<TItem> roots,
		Func<TItem, TId> identify,
		Func<TItem, IEnumerable<TItem>?> getChildren)
		where TId : notnull
		where TItem : notnull
	{
		var rootNodes = Nodes.FromItemsWithChildren(roots, getChildren);
		return new Hierarchy<TItem, TId>(identify).AttachRoot(rootNodes);
	}
}
