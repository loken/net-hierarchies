namespace Loken.Hierarchies;

/// <summary>
/// Factory class for <see cref="Node{TItem}"/> which allow us to infer
/// the type parameter from the arguments.
/// </summary>
public static class Nodes
{
	/// <summary>
	/// Create a <see cref="Node{TItem}"/> from the <paramref name="item"/>.
	/// </summary>
	public static Node<TItem> Create<TItem>(TItem item)
		where TItem : notnull
	{
		return new() { Item = item };
	}

	/// <summary>
	/// Create a <see cref="Node{TItem}"/> for each of the <paramref name="items"/>.
	/// </summary>
	public static IEnumerable<Node<TItem>> CreateMany<TItem>(IEnumerable<TItem> items)
		where TItem : notnull
	{
		return items.Select(item => new Node<TItem>() { Item = item });
	}

	/// <summary>
	/// Create a <see cref="Node{TItem}"/> for each of the <paramref name="items"/>.
	/// </summary>
	public static Node<TItem>[] CreateMany<TItem>(params TItem[] items)
		where TItem : notnull
	{
		return [.. CreateMany(items.AsEnumerable())];
	}

	/// <summary>
	/// Assemble nodes of <paramref name="items"/> linked as described by the provided <paramref name="childMap"/>.
	/// </summary>
	/// <typeparam name="TItem">The type of item.</typeparam>
	/// <typeparam name="TId">The type of IDs.</typeparam>
	/// <param name="identify">Means of getting an ID for an item.</param>
	/// <param name="items">The items to wrap in nodes.</param>
	/// <param name="childMap">The map describing the relations.</param>
	/// <returns>The root nodes.</returns>
	public static ICollection<Node<TItem>> Assemble<TItem, TId>(Func<TItem, TId> identify, IEnumerable<TItem> items, MultiMap<TId> childMap)
		where TItem : notnull
		where TId : notnull
	{
		var nodes = new Dictionary<TId, Node<TItem>>();
		var roots = new Dictionary<TId, Node<TItem>>();

		foreach (var item in items)
		{
			var id = identify(item);
			var node = Create(item);

			nodes.Add(id, node);

			if (childMap.ContainsKey(id))
				roots.Add(id, node);
		}

		foreach (var (parentId, childIds) in childMap)
		{
			var parent = nodes[parentId];

			foreach (var childId in childIds)
			{
				var childNode = nodes[childId];
				parent.Attach(childNode);
				roots.Remove(childId);
			}
		}

		return roots.Values;
	}

	/// <summary>
	/// Assemble nodes of IDs linked as described by the provided <paramref name="childMap"/>.
	/// </summary>
	/// <typeparam name="TId">The type of IDs.</typeparam>
	/// <param name="childMap">The map describing the relations.</param>
	/// <returns>The root nodes.</returns>
	public static ICollection<Node<TId>> Assemble<TId>(MultiMap<TId> childMap)
		where TId : notnull
	{
		var nodes = new Dictionary<TId, Node<TId>>();
		var roots = new Dictionary<TId, Node<TId>>();

		foreach (var parentId in childMap.Keys)
		{
			var parentNode = Create(parentId);
			roots.Add(parentId, parentNode);
			nodes.Add(parentId, parentNode);
		}

		foreach (var (parentId, childIds) in childMap)
		{
			var parentNode = nodes[parentId];

			foreach (var childId in childIds)
			{
				var childNode = nodes.Lazy(childId, () => Create(childId));
				parentNode.Attach(childNode);
				roots.Remove(childId);
			}
		}

		return roots.Values;
	}
}
