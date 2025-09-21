namespace Loken.Hierarchies;

/// <summary>
/// Extensions providing a pruned hierarchy search similar to the TS library.
/// </summary>
public static class HierarchySearchExtensions
{
	/// <summary>
	/// Create a new hierarchy with nodes for a subset of matching nodes.
	/// </summary>
	/// <param name="hierarchy">The hierarchy to search within.</param>
	/// <param name="ids">The IDs to search for.</param>
	/// <param name="include">
	/// Flags controlling which nodes to include in the result:
	/// <list type="bullet">
	/// <item><description><see cref="SearchInclude.Matches"/> - Include the match itself</description></item>
	/// <item><description><see cref="SearchInclude.Ancestors"/> - Include all ancestors of a match</description></item>
	/// <item><description><see cref="SearchInclude.Descendants"/> - Include all descendants of a match</description></item>
	/// <item><description><see cref="SearchInclude.All"/> - Include <see cref="SearchInclude.Matches"/>, <see cref="SearchInclude.Ancestors"/> and <see cref="SearchInclude.Descendants"/></description></item>
	/// </list>
	/// </param>
	/// <returns>A new hierarchy containing only the specified subset of nodes - effectively a pruned clone.</returns>
	public static Hierarchy<TItem, TId> Search<TItem, TId>(
		this Hierarchy<TItem, TId> hierarchy,
		IEnumerable<TId> ids,
		SearchInclude include = SearchInclude.All)
		where TItem : notnull
		where TId : notnull
	{
		// Illegal to pass 0 in .NET API - callers must pick one or more facets.
		if (include == 0)
			throw new ArgumentException("You must include at least one facet: Matches, Ancestors, or Descendants.", nameof(include));

		var items = new Dictionary<TId, TItem>();
		var childMap = new MultiMap<TId>();

		// Unwrap flags once for hot paths
		var includeMatches = include.HasFlag(SearchInclude.Matches);
		var includeAncestors = include.HasFlag(SearchInclude.Ancestors);
		var includeDescendants = include.HasFlag(SearchInclude.Descendants);

		foreach (var id in ids)
		{
			// Ancestors facet
			if (includeAncestors)
			{
				var ancestorIds = new List<TId>();
				var ascend = includeMatches ? Ascend.WithSelf : (Ascend?)null;
				foreach (var node in hierarchy.GetAncestors(id, ascend))
				{
					var aId = hierarchy.Identify(node.Item);
					ancestorIds.Add(aId);
					if (!items.ContainsKey(aId))
						items.Add(aId, node.Item);
				}

				// Wire ancestor chain (parent -> child)
				// When includeMatches=true, sequence is [self, parent, grandparent, ...]
				// When includeMatches=false, sequence is [parent, grandparent, ...]
				if (ancestorIds.Count > 0)
				{
					if (includeMatches && EqualityComparer<TId>.Default.Equals(ancestorIds[0], id))
					{
						for (int i = 1; i < ancestorIds.Count; i++)
							childMap.Add(ancestorIds[i], ancestorIds[i - 1]);
					}
					else
					{
						for (int i = 1; i < ancestorIds.Count; i++)
							childMap.Add(ancestorIds[i], ancestorIds[i - 1]);
					}

					// Ensure the top-most ancestor exists as a key even if it has no outgoing edges in the result
					childMap.Add(ancestorIds[^1]);
				}
			}

			// Descendants facet
			if (includeDescendants)
			{
				var descendOpts = includeMatches ? Descend.WithSelf : Descend.WithoutSelf;
				foreach (var node in hierarchy.GetDescendants(id, descendOpts))
				{
					var nId = hierarchy.Identify(node.Item);
					if (!items.ContainsKey(nId))
					{
						items.Add(nId, node.Item);
						if (node.IsLeaf)
							childMap.Add(nId); // ensure leaf key exists
					}

					if (node.IsInternal)
					{
						var childIds = node.Children.ToIds(hierarchy.Identify);
						childMap.Add(nId, childIds);
						foreach (var c in node.Children)
						{
							var cId = hierarchy.Identify(c.Item);
							if (!items.ContainsKey(cId))
								items[cId] = c.Item;
						}
					}
				}
			}

			// Matches-only facet (no ancestors/descendants)
			if (includeMatches && !includeAncestors && !includeDescendants)
			{
				var node = hierarchy.GetNode(id);
				var nId = id;
				bool wired = false;

				if (!node.IsRoot)
				{
					var pId = hierarchy.Identify(node.Parent!.Item);
					if (items.ContainsKey(pId))
					{
						childMap.Add(pId, nId);
						wired = true;
					}
				}

				if (node.IsInternal)
				{
					var included = node.Children
						.Select(c => hierarchy.Identify(c.Item))
						.Where(items.ContainsKey)
						.ToArray();
					if (included.Length > 0)
					{
						childMap.Add(nId, included);
						wired = true;
					}
				}

				if (!wired)
					childMap.Add(nId);

				items[nId] = node.Item;
			}
		}

		return Hierarchies.CreateFromChildMap(items.Values, hierarchy.Identify, childMap);
	}

	/// <summary>
	/// Create a new hierarchy with nodes for a subset of matching nodes.
	/// </summary>
	/// <param name="hierarchy">The hierarchy to search within.</param>
	/// <param name="id">The ID to search for.</param>
	/// <param name="include">
	/// Flags controlling which nodes to include in the result:
	/// <list type="bullet">
	/// <item><description><see cref="SearchInclude.Matches"/> - Include the match itself</description></item>
	/// <item><description><see cref="SearchInclude.Ancestors"/> - Include all ancestors of a match</description></item>
	/// <item><description><see cref="SearchInclude.Descendants"/> - Include all descendants of a match</description></item>
	/// <item><description><see cref="SearchInclude.All"/> - Include <see cref="SearchInclude.Matches"/>, <see cref="SearchInclude.Ancestors"/> and <see cref="SearchInclude.Descendants"/></description></item>
	/// </list>
	/// </param>
	/// <returns>A new hierarchy containing only the specified subset of nodes - effectively a pruned clone.</returns>
	public static Hierarchy<TItem, TId> Search<TItem, TId>(
		this Hierarchy<TItem, TId> hierarchy,
		TId id,
		SearchInclude include = SearchInclude.All)
		where TItem : notnull
		where TId : notnull
	{
		return hierarchy.Search([id], include);
	}

	/// <summary>
	/// Create a new hierarchy with nodes for a subset of matching nodes.
	/// </summary>
	/// <param name="hierarchy">The hierarchy to search within.</param>
	/// <param name="predicate">A predicate to filter nodes that should be included as matches.</param>
	/// <param name="include">
	/// Flags controlling which nodes to include in the result:
	/// <list type="bullet">
	/// <item><description><see cref="SearchInclude.Matches"/> - Include the match itself</description></item>
	/// <item><description><see cref="SearchInclude.Ancestors"/> - Include all ancestors of a match</description></item>
	/// <item><description><see cref="SearchInclude.Descendants"/> - Include all descendants of a match</description></item>
	/// <item><description><see cref="SearchInclude.All"/> - Include <see cref="SearchInclude.Matches"/>, <see cref="SearchInclude.Ancestors"/> and <see cref="SearchInclude.Descendants"/></description></item>
	/// </list>
	/// </param>
	/// <returns>A new hierarchy containing only the specified subset of nodes - effectively a pruned clone.</returns>
	public static Hierarchy<TItem, TId> Search<TItem, TId>(
		this Hierarchy<TItem, TId> hierarchy,
		Func<Node<TItem>, bool> predicate,
		SearchInclude include = SearchInclude.All)
		where TItem : notnull
		where TId : notnull
	{
		var matchingIds = hierarchy.Find(predicate).ToIds(hierarchy.Identify);
		return hierarchy.Search(matchingIds, include);
	}
}
