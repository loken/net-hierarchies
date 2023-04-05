using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Loken.System.Collections;

namespace Loken.Hierarchies;

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
public class Node<TItem>
	where TItem : notnull
{
	/// <summary>
	/// The value is the subject/content of the node.
	/// </summary>
	[DataMember, JsonInclude]
	public required TItem Value { get; init; }

	/// <summary>
	/// Links to the node nodes.
	/// No nodes means it's a "leaf".
	/// Will be set to <c>null</c> when empty to keep memory footprint minimal.
	/// </summary>
	[DataMember, JsonInclude]
	public ISet<Node<TItem>>? Children { get; private set; }

	/// <summary>
	/// Link to the parent node.
	/// No parent node means it's a "root".
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	public Node<TItem>? Parent { get; private set; }

	/// <summary>
	/// A node is a "root" when there is no <see cref="Parent"/>.
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	[MemberNotNullWhen(false, nameof(Parent))]
	public bool IsRoot => Parent is null;

	/// <summary>
	/// A node is a "leaf" when there are no <see cref="Children"/>.
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	[MemberNotNullWhen(false, nameof(Children))]
	public bool IsLeaf => Children is null or { Count: 0 };

	/// <summary>
	/// A node is "internal" when it has <see cref="Children"/>,
	/// meaning it's either "internal" or a "leaf".
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	[MemberNotNullWhen(true, nameof(Children))]
	public bool IsInternal => !IsLeaf;

	/// <summary>
	/// A node is "linked" when it is neither a root nor a child.
	/// A node is "linked" when it has a parent or at least one child.
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	public bool IsLinked => !IsRoot || !IsLeaf;

	/// <summary>
	/// Attach the provided <paramref name="nodes"/> as <see cref="Children"/>.
	/// </summary>
	/// <param name="nodes">Nodes to attach.</param>
	/// <returns>The node itself for method chaining purposes.</returns>
	/// <exception cref="ArgumentException">
	/// When any of the <paramref name="nodes"/> is already attached to another <see cref="Parent"/>.
	/// </exception>
	[MemberNotNull(nameof(Children))]
	public Node<TItem> Attach(params Node<TItem>[] nodes)
	{
		if (nodes.Length == 0)
			throw new ArgumentOutOfRangeException(nameof(nodes), $"Must provide one or more");

		Children ??= new HashSet<Node<TItem>>();

		foreach (var node in nodes)
		{
			if (node.Parent != null)
				throw new ArgumentException($"The {nameof(Parent)} must be null before attaching it to another {nameof(Parent)}", nameof(nodes));

			Children.Add(node);
			node.Parent = this;
		}

		return this;
	}

	/// <summary>
	/// Detach the node from it's <see cref="Parent"/>.
	/// </summary>
	/// <returns>The node itself for method chaining purposes.</returns>
	/// <exception cref="Exception">When the node is a root.</exception>
	public Node<TItem> Detach()
	{
		if (IsRoot)
			throw new Exception("Can't detach a root node as there's nothing to detach it from.");

		if (Parent.IsLeaf || !Parent.Children.Contains(this))
			throw new Exception("Invalid object state: It should not be possible for the node not to be a child of its parent!.");

		Parent.Children.Remove(this);

		if (Parent.Children.Count == 0)
			Parent.Children = null;

		Parent = null;

		return this;
	}

	/// <summary>
	/// Dismantling a node means to cascade detach it.
	/// We always caschade detach the children.
	/// We may also cascade up the ancestry, in which case the node is detached,
	/// and then the parent is dismantled, leading to the whole linked structure
	/// ending up unlinked.
	/// </summary>
	/// <param name="includeAncestry">
	/// Should we cascade through the ancestry (true) or only cascade through the children (false)?
	/// <para>No default value is because the caller should always make an active choice.</para>
	/// </param>
	/// <returns>The node itself for method chaining purposes.</returns>
	public Node<TItem> Dismantle(bool includeAncestry)
	{
		if (!IsRoot && includeAncestry)
		{
			var parent = Parent!;
			Detach();
			parent.Dismantle(true);
		}

		foreach (var descendant in GetDescendants(false))
			descendant.Detach();

		return this;
	}

	/// <summary>
	/// Performs a breadth first traversal yielding each node.
	/// </summary>
	/// <param name="includeSelf">
	/// Should the node itself be yielded (true) or should it start
	/// with its <see cref="Children"/> (false)?
	/// <para>No default value is because the caller should always make an active choice.</para>
	/// </param>
	/// <returns>An enumeration of nodes.</returns>
	public IEnumerable<Node<TItem>> GetDescendants(bool includeSelf)
	{
		var queue = new Queue<Node<TItem>>();
		if (includeSelf)
			queue.Enqueue(this);
		else if (!IsLeaf)
			queue.Enqueue(Children);

		while (queue.Count > 0)
		{
			var node = queue.Dequeue();
			yield return node;
			if (!node.IsLeaf)
				queue.Enqueue(node.Children);
		}
	}

	/// <summary>
	/// Walks up the <see cref="Parent"/> links yielding each node.
	/// </summary>
	/// <param name="includeSelf">
	/// Should the node itself be yielded (true) or should it start
	/// with its <see cref="Parent"/> (false)?
	/// <para>No default value is because the caller should always make an active choice.</para>
	/// </param>
	/// <returns>An enumeration of nodes.</returns>
	public IEnumerable<Node<TItem>> GetAncestors(bool includeSelf)
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
	public static implicit operator Node<TItem>(TItem item)
	{
		return new Node<TItem>() { Value = item };
	}

	/// <summary>
	/// Implicitly convert a node into a value, allowing us to unwrap the value from its node.
	/// </summary>
	public static implicit operator TItem(Node<TItem> node)
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
		return obj is Node<TItem> other && Value.Equals(other.Value);
	}
	#endregion
}
