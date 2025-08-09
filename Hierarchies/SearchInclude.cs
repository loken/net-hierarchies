namespace Loken.Hierarchies;

/// <summary>
/// Facets that control which parts of the original hierarchy are included in the pruned search result.
/// Combine flags to include multiple facets.
/// </summary>
[Flags]
public enum SearchInclude
{
	/// <summary>Include the nodes matching the search itself.</summary>
	Matches = 1 << 0,

	/// <summary>Include the ancestors of the matches. When combined with <see cref="Matches"/>, the matching node is included as well.</summary>
	Ancestors = 1 << 1,

	/// <summary>Include the descendants of the matches. When combined with <see cref="Matches"/>, the matching node is included as well.</summary>
	Descendants = 1 << 2,

	/// <summary>Include matches, their ancestors and their descendants.</summary>
	All = Matches | Ancestors | Descendants,
}
