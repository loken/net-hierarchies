namespace Loken.Hierarchies;

/// <summary>
/// Extensions for cloning hierarchies.
/// </summary>
public static class HierarchyCloneExtensions
{
	/// <summary>
	/// Create a clone of an existing item hierarchy with the same structure and items, but new node instances.
	/// This is a fast alternative to complex search operations for simple cloning scenarios.
	/// </summary>
	/// <typeparam name="TItem">The type of item.</typeparam>
	/// <typeparam name="TId">The type of identifier.</typeparam>
	/// <param name="hierarchy">The hierarchy to clone.</param>
	/// <returns>A new <see cref="Hierarchy{TItem, TId}"/> with the same structure and items but new nodes.</returns>
	public static Hierarchy<TItem, TId> Clone<TItem, TId>(this Hierarchy<TItem, TId> hierarchy)
		where TItem : notnull
		where TId : notnull
	{
		var childMap = hierarchy.ToChildMap();
		return Hierarchies.CreateFromChildMap(hierarchy.NodeItems, hierarchy.Identify, childMap);
	}

	/// <summary>
	/// Create a clone of an existing hierarchy with the same structure but new node instances for IDs.
	/// </summary>
	/// <typeparam name="TId">The type of identifier.</typeparam>
	/// <param name="hierarchy">The hierarchy to clone.</param>
	/// <returns>A new <see cref="Hierarchy{TId}"/> with the same structure but new nodes.</returns>
	public static Hierarchy<TId> CloneIds<TItem, TId>(this Hierarchy<TItem, TId> hierarchy)
		where TItem : notnull
		where TId : notnull
	{
		var childMap = hierarchy.ToChildMap();
		return Hierarchies.CreateFromChildMap(childMap);
	}
}
