using System.Runtime.InteropServices;

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
	/// Extract the <see cref="Node{TItem}.Item"/> from each of the <paramref name="nodes"/>.
	/// </summary>
	public static IList<TItem> ToItems<TItem>(this IList<Node<TItem>> nodes)
		where TItem : notnull
	{
		var result = new List<TItem>(nodes.Count);
		CollectionsMarshal.SetCount(result, nodes.Count);
		var span = CollectionsMarshal.AsSpan(result);
		for (int i = 0; i < nodes.Count; i++)
			span[i] = nodes[i].Item;
		return result;
	}

	/// <summary>
	/// Extract the <see cref="Node{TItem}.Item"/> from each of the <paramref name="nodes"/>.
	/// </summary>
	public static IList<TItem> ToItems<TItem>(this ICollection<Node<TItem>> nodes)
		where TItem : notnull
	{
		var result = new List<TItem>(nodes.Count);
		CollectionsMarshal.SetCount(result, nodes.Count);
		var span = CollectionsMarshal.AsSpan(result);
		int index = 0;
		foreach (var node in nodes)
			span[index++] = node.Item;
		return result;
	}

	/// <summary>
	/// Extract the <see cref="Node{TItem}.Item"/> from each of the <paramref name="nodes"/>.
	/// </summary>
	public static IEnumerable<TItem> ToItems<TItem>(this IEnumerable<Node<TItem>> nodes)
		where TItem : notnull
	{
		return nodes.Select(n => n.Item);
	}

	/// <summary>
	/// Extract the <typeparamref name="TId"/> ID from the <paramref name="node"/>.
	/// </summary>
	public static TId ToId<TItem, TId>(this Node<TItem> node, Func<TItem, TId> identify)
		where TItem : notnull
		where TId : notnull
	{
		return identify(node.Item);
	}

	/// <summary>
	/// Extract the <typeparamref name="TId"/> ID from each of the <paramref name="nodes"/>.
	/// </summary>
	public static IList<TId> ToIds<TItem, TId>(this IList<Node<TItem>> nodes, Func<TItem, TId> identify)
		where TItem : notnull
		where TId : notnull
	{
		var result = new List<TId>(nodes.Count);
		CollectionsMarshal.SetCount(result, nodes.Count);
		var span = CollectionsMarshal.AsSpan(result);
		for (int i = 0; i < nodes.Count; i++)
			span[i] = identify(nodes[i].Item);
		return result;
	}

	/// <summary>
	/// Extract the <typeparamref name="TId"/> ID from each of the <paramref name="nodes"/>.
	/// </summary>
	public static IList<TId> ToIds<TItem, TId>(this ICollection<Node<TItem>> nodes, Func<TItem, TId> identify)
		where TItem : notnull
		where TId : notnull
	{
		var result = new List<TId>(nodes.Count);
		CollectionsMarshal.SetCount(result, nodes.Count);
		var span = CollectionsMarshal.AsSpan(result);
		int index = 0;
		foreach (var node in nodes)
			span[index++] = identify(node.Item);
		return result;
	}

	/// <summary>
	/// Extract the <typeparamref name="TId"/> ID from each of the <paramref name="nodes"/>.
	/// </summary>
	public static IEnumerable<TId> ToIds<TItem, TId>(this IEnumerable<Node<TItem>> nodes, Func<TItem, TId> identify)
		where TItem : notnull
		where TId : notnull
	{
		return nodes.Select(n => identify(n.Item));
	}
}
