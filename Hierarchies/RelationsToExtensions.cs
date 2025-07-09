namespace Loken.Hierarchies;

/// <summary>
/// Extensions for mapping from <see cref="Relation"/> to other representations.
/// </summary>
public static class RelationsToExtensions
{
	/// <summary>
	/// Create a child-map matching the <paramref name="relations"/>.
	/// </summary>
	public static MultiMap<TId> ToChildMap<TId>(this IEnumerable<Relation<TId>> relations)
		where TId : notnull
	{
		var map = new MultiMap<TId>();

		foreach (var relation in relations)
		{
			if (relation.ParentOnly)
				map.Add(relation.Parent);
			else
				map.Add(relation.Parent, relation.Child);
		}

		return map;
	}


	/// <summary>
	/// Create nodes from the provided <paramref name="relations"/>.
	/// </summary>
	public static Node<TId>[] ToNodes<TId>(this IEnumerable<Relation<TId>> relations)
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
