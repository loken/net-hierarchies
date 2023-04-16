namespace Loken.Hierarchies.Data;

/// <summary>
/// Knows how to create table/collection names for holding <see cref="HierarchyRelation{TId}"/>s.
/// </summary>
/// <remarks>
/// The methods take a generic type parameter for the type of IDs
/// so that we don't mix relationships of different types of IDs.
/// </remarks>
public static class HierarchyTable
{
	/// <summary>
	/// Create a table/collection name for holding node-to-children relations.
	/// </summary>
	public static string Children<TId>() where TId : notnull => $"Loken_Hierarchy{nameof(RelType.Children)}_{typeof(TId).Name}";

	/// <summary>
	/// Create a table/collection name for holding node-to-descendants relations.
	/// </summary>
	public static string Descendants<TId>() where TId : notnull => $"Loken_Hierarchy{nameof(RelType.Descendants)}_{typeof(TId).Name}";

	/// <summary>
	/// Create a table/collection name for holding node-to-ancestors relations.
	/// </summary>
	public static string Ancestors<TId>() where TId : notnull => $"Loken_Hierarchy{nameof(RelType.Ancestors)}_{typeof(TId).Name}";
}
