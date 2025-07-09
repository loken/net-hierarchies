namespace Loken.Hierarchies;

/// <summary>
/// Extensions for mapping from <see cref="MultiMap{TId}"/> to other representations.
/// </summary>
public static class MultiMapToExtensions
{
	/// <summary>
	/// Create a sequence of relations matching the <paramref name="childMap"/>.
	/// </summary>
	public static IEnumerable<Relation<TId>> ToRelations<TId>(this MultiMap<TId> childMap)
		where TId : notnull
	{
		foreach (var (parent, children) in childMap)
		{
			if (children.Count == 0)
			{
				yield return new(parent);
			}
			else
			{
				foreach (var child in children)
					yield return new(parent, child);
			}
		}
	}
}
