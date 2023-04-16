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
	/// Used for discriminating which <see cref="Hierarchy{TItem, TId}"/>
	/// the relation should be synchronized to.
	/// </summary>
	public required string Concept { get; set; }

	/// <summary>
	/// The ID of the node that has a ancestors, children or descendants.
	/// </summary>
	public required TId Id { get; set; }

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
