namespace Loken.Hierarchies.Traversal;

/// <summary>
/// Whether to add nodes in a forward (first-to-last) or reverse (last-to-first) order when traversing siblings.
/// This affects the order in which nodes are visited during traversal.
/// </summary>
public enum SiblingOrder
{
	Forward = 0,
	Reverse = 1,
}


/// <summary>
/// Ancestor traversal options (ascending the graph).
///
/// Guidance & defaults:
/// - Omitted (<c>null</c>) values are treated as unspecified so the calling API can inject its context-specific default.
/// - Naming/default rule: APIs whose name references <c>Ancestor</c>/<c>Ancestors</c> default <see cref="IncludeSelf"/> to <c>false</c> (start from the parent).
/// - Internal utility methods may default to including self when treating the starting node as part of the logical chain.
///
/// Rationale: Ancestor queries typically mean "walk upward" which conceptually begins at the parent unless you explicitly opt into including the starting node itself.
/// </summary>
/// <param name="IncludeSelf">Whether to treat the starting node itself as part of the ancestor sequence. <c>null</c> lets the API supply its default.</param>
public readonly record struct Ascend(
	bool? IncludeSelf = null)
{
	/// <summary>
	/// A default set of traversal options: breadth-first, excluding the root node, not detecting cycles, and traversing siblings in forward order.
	/// </summary>
	public static Ascend Default { get; } = new();

	/// <summary>
	/// A set of traversal options that includes the root node in the traversal.
	/// </summary>
	public static Ascend WithSelf { get; } = new(IncludeSelf: true);

	/// <summary>
	/// A set of traversal options that excludes the root node in the traversal.
	/// </summary>
	public static Ascend WithoutSelf { get; } = new(IncludeSelf: false);

	// Normalize possibly unspecified TraversalOptions for a given API default for IncludeSelf.
	// Behavior mirrors prior GraphTraversalCore.Normalize implementation.
	internal static Ascend Normalize(in Ascend? options, bool includeSelfDefault)
	{
		if (options is Ascend value)
		{
			if (value.IncludeSelf is null)
				return value with { IncludeSelf = includeSelfDefault };
			return value;
		}

		return new Ascend(
			IncludeSelf: includeSelfDefault
		);
	}
}


/// <summary>
/// Descendant traversal options (descending the graph).
///
/// Guidance & defaults:
/// - Omitted (<c>null</c>) values are treated as unspecified so the calling API can inject its context-specific default.
/// - Naming/default rule: APIs whose name references <c>Descendant</c>/<c>Descendants</c> default <see cref="IncludeSelf"/> to <c>false</c> (start from children).
/// - General traversal helpers such as flatten, traverse, or search default to including the provided roots (<c>true</c>) for ergonomic top-down iteration.
///
/// Shorthands: use the static members for common configurations or pass a <see cref="TraversalType"/> value directly.
/// </summary>
/// <param name="IncludeSelf">Whether to include the starting/root node(s) in the traversal. <c>null</c> allows API-specific defaults.</param>
/// <param name="Type">Traverse in a breadth-first or depth-first manner.</param>
/// <param name="DetectCycles">Whether to detect cycles during traversal.</param>
/// <param name="Siblings">The order in which to traverse sibling nodes.</param>
public readonly record struct Descend(
	bool? IncludeSelf = null,
	TraversalType Type = TraversalType.BreadthFirst,
	bool DetectCycles = false,
	SiblingOrder Siblings = SiblingOrder.Forward)
{
	// Implicit conversions for succinct call sites, e.g. options: true or options: TraversalType.DepthFirst
	public static implicit operator Descend(TraversalType type) => new(Type: type);

	/// <summary>
	/// A default set of traversal options: breadth-first, excluding the root node, not detecting cycles, and traversing siblings in forward order.
	/// </summary>
	public static Descend Default { get; } = new();

	/// <summary>
	/// A set of traversal options that includes the root node in the traversal.
	/// </summary>
	public static Descend WithSelf { get; } = new(IncludeSelf: true);

	/// <summary>
	/// A set of traversal options that excludes the root node in the traversal.
	/// </summary>
	public static Descend WithoutSelf { get; } = new(IncludeSelf: false);

	/// <summary>
	/// A set of traversal options for breadth-first traversal that includes the root node.
	/// </summary>
	public static Descend BreadthFirstWithSelf { get; } = new(Type: TraversalType.BreadthFirst, IncludeSelf: true);

	/// <summary>
	/// A set of traversal options for breadth-first traversal that excludes the root node.
	/// </summary>
	public static Descend BreadthFirstWithoutSelf { get; } = new(Type: TraversalType.BreadthFirst, IncludeSelf: false);

	/// <summary>
	/// A set of traversal options for depth-first traversal that includes the root node.
	/// </summary>
	public static Descend DepthFirstWithSelf { get; } = new(Type: TraversalType.DepthFirst, IncludeSelf: true);

	/// <summary>
	/// A set of traversal options for depth-first traversal that excludes the root node.
	/// </summary>
	public static Descend DepthFirstWithoutSelf { get; } = new(Type: TraversalType.DepthFirst, IncludeSelf: false);

	// Normalize possibly unspecified TraversalOptions for a given API default for IncludeSelf.
	// Behavior mirrors prior GraphTraversalCore.Normalize implementation.
	internal static Descend Normalize(in Descend? options, bool includeSelfDefault)
	{
		if (options is Descend value)
		{
			if (value.IncludeSelf is null)
				return value with { IncludeSelf = includeSelfDefault };
			return value;
		}

		return new Descend(
			IncludeSelf: includeSelfDefault,
			Type: Default.Type,
			DetectCycles: Default.DetectCycles,
			Siblings: Default.Siblings
		);
	}
}
