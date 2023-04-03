using System.Diagnostics;

namespace Loken.Hierarchies;

[DebuggerDisplay("Id: {" + nameof(Id) + "}")]
internal class Item<TId>
{
	public TId Id { get; set; }

	public Item(TId id)
	{
		Id = id;
	}
}