using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Loken.Hierarchies;

/// <summary>
/// Keeps track of the a set of <see cref="Nodes"/> and their <see cref="Roots"/>
/// and provides structural modification.
/// <para>Created through <see cref="Hierarchy"/>.</para>
/// </summary>
/// <typeparam name="TItem">The <see cref="Type"/> of item held by each <see cref="Node{TItem}"/> of the <see cref="Nodes"/>.</typeparam>
/// <typeparam name="TId">The <see cref="Type"/> of identifier for each of the <see cref="Nodes"/>.</typeparam>
[DataContract]
public class Hierarchy<TItem, TId>
	where TId : notnull
	where TItem : notnull
{
	public readonly Func<TItem, TId> Identify;

	protected readonly List<Node<TItem>> _roots = new();
	protected readonly Dictionary<TId, Node<TItem>> _nodes = new();
	protected readonly Dictionary<TId, Action> _debrand = new();

	[IgnoreDataMember, JsonIgnore]
	public IReadOnlyCollection<Node<TItem>> Nodes => _nodes.Values;

	[DataMember, JsonInclude]
	public IReadOnlyList<Node<TItem>> Roots { get; }

	internal Hierarchy(Func<TItem, TId> identify)
	{
		Identify = identify;

		Roots = new ReadOnlyCollection<Node<TItem>>(_roots);
	}

	public Node<TItem> GetNode(TId id)
	{
		return _nodes[id];
	}

	public TItem Get(TId id)
	{
		return GetNode(id).Item;
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
	public Hierarchy<TItem, TId> Attach(params Node<TItem>[] roots)
	{
		if (!roots.All(root => root.IsRoot))
			throw new InvalidOperationException("Must all be roots!");

		AddNodes(roots);

		_roots.AddRange(roots);

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
	public Hierarchy<TItem, TId> Attach(TId parentId, params Node<TItem>[] children)
	{
		if (!_nodes.ContainsKey(parentId))
			throw new ArgumentException("Matching parent not yet added", nameof(parentId));

		AddNodes(children);

		var parentNode = _nodes[parentId];
		parentNode.Attach(children);

		return this;
	}

	/// <summary>
	/// Detach the provided <paramref name="nodes"/>.
	/// </summary>
	/// <param name="nodes">Nodes to detach.</param>
	/// <returns>The node itself for method chaining purposes.</returns>
	/// <exception cref="InvalidOperationException">
	/// Must provide non-empty set of attached <paramref name="nodes"/>.
	/// </exception>
	public Hierarchy<TItem, TId> Detach(params Node<TItem>[] nodes)
	{
		foreach (var node in Traverse.Graph(nodes, n => n.Children))
		{
			var id = Identify(node.Item);
			_debrand[id]();
			_debrand.Remove(id);
			_nodes.Remove(id);
			_roots.Remove(node);
		}

		foreach (var node in nodes)
			node.DetachSelf();

		return this;
	}

	private void AddNodes(IEnumerable<Node<TItem>> nodes)
	{
		foreach (var node in Traverse.Graph(nodes, n => n.Children))
		{
			var id = Identify(node.Item);
			_debrand.Add(id, node.Brand(this));
			_nodes.Add(id, node);
		}
	}
}