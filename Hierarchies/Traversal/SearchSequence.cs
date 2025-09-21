namespace Loken.Hierarchies.Traversal;

/// <summary>
/// Provides sequence search helpers that return either the first match or all matches.
/// </summary>
public static partial class Search
{
	/// <summary>
	/// Search a sequence by traversing from the <paramref name="first"/> element using <paramref name="next"/>.
	/// <para>Stops when the first element matching <paramref name="predicate"/> is found.</para>
	/// </summary>
	public static TEl? Sequence<TEl>(TEl? first, Func<TEl, TEl?> next, Func<TEl, bool> predicate)
	{
		var current = first;
		while (current is not null)
		{
			if (predicate(current))
				return current;

			current = next(current);
		}

		return default;
	}

	/// <summary>
	/// Search a sequence by traversing from the <paramref name="first"/> element using <paramref name="next"/>.
	/// <para>Returns all elements matching <paramref name="predicate"/>.</para>
	/// </summary>
	public static IList<TEl> SequenceMany<TEl>(TEl? first, Func<TEl, TEl?> next, Func<TEl, bool> predicate)
	{
		var result = new List<TEl>();
		var current = first;
		while (current is not null)
		{
			if (predicate(current))
				result.Add(current);

			current = next(current);
		}

		return result;
	}
}
