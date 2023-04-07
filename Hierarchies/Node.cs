namespace Loken.Hierarchies;

/// <summary>
/// Factory class for <see cref="Node{TItem}"/> which allow us to infer
/// the type parameter from the arguments.
/// </summary>
public static class Node
{
	/// <summary>
	/// Create a <see cref="Node{TItem}"/> from the <paramref name="item"/>.
	/// </summary>
	public static Node<TItem> Create<TItem>(TItem item)
		where TItem : notnull
	{
		return new() { Item = item };
	}

	/// <summary>
	/// Create a <see cref="Node{TItem}"/> for each of the <paramref name="items"/>.
	/// </summary>
	public static IEnumerable<Node<TItem>> CreateMany<TItem>(IEnumerable<TItem> items)
		where TItem : notnull
	{
		return items.Select(item => new Node<TItem>() { Item = item });
	}

	/// <summary>
	/// Create a <see cref="Node{TItem}"/> for each of the <paramref name="items"/>.
	/// </summary>
	public static Node<TItem>[] CreateMany<TItem>(params TItem[] items)
		where TItem : notnull
	{
		return CreateMany(items.AsEnumerable()).ToArray();
	}
}
