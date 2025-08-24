namespace Loken.Hierarchies.Traversal;

/// <summary>
/// Provides graph search helpers that return either the first match or all matches.
/// </summary>
public static partial class Search
{
	/// <summary>
	/// Search a graph of nodes by traversing from a single <paramref name="root"/>.
	/// <para>The search stops when the first node matching <paramref name="predicate"/> is found.</para>
	/// </summary>
	public static TNode? Graph<TNode>(TNode? root, Func<TNode, IEnumerable<TNode>?> next, Func<TNode, bool> predicate, bool detectCycles = false, TraversalType type = TraversalType.BreadthFirst)
	{
		if (root is null)
			return default;

		HashSet<TNode>? visited = null;
		if (detectCycles)
		{
			// Align with TS semantics: nodes are considered unique by identity, not by item equality.
			// If TNode is a reference type, use a reference comparer; otherwise use default comparer.
			visited = typeof(TNode).IsValueType
				? new HashSet<TNode>()
				: new HashSet<TNode>(ReferenceEqualityComparer<TNode>.Instance);
		}
		ILinear<TNode> store = type == TraversalType.DepthFirst
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
					if (children is TNode[] childArray)
						store.Attach(childArray);
					else if (children is List<TNode> childList)
						store.Attach(childList);
					else
						store.Attach(children);
				}
			}
		}

		return default;
	}

	/// <summary>
	/// Search a graph of nodes by traversing from a set of <paramref name="roots"/>.
	/// <para>The search stops when the first node matching <paramref name="predicate"/> is found.</para>
	/// </summary>
	public static TNode? Graph<TNode>(IEnumerable<TNode> roots, Func<TNode, IEnumerable<TNode>?> next, Func<TNode, bool> predicate, bool detectCycles = false, TraversalType type = TraversalType.BreadthFirst)
	{
		HashSet<TNode>? visited = null;
		if (detectCycles)
		{
			visited = typeof(TNode).IsValueType
				? new HashSet<TNode>()
				: new HashSet<TNode>(ReferenceEqualityComparer<TNode>.Instance);
		}
		ILinear<TNode> store = type == TraversalType.DepthFirst
			? new LinearStack<TNode>()
			: new LinearQueue<TNode>();

		if (roots is TNode[] arr)
			store.Attach(arr);
		else if (roots is List<TNode> list)
			store.Attach(list);
		else
			store.Attach(roots);

		while (store.TryDetach(out var node))
		{
			if (visited is null || visited.Add(node))
			{
				if (predicate(node))
					return node;

				var children = next(node);
				if (children is not null)
				{
					if (children is TNode[] childArray)
						store.Attach(childArray);
					else if (children is List<TNode> childList)
						store.Attach(childList);
					else
						store.Attach(children);
				}
			}
		}

		return default;
	}

	/// <summary>
	/// Search a graph of nodes by traversing from a single <paramref name="root"/>.
	/// <para>The search is exhaustive and returns all nodes matching <paramref name="predicate"/>.</para>
	/// </summary>
	public static IList<TNode> GraphMany<TNode>(TNode? root, Func<TNode, IEnumerable<TNode>?> next, Func<TNode, bool> predicate, bool detectCycles = false, TraversalType type = TraversalType.BreadthFirst)
	{
		if (root is null)
			return [];

		HashSet<TNode>? visited = null;
		if (detectCycles)
		{
			visited = typeof(TNode).IsValueType
				? new HashSet<TNode>()
				: new HashSet<TNode>(ReferenceEqualityComparer<TNode>.Instance);
		}
		ILinear<TNode> store = type == TraversalType.DepthFirst
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
					if (children is TNode[] childArray)
						store.Attach(childArray);
					else if (children is List<TNode> childList)
						store.Attach(childList);
					else
						store.Attach(children);
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Search a graph of nodes by traversing from a set of <paramref name="roots"/>.
	/// <para>The search is exhaustive and returns all nodes matching <paramref name="predicate"/>.</para>
	/// </summary>
	public static IList<TNode> GraphMany<TNode>(IEnumerable<TNode> roots, Func<TNode, IEnumerable<TNode>?> next, Func<TNode, bool> predicate, bool detectCycles = false, TraversalType type = TraversalType.BreadthFirst)
	{
		HashSet<TNode>? visited = null;
		if (detectCycles)
		{
			visited = typeof(TNode).IsValueType
				? new HashSet<TNode>()
				: new HashSet<TNode>(ReferenceEqualityComparer<TNode>.Instance);
		}
		ILinear<TNode> store = type == TraversalType.DepthFirst
			? new LinearStack<TNode>()
			: new LinearQueue<TNode>();

		if (roots is TNode[] arr)
			store.Attach(arr);
		else if (roots is List<TNode> list)
			store.Attach(list);
		else
			store.Attach(roots);

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
					if (children is TNode[] childArray)
						store.Attach(childArray);
					else if (children is List<TNode> childList)
						store.Attach(childList);
					else
						store.Attach(children);
				}
			}
		}

		return result;
	}
}
