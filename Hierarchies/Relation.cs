using System.Diagnostics.CodeAnalysis;

namespace Loken.Hierarchies;

/// <summary>
/// A <see cref="Parent"/>-<see cref="Child"/> relationship describes the
/// identifiers that are connected.
/// </summary>
/// <typeparam name="TId">The type of identifier.</typeparam>
/// <remarks>
/// A <see cref="Relation{TId}"/> is a class so that it can be more easily stored in a database.
/// Some databases don't accept structs.
/// When <see cref="ParentOnly"/> is true, this represents a one-sided relation (isolated node).
/// </remarks>
public class Relation<TId>
	where TId : notnull
{
	public Relation(TId parent)
	{
		Parent = parent;
		Child = default;
		ParentOnly = true;
	}

	public Relation(TId parent, TId child)
	{
		Parent = parent;
		Child = child;
		ParentOnly = false;
	}

	/// <summary>
	/// The identifier of the parent.
	/// </summary>
	public TId Parent { get; }

	/// <summary>
	/// The identifier of the child.
	/// When null, this represents a one-sided relation (isolated node).
	/// </summary>
	public TId? Child { get; }

	/// <summary>
	/// Gets a value indicating whether this relation is one-sided (isolated node).
	/// </summary>
	[MemberNotNullWhen(false, nameof(Child))]
	public bool ParentOnly { get; }
}