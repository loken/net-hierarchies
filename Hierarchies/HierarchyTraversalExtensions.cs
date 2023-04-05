using Loken.System.Collections;

namespace Loken.Hierarchies;

/// <summary>
/// Predicate answering if an <paramref name="item"/> at a provided <paramref name="depth"/> is a result or not.
/// </summary>
/// <typeparam name="TItem">The <see cref="Type"/> of <paramref name="item"/> to result.</typeparam>
/// <param name="item">The <typeparamref name="TItem"/> to check.</param>
/// <param name="depth">The number of levels traversed when encountering the <paramref name="item"/>. Starts at 0 for roots.</param>
/// <returns>True if the <paramref name="item"/> is a result, False otherwise.</returns>
public delegate bool Match<in TItem>(TItem item, uint depth)
	where TItem : notnull;

/// <summary>
/// Transform the <paramref name="item"/> at a provided <paramref name="depth"/> into a <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TItem">The <see cref="Type"/> of item to transform from.</typeparam>
/// <typeparam name="TResult">The <see cref="Type"/> of result to transform to.</typeparam>
/// <param name="item">The <typeparamref name="TItem"/> to transform.</param>
/// <param name="depth">The number of levels traversed when encountering the <paramref name="item"/>. Starts at 0 for roots.</param>
/// <returns>The transformation result.</returns>
public delegate TResult Transform<in TItem, out TResult>(TItem item, uint depth)
	where TItem : notnull
	where TResult : notnull;

/// <summary>
/// Traversal extensions for <see cref="Hierarchy{TItem,TId}"/>.
/// Uses a breadth first approach.
/// Search starts with the <see cref="Hierarchy{TItem,TId}.Roots"/>,
/// meaning that detached <see cref="Hierarchy{TItem,TId}.Nodes"/> will not be included.
/// </summary>
public static class HierarchyTraversalExtensions
{
	/// <summary>
	/// Find the <typeparamref name="TItem"/> matching the <paramref name="match"/>.
	/// </summary>
	public static TItem? Find<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, Match<TItem> match)
		where TId : notnull
		where TItem : notnull
	{
		Node<TItem>? node = hierarchy.FindNode((node, depth) => match(node.Value, depth));
		return node is null ? default : node.Value;
	}

	/// <summary>
	/// Find the <see cref="Node{TItem}"/> matching the <paramref name="match"/>.
	/// </summary>
	public static Node<TItem>? FindNode<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, Match<Node<TItem>> match)
		where TId : notnull
		where TItem : notnull
	{
		Node<TItem>? result = null;
		hierarchy.Traverse((node, depth) =>
		{
			if (match(node, depth))
			{
				result = node;
				return true;
			}
			return false;
		});

		return result;
	}

	/// <summary>
	/// Traverse the <paramref name="hierarchy"/> and <paramref name="match"/> each <see cref="Node{TItem}"/>.
	/// Traversal will stop as soon as <paramref name="match"/> returns <c>true</c>.
	/// </summary>
	public static void Traverse<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, Match<Node<TItem>> match)
		where TId : notnull
		where TItem : notnull
	{
		var queue = new Queue<Node<TItem>>(hierarchy.Roots);
		var depth = 0u;
		var depthCount = queue.Count;

		while (queue.Count > 0)
		{
			Node<TItem> node = queue.Dequeue();
			if (depthCount-- == 0)
			{
				depth++;
				depthCount = queue.Count;
			}

			if (match(node, depth))
				return;

			if (!node.IsLeaf)
				queue.Enqueue(node.Children);
		}
	}

	/// <summary>
	/// Traverse the <paramref name="hierarchy"/> and yield the <see cref="Node{TItem}"/> <paramref name="transform"/> results.
	/// </summary>
	public static IEnumerable<TResult> Transform<TItem, TId, TResult>(this Hierarchy<TItem, TId> hierarchy, Transform<Node<TItem>, TResult> transform)
		where TId : notnull
		where TItem : notnull
		where TResult : notnull
	{
		var queue = new Queue<Node<TItem>>(hierarchy.Roots);
		var depth = 0u;
		var depthCount = queue.Count;

		while (queue.Count > 0)
		{
			Node<TItem> node = queue.Dequeue();
			if (depthCount-- == 0)
			{
				depth++;
				depthCount = queue.Count;
			}

			yield return transform(node, depth);

			if (!node.IsLeaf)
				queue.Enqueue(node.Children);
		}
	}

	/// <summary>
	/// Enumerate the <paramref name="hierarchy"/> <see cref="Node{TItem}"/>s.
	/// </summary>
	public static IEnumerable<Node<TItem>> EnumerateNodes<TItem, TId>(this Hierarchy<TItem, TId> hierarchy)
		where TId : notnull
		where TItem : notnull
	{
		var queue = new Queue<Node<TItem>>(hierarchy.Roots);

		while (queue.Count > 0)
		{
			var node = queue.Dequeue();
			yield return node;

			if (!node.IsLeaf)
				queue.Enqueue(node.Children);
		}
	}

	/// <summary>
	/// Enumerate the <paramref name="hierarchy"/> <typeparamref name="TItem"/>s.
	/// </summary>
	public static IEnumerable<TItem> Enumerate<TItem, TId>(this Hierarchy<TItem, TId> hierarchy)
		where TId : notnull
		where TItem : notnull
	{
		return hierarchy.EnumerateNodes().Select(n => n.Value);
	}
}