using Loken.System.Collections.MultiMap;

namespace Loken.Hierarchies;

public class NodeMapTests
{
	private static readonly Node<int> Root = Node.Create(0).Attach(
		Node.Create(1).Attach(
			Node.Create(11),
			Node.Create(12).Attach(Node.Create(121))),
		Node.Create(2),
		Node.Create(3).Attach(
			Node.Create(31),
			Node.Create(32)));

	[Fact]
	public void ToChildMap()
	{
		var expected = """
			0:1,2,3
			1:11,12
			3:31,32
			12:121
			""";

		var actual = Root.ToChildMap(id => id).RenderMultiMap();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void ToDescendantMap()
	{
		var expected = """
			0:1,2,3,11,12,31,32,121
			1:11,12,121
			3:31,32
			12:121
			""";

		var actual = Root.ToDescendantMap(id => id).RenderMultiMap();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void ToAncestorMap()
	{
		var expected = """
			1:0
			2:0
			3:0
			11:1,0
			12:1,0
			31:3,0
			32:3,0
			121:12,1,0
			""";

		var actual = Root.ToAncestorMap(id => id).RenderMultiMap();

		Assert.Equal(expected, actual);
	}
}
