using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Loken.System.Collections;

namespace Loken.Hierarchies;

public static class Node
{
	public static Node<T> Create<T>(T value)
		where T : notnull
	{
		return new() { Value = value };
	}

	public static IEnumerable<Node<T>> CreateMany<T>(IEnumerable<T> values)
		where T : notnull
	{
		return values.Select(val => new Node<T>() { Value = val });
	}

	public static Node<T>[] CreateMany<T>(params T[] values)
		where T : notnull
	{
		return CreateMany(values.AsEnumerable()).ToArray();
	}
}

[DataContract]
public class Node<T>
	where T : notnull
{
	[DataMember, JsonInclude]
	public required T Value { get; init; }

	[DataMember, JsonInclude]
	public ISet<Node<T>>? Children { get; private set; }

	[IgnoreDataMember, JsonIgnore]
	public Node<T>? Parent { get; private set; }

	[IgnoreDataMember, JsonIgnore]
	public bool IsRoot => Parent is null;

	[IgnoreDataMember, JsonIgnore]
	public bool IsLeaf => Children is null or { Count: 0 };

	[IgnoreDataMember, JsonIgnore]
	public bool IsInternal => !IsLeaf;

	public Node<T> Attach(params Node<T>[] children)
	{
		foreach (var child in children)
		{
			if (child.Parent != null)
				throw new ArgumentException($"The {nameof(Parent)} must be null before attaching it to another {nameof(Parent)}", nameof(children));

			Children ??= new HashSet<Node<T>>();

			Children.Add(child);
			child.Parent = this;
		}

		return this;
	}

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

	public IEnumerable<Node<T>> GetAncestors(bool includeSelf)
	{
		var curr = includeSelf ? this : Parent;
		while (curr != null)
		{
			yield return curr;
			curr = curr.Parent;
		}
	}

	public static implicit operator Node<T>(T item)
	{
		return new Node<T>() { Value = item };
	}

	public static implicit operator T(Node<T> node)
	{
		return node.Value;
	}

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
}
