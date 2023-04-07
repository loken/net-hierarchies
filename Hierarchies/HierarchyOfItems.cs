using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Loken.Hierarchies.Traversal;
using Loken.System.Collections;

namespace Loken.Hierarchies;

/// <summary>
/// Keeps track of the a set of <see cref="Nodes"/> and their <see cref="Roots"/>
/// and provides structural modification as well as mapping to- and from relations.
/// Created through <see cref="Hierarchy"/>.
/// </summary>
/// <typeparam name="TItem">The <see cref="Type"/> of item held by each <see cref="Node{TItem}"/> of the <see cref="Nodes"/>.</typeparam>
/// <typeparam name="TId">The <see cref="Type"/> of identifier for each of the <see cref="Nodes"/>.</typeparam>
[DataContract]
public class Hierarchy<TItem, TId>
	where TId : notnull
	where TItem : notnull
{
	public readonly Func<TItem, TId> Identify;

	private readonly Dictionary<TId, Node<TItem>> _nodes = new();
	private readonly List<Node<TItem>> _roots = new();

	[IgnoreDataMember, JsonIgnore]
	public IReadOnlyCollection<Node<TItem>> Nodes => _nodes.Values;

	[DataMember, JsonInclude]
	public IReadOnlyList<Node<TItem>> Roots { get; }

	internal Hierarchy(Func<TItem, TId> identify)
	{
		Identify = identify;

		Roots = new ReadOnlyCollection<Node<TItem>>(_roots);
	}

	internal Hierarchy(Func<TItem, TId> identify, params TItem[] items) : this(identify, (IEnumerable<TItem>)items)
	{
	}

	internal Hierarchy(Func<TItem, TId> identify, IEnumerable<TItem> items) : this(identify)
	{
		foreach (var item in items)
			_nodes.Add(Identify(item), item);
	}

	public Node<TItem> GetNode(TId id)
	{
		return _nodes[id];
	}

	public TItem Get(TId id)
	{
		return GetNode(id);
	}

	public IEnumerable<(TId parent, TId child)> ToRelations()
	{
		foreach (var node in _nodes.Values.Where(n => !n.IsRoot))
			yield return (Identify(node.Parent!), Identify(node));
	}

	public Hierarchy<TItem, TId> UsingRelations(params (TId parent, TId child)[] relations)
	{
		foreach (var relation in relations)
		{
			var child = _nodes[relation.child];
			var parent = _nodes[relation.parent];
			parent.Attach(child);
		}

		_roots.AddRange(_nodes.Values.Where(n => n.IsRoot));

		return this;
	}

	public Hierarchy<TItem, TId> UsingOther<TOther>(Hierarchy<TOther, TId> other)
		where TOther : notnull
	{
		return UsingRelations(other.ToRelations().ToArray());
	}

	public Hierarchy<TItem, TId> UsingChildMap(IDictionary<TId, ISet<TId>> map)
	{
		foreach (var pair in map)
		{
			var parent = _nodes[pair.Key];
			foreach (var childId in pair.Value)
			{
				parent.Attach(_nodes[childId]);
			}
		}

		_roots.AddRange(_nodes.Values.Where(n => n.IsRoot));

		return this;
	}

	public IDictionary<TId, ISet<TId>> ToChildMap()
	{
		var map = new Dictionary<TId, ISet<TId>>();

		Traverse.Graph(Roots, (node, signal) =>
		{
			signal.Next(node.Children);

			if (!node.IsLeaf)
			{
				var nodeId = Identify(node);
				map.LazySet(nodeId).AddRange(node.Children.Select(child => Identify(child)));
			}
		});

		return map;
	}

	public IDictionary<TId, ISet<TId>> ToDescendantMap()
	{
		var map = new Dictionary<TId, ISet<TId>>();

		Traverse.Graph(Roots, (node, signal) =>
		{
			signal.Next(node.Children);

			var nodeId = Identify(node);
			foreach (var ancestor in node.GetAncestors(false))
				map.LazySet(Identify(ancestor)).Add(nodeId);
		});

		return map;
	}

	public IDictionary<TId, ISet<TId>> ToAncestorMap()
	{
		var map = new Dictionary<TId, ISet<TId>>();

		Traverse.Graph(Roots, (node, signal) =>
		{
			signal.Next(node.Children);

			var nodeId = Identify(node);
			foreach (var ancestor in node.GetAncestors(false))
				map.LazySet(nodeId).Add(Identify(ancestor));
		});

		return map;
	}

	public void Attach(Node<TItem> root)
	{
		_roots.Add(root);
		_nodes.Add(Identify(root), root);
	}

	public void Attach(TId parentId, Node<TItem> node)
	{
		if (!_nodes.ContainsKey(parentId))
			throw new ArgumentException("Matching parent not yet added", nameof(parentId));

		_nodes[parentId].Attach(node);
		_nodes.Add(Identify(node), node);
	}

	public void Detach(Node<TItem> node)
	{
		if (_roots.Contains(node))
			_roots.Remove(node);
		else
			node.DetachSelf();

		_nodes.Remove(Identify(node));
	}
}