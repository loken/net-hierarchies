namespace Loken.Hierarchies.Data;

/// <summary>
/// Used for deciding which types of relations should be to use.
/// </summary>
[Flags]
public enum RelType
{
	/// <summary>
	/// No relations.
	/// </summary>
	None = 0x000,

	/// <summary>
	/// Node to direct children.
	/// </summary>
	Children = 0x001,

	/// <summary>
	/// Node to all of its descendants.
	/// </summary>
	/// <remarks>A descendant is a node that can be reached by traversing the graph of children.</remarks>
	Descendants = 0x010,

	/// <summary>
	/// Node to all of its ancestors.
	/// </summary>
	/// <remarks>An ancestor is a node that can be reached by traversing the sequence of parents.</remarks>
	Ancestors = 0x100,

	/// <summary>
	/// All of the relations.
	/// </summary>
	All = 0x111,
}
