using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Loken.Hierarchies;

/// <summary>
/// Predicate for matching nodes.
/// </summary>
/// <typeparam name="TItem">The type of item in the node.</typeparam>
/// <param name="node">The node to test.</param>
/// <returns>True if the node matches, false otherwise.</returns>
public delegate bool NodePredicate<TItem>(Node<TItem> node)
	where TItem : notnull;

/// <summary>
/// Wrapper for a <see cref="TItem"/> participating in a double-linked graph.
/// <para>
/// By using a wrapper rather than require specific properties on the type parameter
/// we don't need to make any assumptions about the wrapped <see cref="Type"/>.
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// <strong>Equality:</strong> Nodes are considered equal when they share the same <see cref="TItem"/>
/// instance regardless of their links.
/// </para>
/// <para>
/// <strong>Serialization:</strong> The Parent relationship is ignored during serialization to prevent
/// circular dependencies. Only children are serialized to maintain tree structure.
/// </para>
/// <para>
/// <strong>Performance Design:</strong> This class uses three internal collections for optimal performance:
/// </para>
/// <list type="bullet">
/// <item><description><c>_children</c> - Maintains insertion order and supports serialization</description></item>
/// <item><description><c>_childSet</c> - Provides O(1) membership testing and duplicate prevention</description></item>
/// <item><description><c>_childCache</c> - Lazy cache for secure read-only access</description></item>
/// </list>
/// <para>
/// This space-time tradeoff ensures O(1) operations for attach/detach while providing secure,
/// ordered access to children through <see cref="IReadOnlyList{T}"/>.
/// </para>
/// </remarks>
[DataContract]
public class Node<TItem>
	where TItem : notnull
{
	/// <summary>
	/// An empty collection of child nodes to use as a result when there are no children.
	/// </summary>
	private static ReadOnlyCollection<Node<TItem>> EmptyChildren => new(Array.Empty<Node<TItem>>());

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
	/// Maintains child insertion order and supports serialization.
	/// Will be set to <c>null</c> when empty to keep memory footprint minimal.
	/// </summary>
	[DataMember(Name = nameof(Children))]
	[JsonInclude, JsonPropertyName(nameof(Children))]
	private List<Node<TItem>>? _children;

	/// <summary>
	/// Membership set for O(1) contains checks to prevent duplicates and enable fast validation.
	/// Uses <see cref="ReferenceEqualityComparer{T}"/> to ensure nodes are compared by reference.
	/// Not serialized to avoid circular dependencies.
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	private HashSet<Node<TItem>>? _childSet;

	/// <summary>
	/// Lazy cache of the <see cref="Children"/> read-only view.
	/// Protects against tampering by returning <see cref="ReadOnlyCollection{T}"/> instead of mutable <see cref="List{T}"/>.
	/// Cache is invalidated whenever children are modified (attach/detach operations).
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	private ReadOnlyCollection<Node<TItem>>? _childCache;

	/// <summary>
	/// The item is the subject/content of the node.
	/// </summary>
	[DataMember, JsonInclude]
	public required TItem Item { get; init; }

	/// <summary>
	/// List of child nodes in insertion order.
	/// Returns an empty collection for leaf nodes.
	/// <para>
	/// <strong>Security:</strong> Returns <see cref="IReadOnlyList{T}"/> to prevent external modification.
	/// The collection cannot be cast to mutable types like <see cref="List{T}"/>.
	/// </para>
	/// <para>
	/// <strong>Performance:</strong> O(1) indexed access via <c>node.Children[index]</c>.
	/// Result is cached until children are modified (attach/detach operations).
	/// </para>
	/// </summary>
	/// <value>
	/// A read-only list of child nodes, or an empty collection if this is a leaf node.
	/// </value>
	[IgnoreDataMember, JsonIgnore]
	public ReadOnlyCollection<Node<TItem>> Children => _childCache ??= _children is null
		? EmptyChildren
		: _children.AsReadOnly();

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
	[MemberNotNullWhen(false, nameof(_childSet))]
	public bool IsLeaf => _children is null or { Count: 0 };

	/// <summary>
	/// A node is "internal" when it has <see cref="Children"/>,
	/// meaning it's either "internal" or a "leaf".
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	[MemberNotNullWhen(true, nameof(_children))]
	[MemberNotNullWhen(true, nameof(_childSet))]
	public bool IsInternal => !IsLeaf;

	/// <summary>
	/// A node is "linked" when it has a parent or at least one child,
	/// which means it's not both a root and a leaf.
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
	public Node<TItem> Attach(IEnumerable<Node<TItem>> children)
	{
		if (children is not ICollection<Node<TItem>> collection)
			collection = [.. children];

		if (collection.Count == 0)
			throw new InvalidOperationException($"Must provide one or more {nameof(children)}");

		if (collection.Any(node => node.Parent is not null))
			throw new InvalidOperationException($"Must all be without a {nameof(Parent)} before attaching to another {nameof(Parent)}");

		if (!collection.All(node => node.IsBrandCompatible(this)))
			throw new InvalidOperationException("Must all have a compatible brand");

		_children ??= new List<Node<TItem>>();
		_childSet ??= new HashSet<Node<TItem>>(ReferenceEqualityComparer<Node<TItem>>.Instance);

		foreach (var node in collection)
		{
			if (_childSet.Add(node))
			{
				_children.Add(node);
				node.Parent = this;
			}
		}

		// Invalidate cached read-only view
		_childCache = null;

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
	public Node<TItem> Detach(IEnumerable<Node<TItem>> children)
	{
		if (children is not ICollection<Node<TItem>> collection)
			collection = [.. children];

		if (collection.Count == 0)
			throw new InvalidOperationException($"Must provide one or more {nameof(children)}.");

		if (IsLeaf || !collection.All(n => _childSet!.Contains(n)))
			throw new InvalidOperationException($"Must all be {nameof(children)}.");

		if (collection.Any(node => node.IsBranded))
			throw new InvalidOperationException("Must clear brand using delegate before you can detach a branded node.");

		foreach (var node in collection)
		{
			_childSet!.Remove(node);
			_children!.Remove(node);
			node.Parent = null;
		}

		// Invalidate cached read-only view
		_childCache = null;

		if (IsLeaf)
		{
			_children = null;
			_childSet = null;
		}

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

		if (Parent.IsLeaf || !Parent._childSet.Contains(this))
			throw new Exception("Invalid object state: It should not be possible for the node not to be a child of its parent!.");

		Parent._childSet.Remove(this);
		Parent._children.Remove(this);

		// Invalidate parent's cached read-only view
		Parent._childCache = null;

		if (Parent.IsLeaf)
		{
			Parent._children = null;
			Parent._childSet = null;
		}

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
