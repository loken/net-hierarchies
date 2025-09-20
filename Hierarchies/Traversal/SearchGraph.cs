namespace Loken.Hierarchies.Traversal;

/// <summary>
/// Provides graph search helpers that return either the first match or all matches.
/// </summary>
public static partial class Search
{
	/// <summary>
	/// Searches a graph from a single <paramref name="root"/> and returns the first node matching <paramref name="predicate"/>.
	/// </summary>
	/// <typeparam name="TNode">The node type.</typeparam>
	/// <param name="root">Starting node; if <c>null</c> returns <c>null</c>.</param>
	/// <param name="next">Returns children for a node or <c>null</c>.</param>
	/// <param name="predicate">Tests each visited node.</param>
	/// <param name="descend">Options for controlling how we descend the graph.</param>
	/// <returns>The first match or <c>null</c>.</returns>
	public static TNode? Graph<TNode>(TNode? root, Func<TNode, IEnumerable<TNode>?> next, Func<TNode, bool> predicate, Descend? descend = null)
	{
		if (root is null)
			return default;
		var opts = Descend.Normalize(descend, includeSelfDefault: true);
		HashSet<TNode>? visited = null;

		if (opts.DetectCycles)
		{
			visited = typeof(TNode).IsValueType
				? new HashSet<TNode>()
				: new HashSet<TNode>(ReferenceEqualityComparer<TNode>.Instance);
		}

		ILinear<TNode> store = opts.Type == TraversalType.DepthFirst
			? new LinearStack<TNode>()
			: new LinearQueue<TNode>();

		store.Attach(root);
		while (store.TryDetach(out var node))
		{
			if (visited is null || visited.Add(node))
			{
				if (predicate(node))
					return node;

				var children = next(node);
				if (children is not null)
				{
					if (children is IList<TNode> childList)
						store.AttachMany(childList);
					else if (children is ICollection<TNode> childCollection)
						store.AttachMany(childCollection);
					else
						store.AttachMany(children);
				}
			}
		}

		return default;
	}

	/// <summary>
	/// Searches a graph from multiple <paramref name="roots"/> and returns the first node matching <paramref name="predicate"/>.
	/// </summary>
	/// <typeparam name="TNode">The node type.</typeparam>
	/// <param name="roots">Seed nodes; order influences initial visitation.</param>
	/// <param name="next">Returns children for a node or <c>null</c>.</param>
	/// <param name="predicate">Tests each visited node.</param>
	/// <param name="descend">Options for controlling how we descend the graph.</param>
	/// <returns>The first match or <c>null</c>.</returns>
	public static TNode? Graph<TNode>(IEnumerable<TNode> roots, Func<TNode, IEnumerable<TNode>?> next, Func<TNode, bool> predicate, Descend? descend = null)
	{
		var opts = Descend.Normalize(descend, includeSelfDefault: true);
		HashSet<TNode>? visited = null;

		if (opts.DetectCycles)
		{
			visited = typeof(TNode).IsValueType
				? new HashSet<TNode>()
				: new HashSet<TNode>(ReferenceEqualityComparer<TNode>.Instance);
		}

		ILinear<TNode> store = opts.Type == TraversalType.DepthFirst
			? new LinearStack<TNode>()
			: new LinearQueue<TNode>();

		if (roots is IList<TNode> rootList)
			store.AttachMany(rootList);
		else if (roots is ICollection<TNode> rootCollection)
			store.AttachMany(rootCollection);
		else
			store.AttachMany(roots);

		while (store.TryDetach(out var node))
		{
			if (visited is null || visited.Add(node))
			{
				if (predicate(node))
					return node;

				var children = next(node);
				if (children is not null)
				{
					if (children is IList<TNode> childList)
						store.AttachMany(childList);
					else if (children is ICollection<TNode> childCollection)
						store.AttachMany(childCollection);
					else
						store.AttachMany(children);
				}
			}
		}

		return default;
	}

	/// <summary>
	/// Searches a graph from a single <paramref name="root"/> and returns all nodes matching <paramref name="predicate"/>.
	/// </summary>
	/// <typeparam name="TNode">The node type.</typeparam>
	/// <param name="root">Starting node; if <c>null</c> returns empty.</param>
	/// <param name="next">Returns children for a node or <c>null</c>.</param>
	/// <param name="predicate">Tests each visited node.</param>
	/// <param name="descend">Options for controlling how we descend the graph.</param>
	/// <returns>All matches in visitation order.</returns>
	public static IList<TNode> GraphMany<TNode>(TNode? root, Func<TNode, IEnumerable<TNode>?> next, Func<TNode, bool> predicate, Descend? descend = null)
	{
		if (root is null)
			return [];
		var opts = Descend.Normalize(descend, includeSelfDefault: true);
		HashSet<TNode>? visited = null;

		if (opts.DetectCycles)
		{
			visited = typeof(TNode).IsValueType
				? new HashSet<TNode>()
				: new HashSet<TNode>(ReferenceEqualityComparer<TNode>.Instance);
		}

		ILinear<TNode> store = opts.Type == TraversalType.DepthFirst
			? new LinearStack<TNode>()
			: new LinearQueue<TNode>();

		store.Attach(root);

		var result = new List<TNode>();
		while (store.TryDetach(out var node))
		{
			if (visited is null || visited.Add(node))
			{
				if (predicate(node))
					result.Add(node);

				var children = next(node);
				if (children is not null)
				{
					if (children is IList<TNode> childList)
						store.AttachMany(childList);
					else if (children is ICollection<TNode> childCollection)
						store.AttachMany(childCollection);
					else
						store.AttachMany(children);
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Searches a graph from multiple <paramref name="roots"/> and returns all nodes matching <paramref name="predicate"/>.
	/// </summary>
	/// <typeparam name="TNode">The node type.</typeparam>
	/// <param name="roots">Seed nodes; order influences initial visitation.</param>
	/// <param name="next">Returns children for a node or <c>null</c>.</param>
	/// <param name="predicate">Tests each visited node.</param>
	/// <param name="descend">Options for controlling how we descend the graph.</param>
	/// <returns>All matches in visitation order.</returns>
	public static IList<TNode> GraphMany<TNode>(IEnumerable<TNode> roots, Func<TNode, IEnumerable<TNode>?> next, Func<TNode, bool> predicate, Descend? descend = null)
	{
		var opts = Descend.Normalize(descend, includeSelfDefault: true);
		HashSet<TNode>? visited = null;
		if (opts.DetectCycles)
		{
			visited = typeof(TNode).IsValueType
				? new HashSet<TNode>()
				: new HashSet<TNode>(ReferenceEqualityComparer<TNode>.Instance);
		}

		ILinear<TNode> store = opts.Type == TraversalType.DepthFirst
			? new LinearStack<TNode>()
			: new LinearQueue<TNode>();

		if (roots is IList<TNode> rootList)
			store.AttachMany(rootList);
		else if (roots is ICollection<TNode> rootCollection)
			store.AttachMany(rootCollection);
		else
			store.AttachMany(roots);

		var result = new List<TNode>();
		while (store.TryDetach(out var node))
		{
			if (visited is null || visited.Add(node))
			{
				if (predicate(node))
					result.Add(node);

				var children = next(node);
				if (children is not null)
				{
					if (children is IList<TNode> childList)
						store.AttachMany(childList);
					else if (children is ICollection<TNode> childCollection)
						store.AttachMany(childCollection);
					else
						store.AttachMany(children);
				}
			}
		}

		return result;
	}
}
