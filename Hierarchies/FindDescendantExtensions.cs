namespace Loken.Hierarchies;

/// <summary>
/// Extension methods for finding and traversing descendant nodes within a graph of <see cref="Node{TItem}"/>s.
/// </summary>
public static class FindDescendantExtensions
{
	/// <summary>
	/// Find the first descendant node matching the predicate.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="roots">The root nodes to search from.</param>
	/// <param name="predicate">The predicate to match nodes.</param>
	/// <param name="includeSelf">Whether to include the root nodes themselves in the search.</param>
	/// <param name="traversalType">The type of traversal to use (breadth-first or depth-first).</param>
	/// <returns>The first matching descendant node, or null if none found.</returns>
	public static Node<TItem>? FindDescendant<TItem>(this IEnumerable<Node<TItem>> roots, NodePredicate<TItem> predicate, bool includeSelf = false, TraversalType traversalType = TraversalType.BreadthFirst)
		where TItem : notnull
	{
		var rootsToSearch = includeSelf ? roots : roots.SelectMany(r => r.Children);

		foreach (var node in Traverse.Graph(rootsToSearch, (n, signal) => signal.Next(n.Children), false, traversalType))
		{
			if (predicate(node))
				return node;
		}

		return null;
	}

	/// <summary>
	/// Find the first descendant node matching the predicate.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="root">The root node to search from.</param>
	/// <param name="predicate">The predicate to match nodes.</param>
	/// <param name="includeSelf">Whether to include the root node itself in the search.</param>
	/// <param name="traversalType">The type of traversal to use (breadth-first or depth-first).</param>
	/// <returns>The first matching descendant node, or null if none found.</returns>
	public static Node<TItem>? FindDescendant<TItem>(this Node<TItem>? root, NodePredicate<TItem> predicate, bool includeSelf = false, TraversalType traversalType = TraversalType.BreadthFirst)
		where TItem : notnull
	{
		if (root == null) return null;
		return new[] { root }.FindDescendant(predicate, includeSelf, traversalType);
	}

	/// <summary>
	/// Find all descendant nodes matching the predicate.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="roots">The root nodes to search from.</param>
	/// <param name="predicate">The predicate to match nodes.</param>
	/// <param name="includeSelf">Whether to include the root nodes themselves in the search.</param>
	/// <param name="traversalType">The type of traversal to use (breadth-first or depth-first).</param>
	/// <returns>All matching descendant nodes.</returns>
	public static IList<Node<TItem>> FindDescendants<TItem>(this IEnumerable<Node<TItem>> roots, NodePredicate<TItem> predicate, bool includeSelf = false, TraversalType traversalType = TraversalType.BreadthFirst)
		where TItem : notnull
	{
		var results = new List<Node<TItem>>();
		var rootsToSearch = includeSelf ? roots : roots.SelectMany(r => r.Children);

		foreach (var node in Traverse.Graph(rootsToSearch, (n, signal) => signal.Next(n.Children), false, traversalType))
		{
			if (predicate(node))
				results.Add(node);
		}

		return results;
	}

	/// <summary>
	/// Find all descendant nodes matching the predicate.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="root">The root node to search from.</param>
	/// <param name="predicate">The predicate to match nodes.</param>
	/// <param name="includeSelf">Whether to include the root node itself in the search.</param>
	/// <param name="traversalType">The type of traversal to use (breadth-first or depth-first).</param>
	/// <returns>All matching descendant nodes.</returns>
	public static IList<Node<TItem>> FindDescendants<TItem>(this Node<TItem>? root, NodePredicate<TItem> predicate, bool includeSelf = false, TraversalType traversalType = TraversalType.BreadthFirst)
		where TItem : notnull
	{
		if (root == null) return new List<Node<TItem>>();
		return new[] { root }.FindDescendants(predicate, includeSelf, traversalType);
	}

	/// <summary>
	/// Check if any descendant node matches the predicate.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="roots">The root nodes to search from.</param>
	/// <param name="predicate">The predicate to match nodes.</param>
	/// <param name="includeSelf">Whether to include the root nodes themselves in the search.</param>
	/// <param name="traversalType">The type of traversal to use (breadth-first or depth-first).</param>
	/// <returns>True if any descendant matches, false otherwise.</returns>
	public static bool HasDescendant<TItem>(this IEnumerable<Node<TItem>> roots, NodePredicate<TItem> predicate, bool includeSelf = false, TraversalType traversalType = TraversalType.BreadthFirst)
		where TItem : notnull
	{
		return roots.FindDescendant(predicate, includeSelf, traversalType) != null;
	}

	/// <summary>
	/// Check if any descendant node matches the predicate.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="root">The root node to search from.</param>
	/// <param name="predicate">The predicate to match nodes.</param>
	/// <param name="includeSelf">Whether to include the root node itself in the search.</param>
	/// <param name="traversalType">The type of traversal to use (breadth-first or depth-first).</param>
	/// <returns>True if any descendant matches, false otherwise.</returns>
	public static bool HasDescendant<TItem>(this Node<TItem>? root, NodePredicate<TItem> predicate, bool includeSelf = false, TraversalType traversalType = TraversalType.BreadthFirst)
		where TItem : notnull
	{
		if (root == null) return false;
		return new[] { root }.HasDescendant(predicate, includeSelf, traversalType);
	}

	/// <summary>
	/// Get descendant nodes by traversing according to the options.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="node">The root node to traverse from.</param>
	/// <param name="includeSelf">Whether to include the root node itself.</param>
	/// <param name="type">The type of traversal to use (breadth-first or depth-first).</param>
	/// <returns>An enumeration of nodes.</returns>
	public static IEnumerable<Node<TItem>> GetDescendants<TItem>(this Node<TItem> node, bool includeSelf = false, TraversalType type = TraversalType.BreadthFirst)
		where TItem : notnull
	{
		var roots = includeSelf ? new[] { node } : node.Children;
		return Traverse.Graph(roots, (n, signal) => signal.Next(n.Children), false, type);
	}

	/// <summary>
	/// Get descendant items by traversing according to the options.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="roots">The root nodes to traverse from.</param>
	/// <param name="includeSelf">Whether to include the root nodes themselves.</param>
	/// <param name="traversalType">The type of traversal to use (breadth-first or depth-first).</param>
	/// <returns>Items from all descendant nodes in traversal order.</returns>
	public static IList<TItem> GetDescendantItems<TItem>(this IEnumerable<Node<TItem>> roots, bool includeSelf = false, TraversalType traversalType = TraversalType.BreadthFirst)
		where TItem : notnull
	{
		var results = new List<TItem>();
		var rootsToTraverse = includeSelf ? roots : roots.SelectMany(r => r.Children);

		foreach (var node in Traverse.Graph(rootsToTraverse, (n, signal) => signal.Next(n.Children), false, traversalType))
		{
			results.Add(node.Item);
		}

		return results;
	}

	/// <summary>
	/// Get descendant items by traversing according to the options.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="root">The root node to traverse from.</param>
	/// <param name="includeSelf">Whether to include the root node itself.</param>
	/// <param name="traversalType">The type of traversal to use (breadth-first or depth-first).</param>
	/// <returns>Items from all descendant nodes in traversal order.</returns>
	public static IList<TItem> GetDescendantItems<TItem>(this Node<TItem>? root, bool includeSelf = false, TraversalType traversalType = TraversalType.BreadthFirst)
		where TItem : notnull
	{
		if (root == null) return new List<TItem>();
		return new[] { root }.GetDescendantItems(includeSelf, traversalType);
	}

	/// <summary>
	/// Generate a sequence of descendant nodes by traversing according to the options.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="roots">The root nodes to traverse from.</param>
	/// <param name="includeSelf">Whether to include the root nodes themselves.</param>
	/// <param name="traversalType">The type of traversal to use (breadth-first or depth-first).</param>
	/// <returns>Enumerable of descendant nodes in traversal order.</returns>
	public static IEnumerable<Node<TItem>> TraverseDescendants<TItem>(this IEnumerable<Node<TItem>> roots, bool includeSelf = false, TraversalType traversalType = TraversalType.BreadthFirst)
		where TItem : notnull
	{
		var rootsToTraverse = includeSelf ? roots : roots.SelectMany(r => r.Children);

		return Traverse.Graph(rootsToTraverse, (node, signal) => signal.Next(node.Children), false, traversalType);
	}

	/// <summary>
	/// Generate a sequence of descendant nodes by traversing according to the options.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="root">The root node to traverse from.</param>
	/// <param name="includeSelf">Whether to include the root node itself.</param>
	/// <param name="traversalType">The type of traversal to use (breadth-first or depth-first).</param>
	/// <returns>Enumerable of descendant nodes in traversal order.</returns>
	public static IEnumerable<Node<TItem>> TraverseDescendants<TItem>(this Node<TItem>? root, bool includeSelf = false, TraversalType traversalType = TraversalType.BreadthFirst)
		where TItem : notnull
	{
		if (root == null) return Enumerable.Empty<Node<TItem>>();
		return new[] { root }.TraverseDescendants(includeSelf, traversalType);
	}
}
