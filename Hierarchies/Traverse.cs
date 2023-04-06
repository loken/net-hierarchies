using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Loken.System.Collections;

namespace Loken.Hierarchies;

/// <summary>
/// Provides traversal for sequences, trees and graphs.
/// </summary>
public static class Traverse
{
	/// <summary>
	/// Traverse a sequence of elements.
	/// </summary>
	/// <typeparam name="TEl">The type of element.</typeparam>
	/// <param name="element">The starting element.</param>
	/// <param name="next">Describes what the next element of the sequence should be, if any.</param>
	/// <returns>An enumeration of elements.</returns>
	public static IEnumerable<TEl> Sequence<TEl>(TEl element, Func<TEl, TEl?> next)
		where TEl : notnull
	{
		return Sequence(element, (n, s) => s.Next(next(n)));
	}

	/// <summary>
	/// Traverse a sequence of elements.
	/// <para>Use the <see cref="SequenceSignal{T}"/> to provide a next element
	/// and whether to include or skip the current element.</para>
	/// <para>By providing no next element you end the sequence.</para>
	/// </summary>
	/// <typeparam name="TEl">The type of element.</typeparam>
	/// <param name="element">The starting element.</param>
	/// <param name="traverse">The traversal action where you detail what's next and what to skip.</param>
	/// <returns>An enumeration of elements.</returns>
	public static IEnumerable<TEl> Sequence<TEl>(TEl element, Action<TEl, SequenceSignal<TEl>> traverse)
		where TEl : notnull
	{
		var signal = new SequenceSignal<TEl>(element);

		while (signal.TryGetNext(out TEl? current))
		{
			traverse(current, signal);

			if (!signal.Skipped)
				yield return current;
		}
	}

	/// <summary>
	/// Traverse a tree of roots.
	/// </summary>
	/// <remarks>A tree should not have any cycles. If it does,
	/// use <see cref="Graph{TNode}(TNode, Func{TNode, IEnumerable{TNode}})"/> instead.</remarks>
	/// <typeparam name="TNode">The type of root.</typeparam>
	/// <param name="root">The root may have a parent, but it is treated as a depth 0 root for the traversal.</param>
	/// <param name="traverse">The traversal action where you detail what's next and what to skip.</param>
	/// <returns>An enumeration of roots.</returns>
	public static IEnumerable<TNode> Tree<TNode>(TNode root, Func<TNode, IEnumerable<TNode>> next)
		where TNode : notnull
	{
		return Tree(new[] { root }, (n, s) => s.Next(next(n)));
	}

	/// <summary>
	/// Traverse a tree of roots.
	/// <para>Use the <see cref="GraphSignal{T}"/> to provide the next roots
	/// and whether to include or skip the current root.</para>
	/// <para>By providing no next roots you exclude the children of the current root from traversal.</para>
	/// </summary>
	/// <remarks>A tree should not have any cycles. If it does,
	/// use <see cref="Graph{TNode}(TNode, Action{TNode, GraphSignal{TNode}})"/> instead.</remarks>
	/// <typeparam name="TNode">The type of root.</typeparam>
	/// <param name="root">The root may have a parent, but it is treated as a depth 0 root for the traversal.</param>
	/// <param name="traverse">The traversal action where you detail what's next and what to skip.</param>
	/// <returns>An enumeration of roots.</returns>
	public static IEnumerable<TNode> Tree<TNode>(TNode root, Action<TNode, GraphSignal<TNode>> traverse)
		where TNode : notnull
	{
		return Tree(new[] { root }, traverse);
	}

	/// <summary>
	/// Traverse a tree of roots.
	/// <para>Use the <see cref="GraphSignal{T}"/> to provide the next roots
	/// and whether to include or skip the current root.</para>
	/// <para>By providing no next roots you exclude the children of the current root from traversal.</para>
	/// </summary>
	/// <remarks>A tree should not have any cycles. If it does,
	/// use <see cref="Graph{TNode}(TNode, Action{TNode, GraphSignal{TNode}})"/> instead.</remarks>
	/// <typeparam name="TNode">The type of root.</typeparam>
	/// <param name="roots">The roots may have parents, but they are treated as depth 0 nodes for the traversal.</param>
	/// <param name="traverse">The traversal action where you detail what's next and what to skip.</param>
	/// <returns>An enumeration of roots.</returns>
	public static IEnumerable<TNode> Tree<TNode>(IEnumerable<TNode> roots, Action<TNode, GraphSignal<TNode>> traverse)
		where TNode : notnull
	{
		var signal = new GraphSignal<TNode>(roots);

		while (signal.TryGetNext(out TNode? current))
		{
			traverse(current, signal);

			if (!signal.Skipped)
				yield return current;
		}
	}

	/// <summary>
	/// Traverse a graph of roots.
	/// <para>Similar to <see cref="Traverse.Tree"/>, but detects graph cycles.</para>
	/// </summary>
	/// <typeparam name="TNode">The type of root.</typeparam>
	/// <param name="root">The root may have a parent, but it is treated as a depth 0 root for the traversal.</param>
	/// <param name="traverse">The traversal action where you detail what's next and what to skip.</param>
	/// <returns>An enumeration of roots.</returns>
	public static IEnumerable<TNode> Graph<TNode>(TNode root, Func<TNode, IEnumerable<TNode>> next)
		where TNode : notnull
	{
		return Graph(new[] { root }, (n, s) => s.Next(next(n)));
	}

	/// <summary>
	/// Traverse a graph of roots.
	/// <para>Similar to <see cref="Traverse.Tree"/>, but detects graph cycles.</para>
	/// <para>Use the <see cref="GraphSignal{T}"/> to provide the next roots
	/// and whether to include or skip the current root.</para>
	/// <para>By providing no next roots you exclude the children of the current root from traversal.</para>
	/// </summary>
	/// <typeparam name="TNode">The type of root.</typeparam>
	/// <param name="root">The root may have a parent, but it is treated as a depth 0 node for the traversal.</param>
	/// <param name="traverse">The traversal action where you detail what's next and what to skip.</param>
	/// <returns>An enumeration of roots.</returns>
	public static IEnumerable<TNode> Graph<TNode>(TNode root, Action<TNode, GraphSignal<TNode>> traverse)
		where TNode : notnull
	{
		return Graph(new[] { root }, traverse);
	}


	/// <summary>
	/// Traverse a graph of roots.
	/// <para>Similar to <see cref="Traverse.Tree"/>, but detects graph cycles.</para>
	/// <para>Use the <see cref="GraphSignal{T}"/> to provide the next roots
	/// and whether to include or skip the current root.</para>
	/// <para>By providing no next roots you exclude the children of the current root from traversal.</para>
	/// </summary>
	/// <typeparam name="TNode">The type of root.</typeparam>
	/// <param name="roots">The roots may have parents, but they are treated as depth 0 nodes for the traversal.</param>
	/// <param name="traverse">The traversal action where you detail what's next and what to skip.</param>
	/// <returns>An enumeration of roots.</returns>
	public static IEnumerable<TNode> Graph<TNode>(IEnumerable<TNode> roots, Action<TNode, GraphSignal<TNode>> traverse)
		where TNode : notnull
	{
		// We use a set of hash codes instead of a set of roots for detecting cycles.
		// This way we consume less memory. If we were to keep the roots in the visited set
		// the GC could not clean up any enumerated roots until we are done traversing the graph.
		var visited = new HashSet<int>();

		var signal = new GraphSignal<TNode>(roots);

		while (signal.TryGetNext(out TNode? current))
		{
			if (!visited.Add(current.GetHashCode()))
				continue;

			traverse(current, signal);

			if (!signal.Skipped)
				yield return current;
		}
	}

	/// <summary>
	/// Use this to signal to the traversal what's <see cref="Next"/> and what to <see cref="Skip"/>.
	/// </summary>
	public sealed class SequenceSignal<TEl>
		where TEl : notnull
	{
		private TEl? NextEl;

		internal bool Skipped { get; private set; }

		internal SequenceSignal(TEl element)
		{
			NextEl = element;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool TryGetNext([MaybeNullWhen(false)] out TEl element)
		{
			if (NextEl is not null)
			{
				element = NextEl;

				Skipped = false;
				NextEl = default;

				return true;
			}
			else
			{
				element = default;
				return false;
			}
		}

		/// <summary>
		/// Call this when you want to signal that the current element should be skipped,
		/// meaning it will not be part of the output.
		/// <para>Traversal will still continue to an element passed to
		/// <see cref="Next"/> irrespective of calling <see cref="Skip"/>.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Skip() => Skipped = true;

		/// <summary>
		/// Call this when traversal should continue to a sub sequence of child roots.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Next(TEl? element)
		{
			NextEl = element;
		}
	}

	/// <summary>
	/// Use this to signal to the traversal what's <see cref="Next"/> and what to <see cref="Skip"/>.
	/// </summary>
	public sealed class GraphSignal<TNode>
		where TNode : notnull
	{
		private readonly Queue<TNode> Queue = new();

		internal bool Skipped { get; private set; }

		internal GraphSignal(IEnumerable<TNode> roots)
		{
			Queue.Enqueue(roots);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool TryGetNext([MaybeNullWhen(false)] out TNode node)
		{
			if (Queue.TryDequeue(out node))
			{
				Skipped = false;
				return true;
			}
			else
			{
				node = default;
				return false;
			}
		}

		/// <summary>
		/// Call this when you want to signal that the current root should be skipped,
		/// meaning it will not be part of the output.
		/// <para>Traversal will still continue to whatever roots are passed to
		/// <see cref="Next"/> irrespective of calling <see cref="Skip"/>.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Skip() => Skipped = true;

		/// <summary>
		/// Call this when traversal should continue to a sub sequence of child roots.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Next(params TNode[] nodes) => Queue.Enqueue(nodes);

		/// <summary>
		/// Call this when traversal should continue to a sub sequence of child roots.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Next(IEnumerable<TNode> nodes) => Queue.Enqueue(nodes);
	}

}
