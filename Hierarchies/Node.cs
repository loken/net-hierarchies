namespace Loken.Hierarchies;

/// <summary>
/// Factory class for <see cref="Node{T}"/> which allow us to infer
/// the type parameter from the arguments.
/// </summary>
public static class Node
{
	/// <summary>
	/// Create a <see cref="Node{T}"/> from the <paramref name="value"/>.
	/// </summary>
	public static Node<T> Create<T>(T value)
		where T : notnull
	{
		return new() { Value = value };
	}

	/// <summary>
	/// Create a <see cref="Node{T}"/> for each of the <paramref name="values"/>.
	/// </summary>
	public static IEnumerable<Node<T>> CreateMany<T>(IEnumerable<T> values)
		where T : notnull
	{
		return values.Select(val => new Node<T>() { Value = val });
	}

	/// <summary>
	/// Create a <see cref="Node{T}"/> for each of the <paramref name="values"/>.
	/// </summary>
	public static Node<T>[] CreateMany<T>(params T[] values)
		where T : notnull
	{
		return CreateMany(values.AsEnumerable()).ToArray();
	}
}
