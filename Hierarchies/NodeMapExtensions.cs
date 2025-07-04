namespace Loken.Hierarchies;

/// <summary>
/// Extension methods for turning <see cref="Node{TItem}"/> roots into various kinds of relational multi-maps.
/// </summary>
public static class NodeMapExtensions
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
		var map = new MultiMap<TId>();

		Traverse.Graph(roots, (node, signal) =>
		{
			signal.Next(node.Children);

			var nodeId = identify(node.Item);
			if (node.IsLeaf)
			{
				if (signal.Depth == 0)
					map.Add(nodeId);
			}
			else
			{
				var childIds = node.Children.AsIds(identify);
				map.Add(nodeId, childIds);
			}
		}).EnumerateAll();

		return map;
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
		var map = new MultiMap<TId>();

		Traverse.Graph(roots, (node, signal) =>
		{
			signal.Next(node.Children);

			var nodeId = identify(node.Item);

			foreach (var ancestor in node.GetAncestors())
				map.Add(identify(ancestor.Item), nodeId);
		}).EnumerateAll();

		return map;
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
		var map = new MultiMap<TId>();

		Traverse.Graph(roots, (node, signal) =>
		{
			signal.Next(node.Children);

			var nodeId = identify(node.Item);
			foreach (var ancestor in node.GetAncestors())
				map.Add(nodeId, identify(ancestor.Item));
		}).EnumerateAll();

		return map;
	}
}
