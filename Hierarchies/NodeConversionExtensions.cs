namespace Loken.Hierarchies;

/// <summary>
/// Extension methods for converting between <see cref="Node{TItem}"/>s and items.
/// </summary>
/// <remarks>
/// Since there are implicit conversion operators in place for <see cref="Node{TItem}"/>,
/// there aren't extension methods for converting between single entries.
/// </remarks>
public static class NodeConversionExtensions
{
	/// <summary>
	/// Select the <see cref="Node{TItem}.Item"/> from each of the <paramref name="nodes"/>.
	/// </summary>
	public static IEnumerable<TItem> AsItems<TItem>(this IEnumerable<Node<TItem>> nodes)
		where TItem : notnull
	{
		return nodes.Select(n => n.Item);
	}

	/// <summary>
	/// Wrap each of the <paramref name="items"/> in a <see cref="Node{TItem}"/>.
	/// </summary>
	public static IEnumerable<Node<TItem>> AsNodes<TItem>(this IEnumerable<TItem> items)
		where TItem : notnull
	{
		return items.Select(item => (Node<TItem>)item);
	}

	/// <summary>
	/// Map each of the <paramref name="items"/> to its <typeparamref name="TId"/>.
	/// </summary>
	public static IEnumerable<TId> AsIds<TItem, TId>(this IEnumerable<TItem> items, Func<TItem, TId> identify)
		where TItem : notnull
		where TId : notnull
	{
		return items.Select(item => identify(item));
	}

	/// <summary>
	/// Map each of the <paramref name="nodes"/> to its <typeparamref name="TId"/>.
	/// </summary>
	public static IEnumerable<TId> AsIds<TItem, TId>(this IEnumerable<Node<TItem>> nodes, Func<TItem, TId> identify)
		where TItem : notnull
		where TId : notnull
	{
		return nodes.Select(n => identify(n.Item));
	}
}
