namespace Loken.Hierarchies;

/// <summary>
/// Extension methods for retrieving nodes within a graph of <see cref="Node{TItem}"/>s.
/// </summary>
public static class NodeGetExtensions
{
	#region Descendants
	/// <summary>
	/// Get descendant nodes by traversing according to the options.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="root">The root node to traverse from.</param>
	/// <param name="descend">Options for controlling how we descend the graph.</param>
	/// <returns>An enumeration of nodes.</returns>
	public static IList<Node<TItem>> GetDescendants<TItem>(this Node<TItem>? root, Descend? descend = null)
		where TItem : notnull
	{
		if (root is null)
			return [];
		var opts = Descend.Normalize(descend, includeSelfDefault: false); // descendants default exclude self
		return opts.IncludeSelf == true
			? Flatten.Graph(root, n => n.Children, opts)
			: Flatten.Graph(root.Children, n => n.Children, opts);
	}

	/// <summary>
	/// Get descendant nodes by traversing according to the options.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="roots">The root nodes to traverse from.</param>
	/// <param name="descend">Options for controlling how we descend the graph.</param>
	/// <returns>An enumeration of nodes.</returns>
	public static IList<Node<TItem>> GetDescendants<TItem>(this IEnumerable<Node<TItem>> roots, Descend? descend = null)
		where TItem : notnull
	{
		var opts = Descend.Normalize(descend, includeSelfDefault: false);
		return opts.IncludeSelf == true
			? Flatten.Graph(roots, n => n.Children, opts)
			: Flatten.Graph(roots.SelectMany(r => r.Children), n => n.Children, opts);
	}
	#endregion

	#region Ancestors
	/// <summary>
	/// Walks up the <see cref="Node{TItem}.Parent"/> chain yielding ancestor nodes.
	/// </summary>
	/// <param name="node">The starting node.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>Ancestor nodes in order from nearest to farthest.</returns>
	public static IList<Node<TItem>> GetAncestors<TItem>(this Node<TItem>? node, Ascend? ascend = null)
		where TItem : notnull
	{
		if (node is null)
			return [];
		var opts = Ascend.Normalize(ascend, includeSelfDefault: false); // ancestors default exclude self
		var first = opts.IncludeSelf == true ? node : node.Parent;
		return Flatten.Sequence(first, n => n.Parent);
	}

	/// <summary>
	/// Collects all unique ancestor nodes from multiple starting nodes (deduplicated as encountered).
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="nodes">The starting nodes.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>Unique ancestor nodes in the order first encountered.</returns>
	public static IList<Node<TItem>> GetAncestors<TItem>(this IEnumerable<Node<TItem>> nodes, Ascend? ascend = null)
		where TItem : notnull
	{
		var opts = Ascend.Normalize(ascend, includeSelfDefault: false);
		var seen = new HashSet<Node<TItem>>(ReferenceEqualityComparer.Instance);
		var ancestors = new List<Node<TItem>>();
		foreach (var node in nodes)
		{
			var current = opts.IncludeSelf == true ? node : node.Parent;
			while (current != null && !seen.Contains(current))
			{
				seen.Add(current);
				ancestors.Add(current);
				current = current.Parent;
			}
		}
		return ancestors;
	}
	#endregion
}
