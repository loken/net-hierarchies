namespace Loken.Hierarchies;

public class NodesFromItemsTests
{
	private const string Input = """
		A:A1,A2
		B:B1
		C
		A1:A11,A12
		B1:B12
		""";

	private static readonly string[] InputRoots = ["A", "B", "C"];
	private static readonly MultiMap<string> InputMap = MultiMap.Parse<string>(Input);

	[Fact]
	public void FromItemsWithChildMap()
	{
		var items = "A,B,C,A1,A2,B1,A11,A12,B12".Split(',').Select(id => new Item<string>(id)).ToArray();

		var roots = Nodes.FromItemsWithChildMap(items, item => item.Id, InputMap);

		Assert.Equal(InputRoots, roots.ToIds(item => item.Id));

		var output = roots.ToChildMap(item => item.Id).Render();

		Assert.Equal(Input, output);
	}

	[Fact]
	public void FromItemsWithChildMap_IgnoresItemsNotInChildMap()
	{
		var items = "A,B,C,A1,A2,B1,A11,A12,B12,IGNORED".Split(',').Select(id => new Item<string>(id)).ToArray();

		var roots = Nodes.FromItemsWithChildMap(items, item => item.Id, InputMap);

		Assert.Equal(InputRoots, roots.ToIds(item => item.Id));

		var output = roots.ToChildMap(item => item.Id).Render();

		Assert.Equal(Input, output);
	}

	[Fact]
	public void FromItemsWithChildren()
	{
		// Represents the same structure as the relations
		var itemsWithChildren = new ItemWithChildren[]
		{
			new("A", [
				new("A1", [
					new("A11"),
					new("A12"),
				]),
				new("A2"),
			]),
			new("B", [
				new("B1", [new("B12")]),
			]),
			new("C"),
		};

		var roots = Nodes.FromItemsWithChildren(itemsWithChildren, item => item.Children);

		Assert.Equal(InputRoots, roots.ToIds(item => item.Id));

		var actual = roots.ToChildMap(item => item.Id).Render();

		Assert.Equal(Input, actual);
	}

	[Fact]
	public void FromItemsWithParents()
	{
		// Create a map of items
		var itemsWithParents = new Dictionary<string, ItemWithParent>();
		foreach (var id in InputMap.GetAll())
			itemsWithParents[id] = new ItemWithParent(id);

		// Set the parent references
		foreach (var (parentId, childIds) in InputMap)
		{
			var parent = itemsWithParents[parentId];
			foreach (var childId in childIds)
			{
				var child = itemsWithParents[childId];
				child.Parent ??= parent;
			}
		}

		// Ensure we can pass it all of the items
		var allItems = itemsWithParents.Values.ToArray();
		var rootsFromAll = Nodes.FromItemsWithParents(allItems, item => item.Parent);

		var actualFromAll = rootsFromAll.ToChildMap(item => item.Id);
		Assert.Equal(InputMap, actualFromAll);

		// Ensure we can pass it the leaf items only
		var leafItems = itemsWithParents.Values.Where(item => !InputMap.Get(item.Id)?.Any() ?? true).ToArray();
		var rootsFromLeaves = Nodes.FromItemsWithParents(leafItems, item => item.Parent);

		var actualFromLeaves = rootsFromLeaves.ToChildMap(item => item.Id);
		Assert.Equal(InputMap, actualFromLeaves);
	}

	private record ItemWithChildren(string Id, ItemWithChildren[]? Children = null);

	private record ItemWithParent(string Id)
	{
		public ItemWithParent? Parent { get; set; }
	}
}
