using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Loken.Hierarchies;

/// <summary>
/// Keeps track of the a set of <see cref="Nodes"/> and their <see cref="Roots"/>
/// and provides structural modification.
/// <para>Created through the <see cref="Hierarchies"/> factory class.</para>
/// </summary>
/// <typeparam name="TItem">The <see cref="Type"/> of item held by each <see cref="Node{TItem}"/> of the <see cref="Nodes"/>.</typeparam>
/// <typeparam name="TId">The <see cref="Type"/> of identifier for each of the <see cref="Nodes"/>.</typeparam>
[DataContract]
public class Hierarchy<TItem, TId>
	where TId : notnull
	where TItem : notnull
{
	public readonly Func<TItem, TId> Identify;

	protected readonly List<Node<TItem>> _roots = [];
	protected readonly Dictionary<TId, Node<TItem>> _nodes = [];
	protected readonly Dictionary<TId, Action> _debrand = [];

	[IgnoreDataMember, JsonIgnore]
	public IReadOnlyCollection<Node<TItem>> Nodes => _nodes.Values;

	/// <summary>
	/// Get all root nodes.
	/// </summary>
	[DataMember, JsonInclude]
	public IReadOnlyList<Node<TItem>> Roots { get; }

	/// <summary>
	/// Get all root items.
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	public IList<TItem> RootItems => _roots.ToItems();

	/// <summary>
	/// Get all root IDs.
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	public IList<TId> RootIds => _roots.ToIds(Identify);

	/// <summary>
	/// Get all node items.
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	public IList<TItem> NodeItems => _nodes.Values.ToItems();

	/// <summary>
	/// Get all node IDs.
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	public IList<TId> NodeIds => _nodes.Values.ToIds(Identify);

	internal Hierarchy(Func<TItem, TId> identify)
	{
		Identify = identify;

		Roots = new ReadOnlyCollection<Node<TItem>>(_roots);
	}

	/// <summary>
	/// Attempt to get the node with the specified ID.
	/// </summary>
	/// <param name="id">The ID of the node to retrieve.</param>
	/// <param name="node">When this method returns, contains the node with the specified ID if found; otherwise, null.</param>
	/// <returns>True if a node with the specified ID was found; otherwise, false.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryGetNode(TId id, [NotNullWhen(true)] out Node<TItem>? node)
	{
		return _nodes.TryGetValue(id, out node);
	}

	/// <summary>
	/// Get the node with the specified ID.
	/// </summary>
	/// <param name="id">The ID of the node to retrieve.</param>
	/// <returns>The node with the specified ID.</returns>
	/// <exception cref="KeyNotFoundException">Thrown when no node with the specified ID exists in the hierarchy.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Node<TItem> GetNode(TId id)
	{
		return _nodes[id];
	}

	/// <summary>
	/// Get the nodes with the specified IDs.
	/// </summary>
	/// <param name="ids">The IDs of the nodes to retrieve.</param>
	/// <returns>A list containing the nodes with the specified IDs.</returns>
	/// <exception cref="KeyNotFoundException">Thrown when any of the specified IDs does not exist in the hierarchy.</exception>
	public IList<Node<TItem>> GetNodes(IEnumerable<TId> ids)
	{
		return ids.Select(id => _nodes[id]).ToList();
	}

	/// <summary>
	/// Get the item of the node with the specified ID.
	/// </summary>
	/// <param name="id">The ID of the node whose item to retrieve.</param>
	/// <returns>The item of the node with the specified ID.</returns>
	/// <exception cref="KeyNotFoundException">Thrown when no node with the specified ID exists in the hierarchy.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TItem GetItem(TId id)
	{
		return GetNode(id).Item;
	}

	/// <summary>
	/// Get the items of the nodes with the specified IDs.
	/// </summary>
	/// <param name="ids">The IDs of the nodes whose items to retrieve.</param>
	/// <returns>A list containing the items of the nodes with the specified IDs.</returns>
	/// <exception cref="KeyNotFoundException">Thrown when any of the specified IDs does not exist in the hierarchy.</exception>
	public IList<TItem> GetItems(IEnumerable<TId> ids)
	{
		return ids.Select(id => GetNode(id).Item).ToList();
	}

	/// <summary>
	/// Check if the provided ID exists in the hierarchy.
	/// </summary>
	/// <param name="id">The ID to check for existence.</param>
	/// <returns>True if the ID exists in the hierarchy, false otherwise.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Has(TId id)
	{
		return _nodes.ContainsKey(id);
	}

	/// <summary>
	/// Check if all of the provided IDs exist in the hierarchy.
	/// </summary>
	/// <param name="ids">The IDs to check for existence.</param>
	/// <returns>True if all IDs exist in the hierarchy, false otherwise.</returns>
	public bool HasAll(IEnumerable<TId> ids)
	{
		return ids.All(_nodes.ContainsKey);
	}

	/// <summary>
	/// Check if any of the provided IDs exist in the hierarchy.
	/// </summary>
	/// <param name="ids">The IDs to check for existence.</param>
	/// <returns>True if any ID exists in the hierarchy, false otherwise.</returns>
	public bool HasAny(IEnumerable<TId> ids)
	{
		return ids.Any(_nodes.ContainsKey);
	}

	/// <summary>
	/// Attach the provided <paramref name="roots"/>.
	/// </summary>
	/// <param name="roots">Nodes to attach.</param>
	/// <returns>The hierarchy itself for method chaining purposes.</returns>
	/// <exception cref="InvalidOperationException">
	/// Must provide non-empty set of <paramref name="roots"/> of a compatible brand
	/// that aren't already attached to another parent.
	/// </exception>
	public Hierarchy<TItem, TId> AttachRoot(IEnumerable<Node<TItem>> roots)
	{
		if (roots is not ICollection<Node<TItem>> collection)
			collection = [.. roots];

		if (!collection.All(root => root.IsRoot))
			throw new InvalidOperationException("Must all be roots!");

		AddNodes(collection);

		_roots.AddRange(collection);

		return this;
	}

	/// <summary>
	/// Attach the provided <paramref name="children"/> to the node of the
	/// provided <paramref name="parentId"/>.
	/// </summary>
	/// <param name="parentId">The id of the node to attach to.</param>
	/// <param name="children">Nodes to attach.</param>
	/// <returns>The hierarchy itself for method chaining purposes.</returns>
	/// <exception cref="InvalidOperationException">
	/// Must provide non-empty set of <paramref name="children"/> of a compatible brand
	/// that aren't already attached to another parent.
	/// </exception>
	public Hierarchy<TItem, TId> Attach(TId parentId, IEnumerable<Node<TItem>> children)
	{
		if (children is not ICollection<Node<TItem>> collection)
			collection = [.. children];

		if (!_nodes.ContainsKey(parentId))
			throw new ArgumentException("Matching parent not yet added", nameof(parentId));

		AddNodes(collection);

		var parentNode = _nodes[parentId];
		parentNode.Attach(collection);

		return this;
	}

	/// <summary>
	/// Detach the provided <paramref name="nodes"/>.
	/// </summary>
	/// <param name="nodes">Nodes to detach.</param>
	/// <returns>The hierarchy itself for method chaining purposes.</returns>
	/// <exception cref="InvalidOperationException">
	/// Must provide non-empty set of attached <paramref name="nodes"/>.
	/// </exception>
	public Hierarchy<TItem, TId> Detach(IEnumerable<Node<TItem>> nodes)
	{
		if (nodes is not ICollection<Node<TItem>> collection)
			collection = [.. nodes];

		foreach (var node in collection.GetDescendants(Descend.WithSelf))
		{
			var id = Identify(node.Item);
			_debrand[id]();
			_debrand.Remove(id);
			_nodes.Remove(id);
			_roots.Remove(node);
		}

		foreach (var node in collection)
		{
			if (!node.IsRoot)
				node.DetachSelf();
		}

		return this;
	}

	private void AddNodes(ICollection<Node<TItem>> nodes)
	{
		foreach (var node in nodes.GetDescendants(Descend.WithSelf))
		{
			var id = Identify(node.Item);
			_debrand.Add(id, node.Brand(this));
			_nodes.Add(id, node);
		}
	}
}