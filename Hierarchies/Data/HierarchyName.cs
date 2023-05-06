namespace Loken.Hierarchies.Data;

/// <summary>
/// Knows how to create table/collection names for holding <see cref="HierarchyRelation{TId}"/>s.
/// </summary>
/// <remarks>
/// The methods take a generic type parameter for the type of IDs
/// so that we don't mix relationships of different types of IDs.
/// </remarks>
public static class HierarchyName
{
	/// <summary>
	/// Create a table/collection name for holding node-to-children relations.
	/// </summary>
	public static string For<TId>() where TId : notnull => $"Loken_Hierarchies_{typeof(TId).Name}";
}
