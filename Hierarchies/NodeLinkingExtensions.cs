namespace Loken.Hierarchies;

public static class NodeLinkingExtensions
{
	/// <summary>
	/// Disassmbling a node means to cascade detach it.
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
}
