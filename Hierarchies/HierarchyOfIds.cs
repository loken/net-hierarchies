namespace Loken.Hierarchies;

/// <summary>
/// A <see cref="Hierarchy{TId,TId}"/> where the items are the IDs.
/// Created through the <see cref="Hierarchies"/> factory class.
/// </summary>
/// <remarks>
/// These can be useful when you don't want to keep all of the items in memory,
/// but want to be able to reason around the relationships in a more light weight manner.
/// </remarks>
public class Hierarchy<TId> : Hierarchy<TId, TId>
	where TId : notnull
{
	internal Hierarchy() : base(id => id)
	{
	}
}