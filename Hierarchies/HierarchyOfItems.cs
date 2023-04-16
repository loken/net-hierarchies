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

	protected readonly Dictionary<TId, Node<TItem>> _nodes = new();
	protected readonly List<Node<TItem>> _roots = new();

	[IgnoreDataMember, JsonIgnore]
	public IReadOnlyCollection<Node<TItem>> Nodes => _nodes.Values;

	[DataMember, JsonInclude]
	public IReadOnlyList<Node<TItem>> Roots { get; }

	internal Hierarchy(Func<TItem, TId> identify)
	{
		Identify = identify;

		Roots = new ReadOnlyCollection<Node<TItem>>(_roots);
	}

	internal Hierarchy(Func<TItem, TId> identify, IEnumerable<TItem> items, IDictionary<TId, ISet<TId>> childMap) : this(identify)
	{
		var cache = new Dictionary<TId, Node<TItem>>();

		foreach (var item in items)
		{
			var id = identify(item);
			var node = new Node<TItem>() { Item = item };

			cache.Add(id, node);

			if (childMap.ContainsKey(id))
			{
				_nodes.Add(id, node);
				_roots.Add(node);
			}
		}

		foreach (var (parentId, childIds) in childMap)
		{
			var parentNode = cache[parentId];

			foreach (var childId in childIds)
			{
				var childNode = cache[childId];

				parentNode.Attach(childNode);

				_roots.Remove(childNode);
				_nodes.TryAdd(childId, childNode);
			}
		}
	}

	public Node<TItem> GetNode(TId id)
	{
		return _nodes[id];
	}

	public TItem Get(TId id)
	{
		return GetNode(id).Item;
	}

	public IEnumerable<(TId parent, TId child)> ToRelations()
	{
		return ToChildMap().ToRelations();
	}

	public IDictionary<TId, ISet<TId>> ToChildMap()
	{
		return Roots.ToChildMap(Identify);
	}

	public void Attach(Node<TItem> root)
	{
		_roots.Add(root);
		_nodes.Add(Identify(root.Item), root);
	}

	public void Attach(TId parentId, Node<TItem> node)
	{
		if (!_nodes.ContainsKey(parentId))
			throw new ArgumentException("Matching parent not yet added", nameof(parentId));

		_nodes[parentId].Attach(node);
		_nodes.Add(Identify(node.Item), node);
	}

	public void Detach(Node<TItem> node)
	{
		if (_roots.Contains(node))
			_roots.Remove(node);
		else
			node.DetachSelf();

		_nodes.Remove(Identify(node.Item));
	}
}