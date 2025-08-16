namespace Loken.Hierarchies;

/// <summary>
/// Extensions for mapping from <see cref="Node{TItem}"/> to other representations.
/// </summary>
public static class NodesToExtensions
{
	/// <summary>
	/// Create a map of ids to ids of a given <see cref="RelType"/> by traversing the graph of the <paramref name="root"/>.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <typeparam name="TId">The type of identity associated with an item.</typeparam>
	/// <param name="root">The root to use for traversal.</param>
	/// <param name="identify">Means of getting an ID for an item.</param>
	/// <returns>A multi-map of ids.</returns>
	public static MultiMap<TId> ToMap<TItem, TId>(this Node<TItem>? root, Func<TItem, TId> identify, RelType type)
		where TItem : notnull
		where TId : notnull
	{
		return root.ToEnumerable().ToMap(identify, type);
	}

	/// <summary>
	/// Create a map of ids to ids of a given <see cref="RelType"/> by traversing the graph of the <paramref name="roots"/>.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <typeparam name="TId">The type of identity associated with an item.</typeparam>
	/// <param name="roots">The roots to use for traversal.</param>
	/// <param name="identify">Means of getting an ID for an item.</param>
	/// <returns>A multi-map of ids.</returns>
	public static MultiMap<TId> ToMap<TItem, TId>(this IEnumerable<Node<TItem>> roots, Func<TItem, TId> identify, RelType type)
		where TItem : notnull
		where TId : notnull
	{
		return type switch
		{
			RelType.Children => roots.ToChildMap(identify),
			RelType.Descendants => roots.ToDescendantMap(identify),
			RelType.Ancestors => roots.ToAncestorMap(identify),
			_ => throw Rel.NotSpecificException(type),
		};
	}

	/// <summary>
	/// Create a map of ids to child-ids by traversing the graph of the <paramref name="root"/>.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <typeparam name="TId">The type of identity associated with an item.</typeparam>
	/// <param name="root">The root to use for traversal.</param>
	/// <param name="identify">Means of getting an ID for an item.</param>
	/// <returns>A parent-to-children map of ids.</returns>
	public static MultiMap<TId> ToChildMap<TItem, TId>(this Node<TItem>? root, Func<TItem, TId> identify)
		where TItem : notnull
		where TId : notnull
	{
		return root.ToEnumerable().ToChildMap(identify);
	}

	/// <summary>
	/// Create a map of ids to child-ids by traversing the graph of the <paramref name="roots"/>.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <typeparam name="TId">The type of identity associated with an item.</typeparam>
	/// <param name="roots">The roots to use for traversal.</param>
	/// <param name="identify">Means of getting an ID for an item.</param>
	/// <returns>A parent-to-children map of ids.</returns>
	public static MultiMap<TId> ToChildMap<TItem, TId>(this IEnumerable<Node<TItem>> roots, Func<TItem, TId> identify)
		where TItem : notnull
		where TId : notnull
	{
		var childMap = new MultiMap<TId>();

		foreach (var root in roots)
		{
			var nodeId = identify(root.Item);
			if (root.IsInternal)
			{
				var childIds = root.Children.ToIds(identify);
				childMap.Add(nodeId, childIds);
			}
			else
			{
				childMap.Add(nodeId);
			}
		}

		var nodes = Traverse.Graph(roots, node => node.Children.Where(n => n.IsInternal)).ToArray();

		foreach (var node in nodes)
		{
			var childNodes = node.Children;
			var nodeId = identify(node.Item);
			var childIds = childNodes.ToIds(identify);
			childMap.Add(nodeId, childIds);
		}

		return childMap;
	}

	/// <summary>
	/// Create a map of ids to descendant-ids by traversing the graph of the <paramref name="root"/>.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <typeparam name="TId">The type of identity associated with an item.</typeparam>
	/// <param name="root">The root to use for traversal.</param>
	/// <param name="identify">Means of getting an ID for an item.</param>
	/// <returns>A parent-to-descendants map of ids.</returns>
	public static MultiMap<TId> ToDescendantMap<TItem, TId>(this Node<TItem>? root, Func<TItem, TId> identify)
		where TItem : notnull
		where TId : notnull
	{
		return root.ToEnumerable().ToDescendantMap(identify);
	}

	/// <summary>
	/// Create a map of ids to descendant-ids by traversing the graph of the <paramref name="roots"/>.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <typeparam name="TId">The type of identity associated with an item.</typeparam>
	/// <param name="roots">The roots to use for traversal.</param>
	/// <param name="identify">Means of getting an ID for an item.</param>
	/// <returns>A parent-to-children map of ids.</returns>
	public static MultiMap<TId> ToDescendantMap<TItem, TId>(this IEnumerable<Node<TItem>> roots, Func<TItem, TId> identify)
		where TItem : notnull
		where TId : notnull
	{
		var descendantMap = new MultiMap<TId>();

		var store = new Queue<(Node<TItem> node, ISet<TId>[] ancestors)>();
		store.Enqueue(roots.Select(r => (r, Array.Empty<ISet<TId>>())));

		foreach (var root in roots)
		{
			var rootId = identify(root.Item);
			descendantMap.Add(rootId);
		}

		while (store.Count > 0)
		{
			var (node, ancestors) = store.Dequeue();
			var nodeId = identify(node.Item);

			foreach (var ancestor in ancestors)
				ancestor.Add(nodeId);

			if (node.IsInternal)
			{
				var nodeDescendants = descendantMap.LazySet(nodeId);
				ISet<TId>[] childAncestors = [.. ancestors, nodeDescendants];
				store.Enqueue(node.Children.Select(child => (child, childAncestors)));
			}
		}

		return descendantMap;
	}

	/// <summary>
	/// Create a map of ids to ancestor-ids by traversing the graph of the <paramref name="root"/>.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <typeparam name="TId">The type of identity associated with an item.</typeparam>
	/// <param name="root">The root to use for traversal.</param>
	/// <param name="identify">Means of getting an ID for an item.</param>
	/// <returns>A parent-to-ancestor map of ids.</returns>
	public static MultiMap<TId> ToAncestorMap<TItem, TId>(this Node<TItem>? root, Func<TItem, TId> identify)
		where TItem : notnull
		where TId : notnull
	{
		return root.ToEnumerable().ToAncestorMap(identify);
	}

	/// <summary>
	/// Create a map of ids to ancestor-ids by traversing the graph of the <paramref name="roots"/>.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <typeparam name="TId">The type of identity associated with an item.</typeparam>
	/// <param name="roots">The roots to use for traversal.</param>
	/// <param name="identify">Means of getting an ID for an item.</param>
	/// <returns>A parent-to-ancestor map of ids.</returns>
	public static MultiMap<TId> ToAncestorMap<TItem, TId>(this IEnumerable<Node<TItem>> roots, Func<TItem, TId> identify)
		where TItem : notnull
		where TId : notnull
	{
		var ancestorMap = new MultiMap<TId>();

		var store = new Queue<(Node<TItem> node, TId[]? ancestors)>();
		store.Enqueue(roots.Select(r => (r, null as TId[])));

		foreach (var root in roots)
		{
			if (root.IsLeaf)
				ancestorMap.Add(identify(root.Item));
		}

		while (store.Count > 0)
		{
			var (node, ancestors) = store.Dequeue();
			var nodeId = identify(node.Item);

			if (ancestors is not null)
				ancestorMap.Add(nodeId, ancestors);

			if (node.IsInternal)
			{
				TId[] childAncestors = ancestors is not null ? [nodeId, .. ancestors] : [nodeId];
				store!.Enqueue(node.Children.Select(child => (child, childAncestors)));
			}
		}

		return ancestorMap;
	}


	/// <summary>
	/// Create a sequence of relations by traversing the graph of the <paramref name="roots"/>.
	/// </summary>
	/// <typeparam name="TItem">The type of item.</typeparam>
	/// <typeparam name="TId">The type of IDs.</typeparam>
	/// <param name="roots">The roots to use for traversal.</param>
	/// <param name="identify">Means of getting an ID for an item.</param>
	/// <returns>An enumerable of <see cref="Relation{TId}"/>s.</returns>
	public static IEnumerable<Relation<TId>> ToRelations<TItem, TId>(
		this IEnumerable<Node<TItem>> roots,
		Func<TItem, TId> identify)
		where TItem : notnull
		where TId : notnull
	{
		var relations = new List<Relation<TId>>();

		Traverse.Graph(roots, node =>
		{
			var isRoot = node.IsRoot;
			var isLeaf = node.IsLeaf;
			if (isLeaf && isRoot)
			{
				var rootId = identify(node.Item);
				relations.Add(new(rootId));

				return [];
			}

			if (isLeaf)
				return [];

			var nodeId = identify(node.Item);
			var children = node.Children;
			var childIds = children.ToIds(identify);
			foreach (var childId in childIds)
				relations.Add(new(nodeId, childId));

			return children.Where(child => child.IsInternal);
		}).EnumerateAll();

		return relations;
	}

	/// <summary>
	/// Create a sequence of relations by traversing the graph of the <paramref name="root"/>.
	/// </summary>
	/// <typeparam name="TItem">The type of item.</typeparam>
	/// <typeparam name="TId">The type of IDs.</typeparam>
	/// <param name="root">The root to use for traversal.</param>
	/// <param name="identify">Means of getting an ID for an item.</param>
	/// <returns>An enumerable of <see cref="Relation{TId}"/>s.</returns>
	public static IEnumerable<Relation<TId>> ToRelations<TItem, TId>(
		this Node<TItem> root,
		Func<TItem, TId> identify)
		where TItem : notnull
		where TId : notnull
	{
		return ToRelations([root], identify);
	}
}
