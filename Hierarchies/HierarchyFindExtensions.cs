namespace Loken.Hierarchies;

/// <summary>
/// Extensions providing general find operations for hierarchies.
/// </summary>
public static class HierarchyFindExtensions
{
	#region Find
	/// <summary>
	/// Find nodes matching a single ID.
	/// </summary>
	/// <param name="hierarchy">The hierarchy to search in.</param>
	/// <param name="id">The ID to search for.</param>
	/// <returns>An array of matching nodes.</returns>
	public static IList<Node<TItem>> Find<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id)
		where TItem : notnull
		where TId : notnull
	{
		return hierarchy.TryGetNode(id, out var node) ? [node] : [];
	}

	/// <summary>
	/// Find nodes matching a list of IDs.
	/// </summary>
	/// <param name="hierarchy">The hierarchy to search in.</param>
	/// <param name="ids">The IDs to search for.</param>
	/// <returns>An array of matching nodes.</returns>
	public static IList<Node<TItem>> Find<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids)
		where TItem : notnull
		where TId : notnull
	{
		var nodes = new List<Node<TItem>>();

		foreach (var id in ids)
		{
			if (hierarchy.TryGetNode(id, out var node))
				nodes.Add(node);
		}

		return nodes;
	}

	/// <summary>
	/// Find nodes matching a predicate.
	/// </summary>
	/// <param name="hierarchy">The hierarchy to search in.</param>
	/// <param name="predicate">The predicate function to match nodes.</param>
	/// <returns>An array of matching nodes.</returns>
	public static IList<Node<TItem>> Find<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, Func<Node<TItem>, bool> predicate)
		where TItem : notnull
		where TId : notnull
	{
		var results = new List<Node<TItem>>();

		foreach (var node in hierarchy.Nodes)
		{
			if (predicate(node))
				results.Add(node);
		}

		return results;
	}
	#endregion

	#region FindItems
	/// <summary>
	/// Find items matching a single ID.
	/// </summary>
	/// <param name="hierarchy">The hierarchy to search in.</param>
	/// <param name="id">The ID to search for.</param>
	/// <returns>An array of matching items.</returns>
	public static IList<TItem> FindItems<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id)
		where TItem : notnull
		where TId : notnull
	{
		return hierarchy.TryGetNode(id, out var node) ? [node.Item] : [];
	}

	/// <summary>
	/// Find items matching a list of IDs.
	/// </summary>
	/// <param name="hierarchy">The hierarchy to search in.</param>
	/// <param name="ids">The IDs to search for.</param>
	/// <returns>An array of matching items.</returns>
	public static IList<TItem> FindItems<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids)
		where TItem : notnull
		where TId : notnull
	{
		var results = new List<TItem>();

		foreach (var id in ids)
		{
			if (hierarchy.TryGetNode(id, out var node))
				results.Add(node.Item);
		}

		return results;
	}

	/// <summary>
	/// Find items matching a predicate.
	/// </summary>
	/// <param name="hierarchy">The hierarchy to search in.</param>
	/// <param name="predicate">The predicate function to match nodes.</param>
	/// <returns>An array of matching items.</returns>
	public static IList<TItem> FindItems<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, Func<Node<TItem>, bool> predicate)
		where TItem : notnull
		where TId : notnull
	{
		var results = new List<TItem>();

		foreach (var node in hierarchy.Nodes)
		{
			if (predicate(node))
				results.Add(node.Item);
		}

		return results;
	}
	#endregion

	#region FindIds
	/// <summary>
	/// Find IDs matching a single ID.
	/// </summary>
	/// <param name="hierarchy">The hierarchy to search in.</param>
	/// <param name="id">The ID to search for.</param>
	/// <returns>An array of matching IDs.</returns>
	public static IList<TId> FindIds<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, TId id)
		where TItem : notnull
		where TId : notnull
	{
		return hierarchy.TryGetNode(id, out var node) ? [id] : [];
	}

	/// <summary>
	/// Find IDs matching a list of IDs.
	/// </summary>
	/// <param name="hierarchy">The hierarchy to search in.</param>
	/// <param name="ids">The IDs to search for.</param>
	/// <returns>An array of matching IDs.</returns>
	public static IList<TId> FindIds<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, IEnumerable<TId> ids)
		where TItem : notnull
		where TId : notnull
	{
		var results = new List<TId>();

		foreach (var id in ids)
		{
			if (hierarchy.TryGetNode(id, out var node))
				results.Add(id);
		}

		return results;
	}

	/// <summary>
	/// Find IDs matching a predicate.
	/// </summary>
	/// <param name="hierarchy">The hierarchy to search in.</param>
	/// <param name="predicate">The predicate function to match nodes.</param>
	/// <returns>An array of matching IDs.</returns>
	public static IList<TId> FindIds<TItem, TId>(this Hierarchy<TItem, TId> hierarchy, Func<Node<TItem>, bool> predicate)
		where TItem : notnull
		where TId : notnull
	{
		var results = new List<TId>();

		foreach (var node in hierarchy.Nodes)
		{
			if (predicate(node))
				results.Add(hierarchy.Identify(node.Item));
		}

		return results;
	}
	#endregion
}
