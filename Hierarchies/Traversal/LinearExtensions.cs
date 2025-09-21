using System.Runtime.CompilerServices;

namespace Loken.Hierarchies.Traversal;

internal static class LinearExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AttachManySpecific<TNode>(this ILinear<TNode> linear, IEnumerable<TNode>? nodes, bool reverse = false)
	{
		if (nodes is null)
			return;

		if (nodes is IList<TNode> childList)
			linear.AttachMany(childList, reverse);
		else if (nodes is ICollection<TNode> childCollection)
			linear.AttachMany(childCollection, reverse);
		else
			linear.AttachMany(nodes, reverse);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AttachNext<TNode>(this ILinear<TNode> linear, TNode node, Func<TNode, IEnumerable<TNode>?> next, bool includeSelf = false, bool reverse = false)
	{
		if (includeSelf)
			linear.Attach(node);
		else
			linear.AttachManySpecific(next(node), reverse);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AttachNext<TNode>(this ILinear<TNode> linear, IEnumerable<TNode> nodes, Func<TNode, IEnumerable<TNode>?> next, bool includeSelf = false, bool reverse = false)
	{
		if (includeSelf)
		{
			linear.AttachMany(nodes, reverse);
		}
		else
		{
			foreach (var root in nodes)
				linear.AttachManySpecific(next(root), reverse);
		}
	}
}
