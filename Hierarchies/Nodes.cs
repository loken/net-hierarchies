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
	/// Assemble nodes of IDs linked as described by the provided <paramref name="childMap"/>.
	/// </summary>
	/// <typeparam name="TId">The type of IDs.</typeparam>
	/// <param name="childMap">The map describing the relations.</param>
	/// <returns>The root nodes.</returns>
	public static ICollection<Node<TId>> AssembleIds<TId>(MultiMap<TId> childMap)
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

	/// <summary>
	/// Assemble nodes of <paramref name="items"/> linked as described by the provided <paramref name="childMap"/>.
	/// </summary>
	/// <typeparam name="TItem">The type of item.</typeparam>
	/// <typeparam name="TId">The type of IDs.</typeparam>
	/// <param name="identify">Means of getting an ID for an item.</param>
	/// <param name="items">The items to wrap in nodes.</param>
	/// <param name="childMap">The map describing the relations.</param>
	/// <returns>The root nodes.</returns>
	/// <exception cref="ArgumentException">
	/// When a parent or child ID referenced in the <paramref name="childMap"/>
	/// is not found in the provided <paramref name="items"/>.
	/// </exception>
	public static ICollection<Node<TItem>> AssembleItems<TItem, TId>(Func<TItem, TId> identify, IEnumerable<TItem> items, MultiMap<TId> childMap)
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
			if (!nodes.TryGetValue(parentId, out var parentNode))
				throw new ArgumentException($"Parent item with ID '{parentId}' not found in provided items.", nameof(items));

			foreach (var childId in childIds)
			{
				if (!nodes.TryGetValue(childId, out var childNode))
					throw new ArgumentException($"Child item with ID '{childId}' not found in provided items.", nameof(items));

				parentNode.Attach(childNode);
				roots.Remove(childId);
			}
		}

		return roots.Values;
	}

	/// <summary>
	/// Assemble nodes of <paramref name="items"/> linked as described by the provided <paramref name="getChildren"/> delegate.
	/// </summary>
	/// <typeparam name="TItem">The type of item.</typeparam>
	/// <param name="roots">The root items to wrap in nodes.</param>
	/// <param name="getChildren">The delegate for getting the child items from a parent item.</param>
	/// <returns>The root nodes.</returns>
	public static ICollection<Node<TItem>> AssembleItemsWithChildren<TItem>(
		IEnumerable<TItem> roots,
		Func<TItem, IEnumerable<TItem>?> getChildren)
		where TItem : notnull
	{
		var rootNodes = CreateMany(roots).ToArray();

		Traverse.Graph(rootNodes, node => {
			var children = getChildren(node.Item);
			if (children is not null)
			{
				foreach (var child in children)
				{
					var childNode = Create(child);
					node.Attach(childNode);
				}
			}

			return node.Children;
		}).EnumerateAll();

		return rootNodes;
	}

	/// <summary>
	/// Assemble nodes of <paramref name="leaves"/> linked as described by the provided <paramref name="getParent"/> delegate.
	/// </summary>
	/// <typeparam name="TItem">The type of item.</typeparam>
	/// <param name="leaves">The leaf items to wrap in nodes.</param>
	/// <param name="getParent">The delegate for getting the parent of an item.</param>
	/// <returns>The root nodes.</returns>
	public static ICollection<Node<TItem>> AssembleItemsWithParents<TItem>(
		IEnumerable<TItem> leaves,
		Func<TItem, TItem?> getParent)
		where TItem : notnull
	{
		var nodes = new Dictionary<TItem, Node<TItem>>();
		var roots = new List<Node<TItem>>();

		foreach (var leaf in leaves)
		{
			TItem currentItem = leaf;
			Node<TItem>? currentNode = GetNode(currentItem);

			while (true)
			{
				var parentItem = getParent(currentItem);
				if (parentItem is not null)
				{
					var parentSeen = nodes.ContainsKey(parentItem);
					var parentNode = GetNode(parentItem);

					parentNode.Attach(currentNode!);

					if (parentSeen)
						break;

					currentItem = parentItem;
					currentNode = parentNode;
				}
				else
				{
					roots.Add(currentNode!);
					break;
				}
			}
		}

		return roots;

		Node<TItem> GetNode(TItem item)
		{
			if (!nodes.TryGetValue(item, out var node))
			{
				node = Create(item);
				nodes[item] = node;
			}
			return node;
		}
	}
}
