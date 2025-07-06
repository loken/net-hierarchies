namespace Loken.Hierarchies;

public class RelationMapTests
{
	private static readonly Node<int>[] Roots = [
		Nodes.Create(-1), // Isolated root
		Nodes.Create(0).Attach(
			Nodes.Create(1).Attach(
				Nodes.Create(11),
				Nodes.Create(12).Attach(Nodes.Create(121))),
			Nodes.Create(2),
			Nodes.Create(3).Attach(
				Nodes.Create(31),
				Nodes.Create(32)))
	];

	[Fact]
	public void ToChildMap()
	{
		var expected = """
			-1
			0:1,2,3
			1:11,12
			3:31,32
			12:121
			""";

		var actual = Roots.ToChildMap(id => id).Render();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void ToDescendantMap()
	{
		var expected = """
			-1
			0:1,2,3,11,12,31,32,121
			1:11,12,121
			3:31,32
			12:121
			""";

		var actual = Roots.ToDescendantMap(id => id).Render();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void ToAncestorMap()
	{
		var expected = """
			-1
			1:0
			2:0
			3:0
			11:1,0
			12:1,0
			31:3,0
			32:3,0
			121:12,1,0
			""";

		var actual = Roots.ToAncestorMap(id => id).Render();

		Assert.Equal(expected, actual);
	}
}
