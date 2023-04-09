using Loken.Hierarchies.Traversal;

namespace Loken.Hierarchies;

/// <summary>
/// Predicate answering if an <paramref name="item"/> at a provided <paramref name="depth"/> is a result or not.
/// </summary>
/// <typeparam name="TItem">The <see cref="Type"/> of <paramref name="item"/> to result.</typeparam>
/// <param name="item">The <typeparamref name="TItem"/> to check.</param>
/// <param name="depth">The number of levels traversed when encountering the <paramref name="item"/>. Starts at 0 for roots.</param>
/// <returns>True if the <paramref name="item"/> is a result, False otherwise.</returns>
public delegate bool GraphMatch<in TItem>(TItem item, int depth)
	where TItem : notnull;

/// <summary>
/// Extension methods for finding nodes within a graph of <see cref="Node{TItem}"/>s using a breadth first approcah.
/// </summary>
public static class NodeFindExtensions
{
	/// <summary>
	/// Find the matching nodes by within the <paramref name="roots"/>.
	/// </summary>
	public static IEnumerable<Node<TItem>> Find<TItem>(this IEnumerable<Node<TItem>> roots, GraphMatch<Node<TItem>> match, bool detectCycles = false)
		where TItem : notnull
	{
		return Traverse.Graph(roots, (node, signal) =>
		{
			signal.Next(node.Children);

			if (!match(node, signal.Depth))
				signal.Skip();
		}, detectCycles);
	}

	/// <summary>
	/// Find the matching nodes by within the <paramref name="root"/>.
	/// </summary>
	public static IEnumerable<Node<TItem>> Find<TItem>(this Node<TItem>? root, GraphMatch<Node<TItem>> match, bool detectCycles = false)
		where TItem : notnull
	{
		var roots = root is not null ? new[] { root } : Enumerable.Empty<Node<TItem>>();
		return roots.Find(match, detectCycles);
	}

	/// <summary>
	/// Find nodes matching an <paramref name="item"/> within the <paramref name="root"/>.
	/// </summary>
	public static IEnumerable<Node<TItem>> Find<TItem>(this Node<TItem>? root, TItem item, IEqualityComparer<TItem>? comparer = default, bool detectCycles = false)
		where TItem : notnull
	{
		comparer ??= EqualityComparer<TItem>.Default;
		return root.Find((node, signal) => comparer.Equals(node.Item, item), detectCycles);
	}

	/// <summary>
	/// Find nodes matching an <paramref name="item"/> within the <paramref name="roots"/>.
	/// </summary>
	public static IEnumerable<Node<TItem>> Find<TItem>(this IEnumerable<Node<TItem>> roots, TItem item, IEqualityComparer<TItem>? comparer = default, bool detectCycles = false)
		where TItem : notnull
	{
		comparer ??= EqualityComparer<TItem>.Default;
		return roots.Find((node, signal) => comparer.Equals(node.Item, item), detectCycles);
	}
}
