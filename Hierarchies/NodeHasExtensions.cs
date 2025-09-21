namespace Loken.Hierarchies;

/// <summary>
/// Extension methods for checking existence of nodes within a graph of <see cref="Node{TItem}"/>s.
/// </summary>
public static class NodeHasExtensions
{
	#region Descendants
	/// <summary>
	/// Check if any descendant node matches the predicate.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="root">The root node to search from.</param>
	/// <param name="predicate">The predicate to match nodes.</param>
	/// <param name="includeSelf">Whether to include the root node itself in the search.</param>
	/// <param name="traversalType">The type of traversal to use (breadth-first or depth-first).</param>
	/// <returns>True if any descendant matches, false otherwise.</returns>
	public static bool HasDescendant<TItem>(this Node<TItem>? root, Func<Node<TItem>, bool> predicate, Descend? descend = null)
		where TItem : notnull
	{
		if (root == null)
			return false;

		return root.FindDescendant(predicate, descend) != null;
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
	public static bool HasDescendant<TItem>(this IEnumerable<Node<TItem>> roots, Func<Node<TItem>, bool> predicate, Descend? descend = null)
		where TItem : notnull
	{
		return roots.FindDescendant(predicate, descend) != null;
	}
	#endregion

	#region Ancestors
	/// <summary>
	/// Check if any ancestor (optionally including the node itself) matches the predicate.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="node">The starting node.</param>
	/// <param name="predicate">The predicate to match nodes.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>True if any ancestor matches, false otherwise.</returns>
	public static bool HasAncestor<TItem>(this Node<TItem>? node, Func<Node<TItem>, bool> predicate, Ascend? ascend = null)
		where TItem : notnull
	{
		return node?.FindAncestor(predicate, ascend) != null;
	}

	/// <summary>
	/// Check if any ancestor (optionally including the starting nodes) matches the predicate across multiple starts.
	/// </summary>
	/// <typeparam name="TItem">The type of item in the nodes.</typeparam>
	/// <param name="nodes">The starting nodes.</param>
	/// <param name="predicate">The predicate to match nodes.</param>
	/// <param name="ascend">Options for controlling how we ascend the graph.</param>
	/// <returns>True if any ancestor matches, false otherwise.</returns>
	public static bool HasAncestor<TItem>(this IEnumerable<Node<TItem>> nodes, Func<Node<TItem>, bool> predicate, Ascend? ascend = null)
		where TItem : notnull
	{
		return nodes.FindAncestor(predicate, ascend) != null;
	}
	#endregion
}
