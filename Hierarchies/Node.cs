using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Loken.System.Collections;

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

/// <summary>
/// Wrapper for a <see cref="Value"/> participating in a double-linked graph.
/// <para>
/// By using a wrapper rather than require specific properties on the type parameter
/// we don't need to make any assumptions about the wrapped <see cref="Type"/>.
/// </para>
/// </summary>
/// <remarks>
/// Nodes are considered equal when they share the same <see cref="Value"/> instance regardless of its links.
/// In order to support serialization we ignore the Parent relationship. Otherwise a serializer will trip up on circular dependencies.
/// </remarks>
[DataContract]
public class Node<T>
	where T : notnull
{
	/// <summary>
	/// The value is the subject/content of the node.
	/// </summary>
	[DataMember, JsonInclude]
	public required T Value { get; init; }

	/// <summary>
	/// Links to the node nodes.
	/// No nodes means it's a "leaf".
	/// Will be set to <c>null</c> when empty to keep memory footprint minimal.
	/// </summary>
	[DataMember, JsonInclude]
	public ISet<Node<T>>? Children { get; private set; }

	/// <summary>
	/// Link to the parent node.
	/// No parent node means it's a "root".
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	public Node<T>? Parent { get; private set; }

	/// <summary>
	/// A node is a "root" when there is no <see cref="Parent"/>.
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	public bool IsRoot => Parent is null;

	/// <summary>
	/// A node is a "leaf" when there are no <see cref="Children"/>.
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	public bool IsLeaf => Children is null or { Count: 0 };

	/// <summary>
	/// A node is "internal" when it has <see cref="Children"/>,
	/// meaning it's either "internal" or a "leaf".
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	public bool IsInternal => !IsLeaf;

	/// <summary>
	/// Attach the provided <paramref name="nodes"/> as <see cref="Children"/>.
	/// </summary>
	/// <param name="nodes">Nodes to attach.</param>
	/// <returns>The node itself for method chaining purposes.</returns>
	/// <exception cref="ArgumentException">
	/// When any of the <paramref name="nodes"/> is already attached to another <see cref="Parent"/>.
	/// </exception>
	public Node<T> Attach(params Node<T>[] nodes)
	{
		foreach (var node in nodes)
		{
			if (node.Parent != null)
				throw new ArgumentException($"The {nameof(Parent)} must be null before attaching it to another {nameof(Parent)}", nameof(nodes));

			Children ??= new HashSet<Node<T>>();

			Children.Add(node);
			node.Parent = this;
		}

		return this;
	}

	/// <summary>
	/// Detatch the node from it's <see cref="Parent"/>.
	/// </summary>
	/// <returns>The node itself for method chaining purposes.</returns>
	/// <exception cref="Exception">When the node is a root.</exception>
	public Node<T> Detach()
	{
		if (Parent is null)
			throw new Exception("Can't detach a root node as there's nothing to detach it from.");

		if (Parent.Children is null || !Parent.Children.Contains(this))
			throw new Exception("Invalid object state: It should not be possible for the node not to be a child of its parent!.");

		Parent.Children.Remove(this);

		if (Parent.Children.Count == 0)
			Parent.Children = null;

		Parent = null;

		return this;
	}

	public void Dismantle(bool detach = true)
	{
		if (!IsRoot && detach)
			Detach();

		foreach (var descendant in GetDescendants(false))
			descendant.Detach();
	}

	/// <summary>
	/// Performs a breadth first traversal yielding each node.
	/// </summary>
	/// <param name="includeSelf">
	/// Should the node itself be yielded (true) or should it start
	/// with its <see cref="Children"/> (false)?
	/// </param>
	/// <returns>An enumeration of nodes.</returns>
	/// <remarks>
	/// No default value is provided for <paramref name="includeSelf"/>
	/// because the caller should always make an active choice.
	/// </remarks>
	public IEnumerable<Node<T>> GetDescendants(bool includeSelf)
	{
		var queue = new Queue<Node<T>>();
		if (includeSelf)
			queue.Enqueue(this);
		else if (Children is not null)
			queue.Enqueue(Children);

		while (queue.Count > 0)
		{
			var node = queue.Dequeue();
			yield return node;
			if (node.Children is not null)
				queue.Enqueue(node.Children);
		}
	}

	/// <summary>
	/// Walks up the <see cref="Parent"/> links yielding each node.
	/// </summary>
	/// <param name="includeSelf">
	/// Should the node itself be yielded (true) or should it start
	/// with its <see cref="Parent"/> (false)?
	/// </param>
	/// <returns>An enumeration of nodes.</returns>
	/// <remarks>
	/// No default value is provided for <paramref name="includeSelf"/>
	/// because the caller should always make an active choice.
	/// </remarks>
	public IEnumerable<Node<T>> GetAncestors(bool includeSelf)
	{
		var curr = includeSelf ? this : Parent;
		while (curr != null)
		{
			yield return curr;
			curr = curr.Parent;
		}
	}

	#region Implicit operators
	/// <summary>
	/// Implicitly convert a value into a node, allowing us to pass values to methods that takes nodes.
	/// </summary>
	public static implicit operator Node<T>(T item)
	{
		return new Node<T>() { Value = item };
	}

	/// <summary>
	/// Implicitly convert a node into a value, allowing us to unwrap the value from its node.
	/// </summary>
	public static implicit operator T(Node<T> node)
	{
		return node.Value;
	}
	#endregion

	#region Object overrides
	public override string? ToString()
	{
		return Value.ToString();
	}

	public override int GetHashCode()
	{
		return Value.GetHashCode();
	}

	public override bool Equals(object? obj)
	{
		return obj is Node<T> other && Value.Equals(other.Value);
	}
	#endregion
}
