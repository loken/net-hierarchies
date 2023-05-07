namespace Loken.Hierarchies.Data;

/// <summary>
/// Represents the difference between two states of a hierarchy.
/// </summary>
/// <remarks>
/// The intent is to generate the diff from what a hierarchy used to be and what it is now,
/// and then generate database updates from the diff, appropriate for whatever database is in use.
/// This way generating the diff is common functionality and each driver only needs to know how
/// to turn the diff into updates.
/// </remarks>
/// <typeparam name="TId">Type of ID.</typeparam>
public class HierarchyDiff<TId>
	where TId : notnull
{
	/// <summary>
	/// Each <see cref="Hierarchy{TItem, TId}"/> represents a unique concept.
	/// By tracking it here we can create a batch update for multiple concepts.
	/// </summary>
	public required string Concept { get; init; }

	/// <summary>
	/// Each <see cref="Hierarchy{TItem, TId}"/> can be viewed and queried on different
	/// types of relationships.
	/// By tracking it here we can create a batch update for multiple relationship types.
	/// </summary>
	public required RelType Type { get; init; }

	/// <summary>
	/// Entries that existed prior but no longer exists.
	/// </summary>
	public ISet<TId> Deleted { get; } = new HashSet<TId>();

	/// <summary>
	/// Entries that didn't exist prior.
	/// </summary>
	public IDictionary<TId, ISet<TId>> Inserted { get; } = new Dictionary<TId, ISet<TId>>();

	/// <summary>
	/// Entries that have had some targets removed.
	/// </summary>
	public IDictionary<TId, ISet<TId>> Removed { get; } = new Dictionary<TId, ISet<TId>>();

	/// <summary>
	/// Entries that have had some targets added.
	/// </summary>
	public IDictionary<TId, ISet<TId>> Added { get; } = new Dictionary<TId, ISet<TId>>();
}
