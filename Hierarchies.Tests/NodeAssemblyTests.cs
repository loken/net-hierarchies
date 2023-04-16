using Loken.System.Collections.MultiMap;

namespace Loken.Hierarchies;

public class NodeAssemblyTests
{
	[Fact]
	public void Assemble_Ids()
	{
		var childMap = """
			A:A1,A2
			B:B1
			A1:A11,A12
			B1:B12
			""";

		var roots = Node.Assemble(childMap.ParseMultiMap());

		var output = roots.ToChildMap(id => id).RenderMultiMap();

		Assert.Equal(childMap, output);
	}

	[Fact]
	public void Assemble_Items()
	{
		var childMap = """
			A:A1,A2
			B:B1
			A1:A11,A12
			B1:B12
			""";

		var items = "A,B,A1,A2,B1,A11,A12,B12".Split(',').Select(s => new Item<string>(s)).ToArray();

		var roots = Node.Assemble(item => item.Id, items, childMap.ParseMultiMap());

		var output = roots.ToChildMap(item => item.Id).RenderMultiMap();

		Assert.Equal(childMap, output);
	}
}
