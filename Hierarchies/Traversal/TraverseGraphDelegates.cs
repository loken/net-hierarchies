namespace Loken.Hierarchies.Traversal;

/// <summary>
/// Describes how to get the the next nodes from a node visited while traversing a graph.
/// </summary>
/// <typeparam name="TNode">The type of node.</typeparam>
/// <param name="node">The source node.</param>
/// <returns>An enumeration of the next nodes.</returns>
public delegate IEnumerable<TNode> NextNodes<TNode>(TNode node)
	where TNode : notnull;

/// <summary>
/// Describes how to traverse a graph when visiting a node using a <see cref="GraphSignal{TNode}"/>.
/// </summary>
/// <typeparam name="TNode">The type of node.</typeparam>
/// <param name="node">The source node.</param>
/// <param name="signal">Use this to signal how to traverse.</param>
public delegate void TraverseNode<TNode>(TNode node, GraphSignal<TNode> signal)
	where TNode : notnull;
