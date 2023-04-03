namespace Loken.Hierarchies;

public class Relation<TId>
	where TId : notnull
{
	public required TId Parent { get; init; }

	public required TId Child { get; init; }

	public static implicit operator Relation<TId>((TId parent, TId child) tuple)
	{
		return new Relation<TId>
		{
			Parent = tuple.parent,
			Child = tuple.child
		};
	}

	public static implicit operator (TId parent, TId child)(Relation<TId> relation)
	{
		return (relation.Parent, relation.Child);
	}
}