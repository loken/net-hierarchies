namespace Loken.Hierarchies;

/// <summary>
/// A <see cref="Hierarchy{TId,TId}"/> where the items are the ids.
/// Created through <see cref="Hierarchy"/>.
/// </summary>
/// <remarks>
/// These can be useful when you don't want to keep all of the items in memory,
/// but want to be able to reason around the relationships in a more light weight manner.
/// </remarks>
public class Hierarchy<TId> : Hierarchy<TId, TId>
	where TId : notnull
{
	internal Hierarchy() : base(t => t)
	{
	}

	internal Hierarchy(params TId[] items) : base(t => t, items)
	{
	}

	internal Hierarchy(IEnumerable<TId> items) : base(t => t, items)
	{
	}
}