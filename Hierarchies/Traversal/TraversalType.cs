namespace Loken.Hierarchies.Traversal;

/// <summary>
/// The type of traversal.
/// </summary>
public enum TraversalType
{
	/// <summary>
	/// Breadth first processes each node at a given depth before it proceeds to the next depth.
	/// </summary>
	BreadthFirst = 0,

	/// <summary>
	/// Depth first traverses as deep as it can at any given time only
	/// exploring the next branch once the previous one has been fully explored.
	/// </summary>
	DepthFirst = 1,
}
