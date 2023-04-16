using System.Diagnostics;

namespace Loken.Hierarchies;

[DebuggerDisplay("Id: {" + nameof(Id) + "}")]
internal class Item<TId>
{
	public TId Id { get; private set; }

	public Item(TId id)
	{
		Id = id;
	}
}