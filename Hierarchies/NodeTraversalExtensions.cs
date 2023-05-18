namespace Loken.Hierarchies;

/// <summary>
/// Extensions for traversing <see cref="Node{TItem}"/>s.
/// </summary>
public static class NodeTraversalExtensions
{
	/// <summary>
	/// Performs a breadth first traversal yielding each node.
	/// </summary>
	/// <param name="includeSelf">
	/// Should the node itself be yielded (true) or should it start
	/// with its <see cref="Children"/> (false)?
	/// <para>No default value is because the caller should always make an active choice.</para>
	/// </param>
	/// <returns>An enumeration of nodes.</returns>
	public static IEnumerable<Node<TItem>> GetDescendants<TItem>(this Node<TItem> node, bool includeSelf, TraversalType type = TraversalType.BreadthFirst)
		where TItem : notnull
	{
		var roots = includeSelf ? new[] { node } : node.Children;
		return Traverse.Graph(roots, (n, signal) => signal.Next(n.Children), false, type);
	}

	/// <summary>
	/// Walks up the <see cref="Parent"/> links yielding each node.
	/// </summary>
	/// <param name="includeSelf">
	/// Should the node itself be yielded (true) or should it start
	/// with its <see cref="Parent"/> (false)?
	/// <para>No default value is because the caller should always make an active choice.</para>
	/// </param>
	/// <returns>An enumeration of nodes.</returns>
	public static IEnumerable<Node<TItem>> GetAncestors<TItem>(this Node<TItem> node, bool includeSelf)
		where TItem : notnull
	{
		var first = includeSelf ? node : node.Parent;
		return Traverse.Sequence(first, n => n.Parent);
	}
}
