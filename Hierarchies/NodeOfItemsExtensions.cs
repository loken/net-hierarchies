namespace Loken.Hierarchies;

public static class NodeOfItemsExtensions
{
	/// <summary>
	/// Dismantling a node means to cascade detach it.
	/// We always caschade detach the nodes.
	/// We may also cascade up the ancestry, in which case the node is detached,
	/// and then the parent is dismantled, leading to the whole linked structure
	/// ending up unlinked.
	/// </summary>
	/// <param name="includeAncestry">
	/// Should we cascade through the ancestry (true) or only cascade through the nodes (false)?
	/// <para>No default value is because the caller should always make an active choice.</para>
	/// </param>
	/// <returns>The node itself for method chaining purposes.</returns>
	public static Node<TItem> Dismantle<TItem>(this Node<TItem> node, bool includeAncestry)
		where TItem : notnull
	{
		if (!node.IsRoot && includeAncestry)
		{
			var parent = node.Parent;
			node.DetachSelf();
			parent.Dismantle(true);
		}

		foreach (var descendant in node.GetDescendants(false))
			descendant.DetachSelf();

		return node;
	}

	/// <summary>
	/// Performs a breadth first traversal yielding each node.
	/// </summary>
	/// <param name="includeSelf">
	/// Should the node itself be yielded (true) or should it start
	/// with its <see cref="Children"/> (false)?
	/// <para>No default value is because the caller should always make an active choice.</para>
	/// </param>
	/// <returns>An enumeration of nodes.</returns>
	public static IEnumerable<Node<TItem>> GetDescendants<TItem>(this Node<TItem> node, bool includeSelf)
		where TItem : notnull
	{
		var roots = includeSelf ? new[] { node } : node.Children;
		return Traverse.Tree(roots, (n, signal) => signal.Next(n.Children));
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

	/// <summary>
	/// Select the <see cref="Node{TItem}.Item"/> from each of the <paramref name="nodes"/>.
	/// </summary>
	public static IEnumerable<TItem> ToItems<TItem>(this IEnumerable<Node<TItem>> nodes)
		where TItem : notnull
	{
		return nodes.Select(n => n.Item);
		;
	}
}
