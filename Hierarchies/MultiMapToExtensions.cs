namespace Loken.Hierarchies;

/// <summary>
/// Extensions for mapping from <see cref="MultiMap{TId}"/> to other representations.
/// </summary>
public static class MultiMapToExtensions
{
	/// <summary>
	/// Create a sequence of relations matching the <paramref name="childMap"/>.
	/// </summary>
	public static IEnumerable<Relation<TId>> ToRelations<TId>(this MultiMap<TId> childMap)
		where TId : notnull
	{
		foreach (var (parent, children) in childMap)
		{
			if (children.Count == 0)
			{
				yield return new(parent);
			}
			else
			{
				foreach (var child in children)
					yield return new(parent, child);
			}
		}
	}

	/// <summary>
	/// Create nodes from the provided <paramref name="childMap"/>.
	/// </summary>
	public static Node<TId>[] ToNodes<TId>(this MultiMap<TId> childMap)
		where TId : notnull
	{
		var nodes = new Dictionary<TId, Node<TId>>();
		var roots = new List<Node<TId>>();

		// Create all parent nodes first
		foreach (var parentId in childMap.Keys)
		{
			var parentNode = Nodes.Create(parentId);
			nodes[parentId] = parentNode;
		}

		// Establish parent-child relationships
		foreach (var (parentId, childIds) in childMap)
		{
			var parentNode = nodes[parentId];

			foreach (var childId in childIds)
			{
				var childNode = nodes.Lazy(childId, () => Nodes.Create(childId));
				parentNode.Attach(childNode);
			}
		}

		// Find roots
		foreach (var node in nodes.Values)
		{
			if (node.IsRoot)
				roots.Add(node);
		}

		return [.. roots];
	}

	/// <summary>
	/// Create a parent map from the <paramref name="childMap"/>.
	/// </summary>
	/// <remarks>
	/// Note: This method returns a <see cref="MultiMap{TId}"/> rather than a <c>Dictionary&lt;TId, TId?&gt;</c>
	/// due to the <c>notnull</c> constraint on <typeparamref name="TId"/>. Root nodes are represented
	/// as entries with empty sets (no parent), while non-root nodes have sets containing exactly one parent.
	/// This differs from the TypeScript implementation which uses <c>Map&lt;Id, Id | undefined&gt;</c>.
	/// </remarks>
	public static MultiMap<TId> ToParentMap<TId>(this MultiMap<TId> childMap, ISet<TId>? roots = null)
		where TId : notnull
	{
		var parentMap = new MultiMap<TId>();
		roots ??= childMap.ToRootIds();

		// Add roots that are also leaves, as otherwise we lose them
		foreach (var root in roots)
		{
			if (childMap.Get(root)?.Count == 0)
				parentMap.Add(root); // Empty set means no parent (root)
		}

		// Add parent-child mappings
		foreach (var (parent, children) in childMap)
		{
			foreach (var child in children)
				parentMap.Add(child, parent);
		}

		return parentMap;
	}

	/// <summary>
	/// Create a descendant map from the <paramref name="childMap"/>.
	/// </summary>
	public static MultiMap<TId> ToDescendantMap<TId>(this MultiMap<TId> childMap, MultiMap<TId>? parentMap = null)
		where TId : notnull
	{
		parentMap ??= childMap.ToParentMap();
		var descendantMap = new MultiMap<TId>();

		foreach (var (child, parents) in parentMap)
		{
			if (parents.Count == 0) // Root node
			{
				descendantMap.Add(child);
				continue;
			}

			var parent = parents.First(); // Each child has exactly one parent
			var ancestor = parent;
			while (true)
			{
				descendantMap.Add(ancestor, child);
				var ancestorParents = parentMap.Get(ancestor);
				if (ancestorParents == null || ancestorParents.Count == 0)
					break;
				ancestor = ancestorParents.First();
			}
		}

		return descendantMap;
	}

	/// <summary>
	/// Create an ancestor map from the <paramref name="childMap"/>.
	/// </summary>
	public static MultiMap<TId> ToAncestorMap<TId>(this MultiMap<TId> childMap, MultiMap<TId>? parentMap = null)
		where TId : notnull
	{
		parentMap ??= childMap.ToParentMap();
		var ancestorMap = new MultiMap<TId>();

		foreach (var (child, parents) in parentMap)
		{
			var ancestors = ancestorMap.LazySet(child);

			if (parents.Count == 0) // Root node
				continue;

			var ancestor = parents.First(); // Each child has exactly one parent
			while (true)
			{
				ancestors.Add(ancestor);
				var ancestorParents = parentMap.Get(ancestor);
				if (ancestorParents == null || ancestorParents.Count == 0)
					break;
				ancestor = ancestorParents.First();
			}
		}

		return ancestorMap;
	}

	/// <summary>
	/// Extract root IDs from the <paramref name="childMap"/>.
	/// </summary>
	public static HashSet<TId> ToRootIds<TId>(this MultiMap<TId> childMap)
		where TId : notnull
	{
		var seenChildren = new HashSet<TId>();
		var roots = new HashSet<TId>();

		foreach (var (parent, children) in childMap)
		{
			if (!seenChildren.Contains(parent))
				roots.Add(parent);

			foreach (var child in children)
			{
				seenChildren.Add(child);
				roots.Remove(child);
			}
		}

		return roots;
	}
}
