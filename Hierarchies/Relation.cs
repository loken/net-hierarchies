namespace Loken.Hierarchies;

/// <summary>
/// A <see cref="Parent"/>-<see cref="Child"/> relationship describes the
/// identifiers that are connected.
/// </summary>
/// <typeparam name="TId">The type of identifier.</typeparam>
/// <remarks>
/// A <see cref="Relation{TId}"/> is a class so that it can be more easily stored in a database.
/// Some databases don't accept structs.
/// </remarks>
public class Relation<TId>
	where TId : notnull
{
	/// <summary>
	/// The identifier of the parent.
	/// </summary>
	public required TId Parent { get; init; }

	/// <summary>
	/// The identifier of the child.
	/// </summary>
	public required TId Child { get; init; }

	/// <summary>
	/// The implicit tuple-to-relation operator allow us to tersely instantiate relations.
	/// </summary>
	public static implicit operator Relation<TId>((TId parent, TId child) tuple)
	{
		return new Relation<TId>
		{
			Parent = tuple.parent,
			Child = tuple.child
		};
	}

	/// <summary>
	/// The implicit relation-to-tuple operator allow us to tersely unwrap a relation
	/// into a memory efficient tuple, which is a struct, unlike the <see cref="Relation{TId}"/>.
	/// </summary>
	/// <param name="relation"></param>
	public static implicit operator (TId parent, TId child)(Relation<TId> relation)
	{
		return (relation.Parent, relation.Child);
	}
}