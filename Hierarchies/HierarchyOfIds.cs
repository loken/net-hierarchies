using Loken.System.Collections;

namespace Loken.Hierarchies;

/// <summary>
/// A <see cref="Hierarchy{TId,TId}"/> where the items are the IDs.
/// Created through <see cref="Hierarchy"/>.
/// </summary>
/// <remarks>
/// These can be useful when you don't want to keep all of the items in memory,
/// but want to be able to reason around the relationships in a more light weight manner.
/// </remarks>
public class Hierarchy<TId> : Hierarchy<TId, TId>
	where TId : notnull
{
	internal Hierarchy() : base(id => id)
	{
	}

	internal Hierarchy(IDictionary<TId, ISet<TId>> childMap) : base(id => id)
	{
		foreach (var parentId in childMap.Keys)
		{
			var parentNode = new Node<TId>() { Item = parentId };
			_roots.Add(parentNode);
			_nodes.Add(parentId, parentNode);
		}

		foreach (var (parentId, childIds) in childMap)
		{
			var parentNode = _nodes[parentId];

			foreach (var childId in childIds)
			{
				var childNode = _nodes.Lazy(childId, () => new Node<TId> { Item = childId });
				parentNode.Attach(childNode);
				_roots.Remove(childNode);
			}
		}
	}
}