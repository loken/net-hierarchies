using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Loken.Hierarchies;

/// <summary>
/// Wrapper for a <see cref="Item"/> participating in a double-linked graph.
/// <para>
/// By using a wrapper rather than require specific properties on the type parameter
/// we don't need to make any assumptions about the wrapped <see cref="Type"/>.
/// </para>
/// </summary>
/// <remarks>
/// Nodes are considered equal when they share the same <see cref="Item"/> instance regardless of its links.
/// In order to support serialization we ignore the Parent relationship. Otherwise a serializer will trip up on circular dependencies.
/// </remarks>
[DataContract]
public class Node<TItem>
	where TItem : notnull
{
	/// <summary>
	/// Backing field for <see cref="Children"/>.
	/// Will be set to <c>null</c> when empty to keep memory footprint minimal.
	/// </summary>
	[DataMember(Name = nameof(Children))]
	[JsonInclude, JsonPropertyName(nameof(Children))]
	private ISet<Node<TItem>>? _children;

	/// <summary>
	/// The item is the subject/content of the node.
	/// </summary>
	[DataMember, JsonInclude]
	public required TItem Item { get; init; }

	/// <summary>
	/// Links to the node nodes.
	/// No nodes means it's a "leaf".
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	public IEnumerable<Node<TItem>> Children => _children?.AsEnumerable() ?? Enumerable.Empty<Node<TItem>>();

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
	[MemberNotNullWhen(false, nameof(_children))]
	public bool IsLeaf => _children is null or { Count: 0 };

	/// <summary>
	/// A node is "internal" when it has <see cref="Children"/>,
	/// meaning it's either "internal" or a "leaf".
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	[MemberNotNullWhen(true, nameof(_children))]
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
	[MemberNotNull(nameof(_children))]
	public Node<TItem> Attach(params Node<TItem>[] nodes)
	{
		if (nodes.Length == 0)
			throw new ArgumentOutOfRangeException(nameof(nodes), $"Must provide one or more");

		_children ??= new HashSet<Node<TItem>>();

		foreach (var node in nodes)
		{
			if (node.Parent != null)
				throw new ArgumentException($"The {nameof(Parent)} must be null before attaching it to another {nameof(Parent)}", nameof(nodes));

			_children.Add(node);
			node.Parent = this;
		}

		return this;
	}

	/// <summary>
	/// Detach the provided <paramref name="nodes"/> so they are no longer <see cref="Children"/>.
	/// </summary>
	/// <returns>The node itself for method chaining purposes.</returns>
	/// <exception cref="Exception">When the node is a root.</exception>
	public Node<TItem> Detach(params Node<TItem>[] nodes)
	{
		if (nodes.Length == 0)
			throw new ArgumentOutOfRangeException(nameof(nodes), $"Must provide one or more nodes.");

		if (IsLeaf || !nodes.All(_children.Contains))
			throw new ArgumentOutOfRangeException(nameof(nodes), $"Must all be children.");

		foreach (var node in nodes)
		{
			_children.Remove(node);
			node.Parent = null;
		}

		if (IsLeaf)
			_children = null;

		return this;
	}

	/// <summary>
	/// Detach the node from it's <see cref="Parent"/>.
	/// </summary>
	/// <returns>The node itself for method chaining purposes.</returns>
	/// <exception cref="Exception">When the node is a root.</exception>
	public Node<TItem> DetachSelf()
	{
		if (IsRoot)
			throw new Exception("Can't detach a root node as there's nothing to detach it from.");

		if (Parent.IsLeaf || !Parent.Children.Contains(this))
			throw new Exception("Invalid object state: It should not be possible for the node not to be a child of its parent!.");

		Parent._children.Remove(this);

		if (Parent.IsLeaf)
			Parent._children = null;

		Parent = null;

		return this;
	}

	#region Object overrides
	public override string? ToString()
	{
		return Item.ToString();
	}

	public override int GetHashCode()
	{
		return Item.GetHashCode();
	}

	public override bool Equals(object? obj)
	{
		return obj is Node<TItem> other && Item.Equals(other.Item);
	}
	#endregion
}
