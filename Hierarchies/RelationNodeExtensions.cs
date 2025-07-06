namespace Loken.Hierarchies;

/// <summary>
/// Extensions for mapping between relations and nodes.
/// </summary>
public static class RelationNodeExtensions
{

	/// <summary>
	/// Create a sequence of relations by traversing the graph of the <paramref name="roots"/>.
	/// </summary>
	/// <typeparam name="TItem">The type of item.</typeparam>
	/// <typeparam name="TId">The type of IDs.</typeparam>
	/// <param name="roots">The roots to use for traversal.</param>
	/// <param name="identify">Means of getting an ID for an item.</param>
	/// <returns>An enumerable of <see cref="Relation{TId}"/>s.</returns>
	public static IEnumerable<Relation<TId>> ToRelations<TItem, TId>(
		IEnumerable<Node<TItem>> roots,
		Func<TItem, TId> identify)
		where TItem : notnull
		where TId : notnull
	{
		var relations = new List<Relation<TId>>();
		var processed = new HashSet<Node<TItem>>();

		void TraverseNode(Node<TItem> node)
		{
			if (processed.Contains(node))
				return;

			processed.Add(node);

			var nodeId = identify(node.Item);
			var children = node.Children.ToArray();

			if (children.Length == 0)
			{
				// Leaf node
				if (node.IsRoot)
				{
					// Isolated root node (no parent, no children)
					relations.Add(new Relation<TId>(nodeId));
				}
				return;
			}

			// Internal node - add parent-child relations
			foreach (var child in children)
			{
				var childId = identify(child.Item);
				relations.Add(new Relation<TId>(nodeId, childId));
			}

			// Recursively traverse children
			foreach (var child in children)
			{
				TraverseNode(child);
			}
		}

		foreach (var root in roots)
		{
			TraverseNode(root);
		}

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
		Node<TItem> root,
		Func<TItem, TId> identify)
		where TItem : notnull
		where TId : notnull
	{
		return ToRelations([root], identify);
	}


	/// <summary>
	/// Create nodes from the provided <paramref name="relations"/>.
	/// </summary>
	public static Node<TId>[] ToRootNodes<TId>(this IEnumerable<Relation<TId>> relations)
		where TId : notnull
	{
		var nodes = new Dictionary<TId, Node<TId>>();
		var children = new HashSet<TId>();

		// Process all relations
		foreach (var relation in relations)
		{
			if (relation.ParentOnly)
			{
				// One-sided relation: [node] - isolated node
				var nodeId = relation.Parent;
				if (!nodes.ContainsKey(nodeId))
					nodes[nodeId] = Nodes.Create(nodeId);
			}
			else
			{
				// Two-sided relation: [parent, child]
				var parentId = relation.Parent;
				var childId = relation.Child;

				// Ensure parent node exists
				if (!nodes.ContainsKey(parentId))
					nodes[parentId] = Nodes.Create(parentId);

				// Ensure child node exists
				if (!nodes.ContainsKey(childId))
					nodes[childId] = Nodes.Create(childId);

				// Create the parent-child relationship
				var parentNode = nodes[parentId];
				var childNode = nodes[childId];
				parentNode.Attach(childNode);

				// Track child nodes (they cannot be roots)
				children.Add(childId);
			}
		}

		// Find roots: nodes that exist but are not children of any other node
		var roots = new List<Node<TId>>();
		foreach (var (nodeId, node) in nodes)
		{
			if (!children.Contains(nodeId))
				roots.Add(node);
		}

		return [.. roots];
	}
}