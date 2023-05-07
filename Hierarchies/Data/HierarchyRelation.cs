namespace Loken.Hierarchies.Data;

/// <summary>
/// Entity for storing relations to a database.
/// </summary>
/// <typeparam name="TId">Type of ID.</typeparam>
/// <remarks>
/// Does not contain a field for type of target because each type of target
/// is stored in a separate table/collection.
/// </remarks>
public class HierarchyRelation<TId>
	where TId : notnull
{
	/// <summary>
	/// Each <see cref="Hierarchy{TItem, TId}"/> represents a unique concept.
	/// By storing it, we can distinguish which concept a given set of relations applies to.
	/// </summary>
	public required string Concept { get; init; }

	/// <summary>
	/// Each <see cref="Hierarchy{TItem, TId}"/> can be viewed and queried on different
	/// types of relationships.
	/// By storing it, we can target a particular relationship type in our queries.
	/// </summary>
	public required RelType Type { get; init; }

	/// <summary>
	/// The ID of the node that has a ancestors, children or descendants.
	/// </summary>
	public required TId Id { get; init; }

	/// <summary>
	/// The IDs of the ancestors, children or descendants involved in the relation.
	/// </summary>
	public ISet<TId> Targets { get; init; } = new HashSet<TId>();

	/// <summary>
	/// All of the IDs, including both the <see cref="Id"/> and all of the <see cref="Targets"/>.
	/// </summary>
	public IEnumerable<TId> All
	{
		get
		{
			yield return Id;

			foreach (var target in Targets)
				yield return target;
		}
	}
}
