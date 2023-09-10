namespace Loken.Hierarchies;

/// <summary>
/// Extensions for managing how <see cref="Node{TItem}"/>s are linked,
/// meaning their parent-child-relations.
/// </summary>
public static class NodeLinkingExtensions
{
	/// <summary>
	/// Attach the provided <paramref name="children"/> to the <paramref name="parent"/>.
	/// </summary>
	/// <param name="parent">Node to attach to.</param>
	/// <param name="children">Nodes to attach.</param>
	/// <returns>The <paramref name="parent"/> for method chaining purposes.</returns>
	/// <exception cref="InvalidOperationException">
	/// Must provide non-empty set of <paramref name="children"/> of a compatible brand
	/// that aren't already attached to another parent.
	/// </exception>
	public static Node<TItem> Attach<TItem>(this Node<TItem> parent, IEnumerable<Node<TItem>> children)
		where TItem : notnull
	{
		return parent.Attach(children.ToArray());
	}

	/// <summary>
	/// Detach the provided <paramref name="children"/> from the <paramref name="parent"/>.
	/// </summary>
	/// <param name="parent">Node to detach from.</param>
	/// <param name="children">Nodes to detach.</param>
	/// <returns>The <paramref name="parent"/> for method chaining purposes.</returns>
	/// <exception cref="InvalidOperationException">
	/// Must provide non-empty set of attached and non-branded <paramref name="children"/>.
	/// </exception>
	public static Node<TItem> Detach<TItem>(this Node<TItem> parent, IEnumerable<Node<TItem>> children)
		where TItem : notnull
	{
		return parent.Detach(children.ToArray());
	}

	/// <summary>
	/// Dismantling a node means to cascade detach it.
	/// We always cascade detach the nodes.
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

		foreach (var descendant in node.GetDescendants())
			descendant.DetachSelf();

		return node;
	}
}
