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
	/// The brand is used to lock the node to a specific owner.
	/// </summary>
	/// <remarks>
	/// We're not serializing the brand, since it could be an object reference.
	/// This means that serialization breaks branding, so one must rebuild the brands
	/// after serializing.
	/// </remarks>
	private object? _brand;

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
	/// Having a brand means that some other entity "owns" the node.
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	[MemberNotNullWhen(true, nameof(_brand))]
	public bool IsBranded => _brand is not null;

	/// <summary>
	/// When the brands of two nodes are compatible, they may be linked/attached
	/// in a parent-child relationship.
	/// </summary>
	public bool IsBrandCompatible(Node<TItem> other)
	{
		if (_brand is null)
			return other._brand is null;
		else if (other._brand is null)
			return false;
		else
			return _brand.Equals(other._brand);
	}

	/// <summary>
	/// Adds the provided <paramref name="brand"/> to the node,
	/// providing an action delegate for removing/clearing the brand.
	/// <para>It is necessary to remove an old brand before you can apply another.</para>
	/// </summary>
	/// <param name="brand">The brand should uniquely identify the owner or owning concept.</param>
	/// <returns>An action that can be called in order to debrand the node.</returns>
	/// <exception cref="ArgumentNullException">The brand cannot be null.</exception>
	/// <exception cref="InvalidOperationException">Must clear brand using delegate before you can rebrand a node.</exception>
	public Action Brand(object brand)
	{
		if (brand is null)
			throw new ArgumentNullException(nameof(brand), "The brand cannot be null.");

		if (_brand is not null)
			throw new InvalidOperationException("Must clear brand using delegate before you can rebrand a node.");

		_brand = brand;
		return () => _brand = default;
	}

	/// <summary>
	/// Attach the provided <paramref name="children"/>.
	/// </summary>
	/// <param name="children">Nodes to attach.</param>
	/// <returns>The node itself for method chaining purposes.</returns>
	/// <exception cref="InvalidOperationException">
	/// Must provide non-empty set of <paramref name="children"/> of a compatible brand
	/// that aren't already attached to another parent.
	/// </exception>
	[MemberNotNull(nameof(_children))]
	public Node<TItem> Attach(params Node<TItem>[] children)
	{
		if (children.Length == 0)
			throw new InvalidOperationException($"Must provide one or more {nameof(children)}");

		if (children.Any(node => node.Parent is not null))
			throw new InvalidOperationException($"Must all be without a {nameof(Parent)} before attaching to another {nameof(Parent)}");

		if (!children.All(node => node.IsBrandCompatible(this)))
			throw new InvalidOperationException("Must all have a compatible brand");

		_children ??= new HashSet<Node<TItem>>();

		foreach (var node in children)
		{
			_children.Add(node);
			node.Parent = this;
		}

		return this;
	}

	/// <summary>
	/// Detach the provided <paramref name="children"/>.
	/// </summary>
	/// <param name="children">Nodes to detach.</param>
	/// <returns>The node itself for method chaining purposes.</returns>
	/// <exception cref="InvalidOperationException">
	/// Must provide non-empty set of attached and non-branded <paramref name="children"/>.
	/// </exception>
	public Node<TItem> Detach(params Node<TItem>[] children)
	{
		if (children.Length == 0)
			throw new InvalidOperationException($"Must provide one or more {nameof(children)}.");

		if (IsLeaf || !children.All(_children.Contains))
			throw new InvalidOperationException($"Must all be {nameof(children)}.");

		if (children.Any(node => node.IsBranded))
			throw new InvalidOperationException("Must clear brand using delegate before you can detach a branded node.");

		foreach (var node in children)
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
	/// <exception cref="InvalidOperationException">Must be attached and non-branded.</exception>
	public Node<TItem> DetachSelf()
	{
		if (IsRoot)
			throw new InvalidOperationException("Can't detach a root node as there's nothing to detach it from.");

		if (IsBranded)
			throw new InvalidOperationException("Must clear brand using delegate before you can detach a branded node.");

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
