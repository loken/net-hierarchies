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

		while (signal.NextElement is not null)
		{
			TEl current = signal.NextElement;

			signal.Reset();
			traverse(current, signal);

			if (!signal.Skipped)
				yield return current;
		}
	}

	/// <summary>
	/// Traverse a tree of nodes.
	/// </summary>
	/// <remarks>A tree should not have any cycles. If it does,
	/// use <see cref="Graph{TNode}(TNode, Func{TNode, IEnumerable{TNode}})"/> instead.</remarks>
	/// <typeparam name="TNode">The type of node.</typeparam>
	/// <param name="node">The starting node.</param>
	/// <param name="traverse">The traversal action where you detail what's next and what to skip.</param>
	/// <returns>An enumeration of nodes.</returns>
	public static IEnumerable<TNode> Tree<TNode>(TNode node, Func<TNode, IEnumerable<TNode>> next)
		where TNode : notnull
	{
		return Tree(node, (n, s) => s.Next(next(n)));
	}

	/// <summary>
	/// Traverse a tree of nodes.
	/// <para>Use the <see cref="GraphSignal{T}"/> to provide the next nodes
	/// and whether to include or skip the current node.</para>
	/// <para>By providing no next nodes you exclude the children of the current node from traversal.</para>
	/// </summary>
	/// <remarks>A tree should not have any cycles. If it does,
	/// use <see cref="Graph{TNode}(TNode, Action{TNode, GraphSignal{TNode}})"/> instead.</remarks>
	/// <typeparam name="TNode">The type of node.</typeparam>
	/// <param name="node">The starting node.</param>
	/// <param name="traverse">The traversal action where you detail what's next and what to skip.</param>
	/// <returns>An enumeration of nodes.</returns>
	public static IEnumerable<TNode> Tree<TNode>(TNode node, Action<TNode, GraphSignal<TNode>> traverse)
		where TNode : notnull
	{
		var queue = new Queue<TNode>();
		var signal = new GraphSignal<TNode>();

		queue.Enqueue(node);

		while (queue.Count > 0)
		{
			var current = queue.Dequeue();

			signal.Reset();
			traverse(current, signal);

			if (!signal.Skipped)
				yield return current;

			foreach (var next in signal.NextNodes)
			{
				queue.Enqueue(next);
			}
		}
	}

	/// <summary>
	/// Traverse a graph of nodes.
	/// <para>Similar to <see cref="Traverse.Tree"/>, but detects graph cycles.</para>
	/// </summary>
	/// <typeparam name="TNode">The type of node.</typeparam>
	/// <param name="node">The starting node.</param>
	/// <param name="traverse">The traversal action where you detail what's next and what to skip.</param>
	/// <returns>An enumeration of nodes.</returns>
	public static IEnumerable<TNode> Graph<TNode>(TNode node, Func<TNode, IEnumerable<TNode>> next)
		where TNode : notnull
	{
		return Graph(node, (n, s) => s.Next(next(n)));
	}

	/// <summary>
	/// Traverse a graph of nodes.
	/// <para>Similar to <see cref="Traverse.Tree"/>, but detects graph cycles.</para>
	/// <para>Use the <see cref="GraphSignal{T}"/> to provide the next nodes
	/// and whether to include or skip the current node.</para>
	/// <para>By providing no next nodes you exclude the children of the current node from traversal.</para>
	/// </summary>
	/// <typeparam name="TNode">The type of node.</typeparam>
	/// <param name="node">The starting node.</param>
	/// <param name="traverse">The traversal action where you detail what's next and what to skip.</param>
	/// <returns>An enumeration of nodes.</returns>
	public static IEnumerable<TNode> Graph<TNode>(TNode node, Action<TNode, GraphSignal<TNode>> traverse)
		where TNode : notnull
	{
		// We use a set of hash codes instead of a set of nodes for detecting cycles.
		// This way we consume less memory. If we were to keep the nodes in the visited set
		// the GC could not clean up any enumerated nodes until we are done traversing the graph.
		var visited = new HashSet<int>();

		var queue = new Queue<TNode>();
		var signal = new GraphSignal<TNode>();

		queue.Enqueue(node);

		while (queue.Count > 0)
		{
			var current = queue.Dequeue();

			if (!visited.Add(current.GetHashCode()))
				continue;

			signal.Reset();
			traverse(current, signal);

			if (!signal.Skipped)
				yield return current;

			foreach (var next in signal.NextNodes)
			{
				queue.Enqueue(next);
			}
		}
	}

	/// <summary>
	/// Use this to signal to the traversal what's <see cref="Next"/> and what to <see cref="Skip"/>.
	/// </summary>
	public sealed class SequenceSignal<TEl>
		where TEl : notnull
	{
		internal bool Skipped { get; private set; }
		internal TEl? NextElement { get; private set; }

		internal SequenceSignal(TEl element)
		{
			NextElement = element;
		}

		/// <summary>
		/// Call this when you want to signal that the current element should be skipped,
		/// meaning it will not be part of the output.
		/// <para>Traversal will still continue to an element passed to
		/// <see cref="Next"/> irrespective of calling <see cref="Skip"/>.</para>
		/// </summary>
		public void Skip() => Skipped = true;

		/// <summary>
		/// Call this when traversal should continue to a sub sequence of child nodes.
		/// </summary>
		public void Next(TEl? element)
		{
			NextElement = element;
		}

		internal void Reset()
		{
			Skipped = false;
			NextElement = default;
		}
	}

	/// <summary>
	/// Use this to signal to the traversal what's <see cref="Next"/> and what to <see cref="Skip"/>.
	/// </summary>
	public sealed class GraphSignal<T>
		where T : notnull
	{
		private readonly List<T> Nodes = new();

		internal bool Skipped { get; private set; }
		internal IEnumerable<T> NextNodes => Nodes?.AsEnumerable() ?? Enumerable.Empty<T>();

		internal GraphSignal(params T[] nodes)
		{
			Nodes.AddRange(nodes);
		}

		/// <summary>
		/// Call this when you want to signal that the current node should be skipped,
		/// meaning it will not be part of the output.
		/// <para>Traversal will still continue to whatever nodes are passed to
		/// <see cref="Next"/> irrespective of calling <see cref="Skip"/>.</para>
		/// </summary>
		public void Skip() => Skipped = true;

		/// <summary>
		/// Call this when traversal should continue to a sub sequence of child nodes.
		/// </summary>
		public void Next(params T[] nodes) => Nodes.AddRange(nodes);

		/// <summary>
		/// Call this when traversal should continue to a sub sequence of child nodes.
		/// </summary>
		public void Next(IEnumerable<T> nodes) => Nodes.AddRange(nodes);

		internal void Reset()
		{
			Skipped = false;
			Nodes.Clear();
		}
	}

}
