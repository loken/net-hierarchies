namespace Loken.Hierarchies;

public class MultiMapToExtensionsTests
{
	private static readonly MultiMap<int> ChildMap = MultiMap.Parse<int>("""
		-1
		0:1,2,3
		1:11,12
		3:31,32
		12:121
		""");

	private static readonly int[] RootIds = [-1, 0];

	[Fact]
	public void ChildMapToParentMap()
	{
		var expected = """
			-1
			1:0
			2:0
			3:0
			11:1
			12:1
			31:3
			32:3
			121:12
			""";

		var actual = ChildMap.ToParentMap().Render();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void ChildMapToDescendantMap()
	{
		var expected = """
			-1
			0:1,2,3,11,12,31,32,121
			1:11,12,121
			3:31,32
			12:121
			""";

		var actual = ChildMap.ToDescendantMap().Render();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void ChildMapToAncestorMap()
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

		var actual = ChildMap.ToAncestorMap().Render();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void ChildMapToNodes()
	{
		var roots = ChildMap.ToNodes();
		var actualRootIds = roots.Select(r => r.Item).Order().ToArray();

		Assert.Equal(RootIds.Order().ToArray(), actualRootIds);
	}

	[Fact]
	public void ChildMapToNodes_NodesToChildMap_RoundTrip()
	{
		var nodes = ChildMap.ToNodes();
		var roundTripChildMap = nodes.ToChildMap(id => id);

		Assert.Equal(ChildMap.Render(), roundTripChildMap.Render());
	}

	[Fact]
	public void ChildMapToRelations()
	{
		var expected = new Relation<int>[]
		{
			new(-1),  // One-sided relation - Child defaults to null (isolated node)
			new(0, 1),
			new(0, 2),
			new(0, 3),
			new(1, 11),
			new(1, 12),
			new(3, 31),
			new(3, 32),
			new(12, 121),
		};

		var actual = ChildMap.ToRelations().ToArray();

		Assert.Equal(expected.Length, actual.Length);
		for (int i = 0; i < expected.Length; i++)
		{
			Assert.Equal(expected[i].Parent, actual[i].Parent);
			Assert.Equal(expected[i].Child, actual[i].Child);
			Assert.Equal(expected[i].ParentOnly, actual[i].ParentOnly);
		}
	}

	[Fact]
	public void ChildMapToRelations_RelationsToChildMap_RoundTrip()
	{
		var relations = ChildMap.ToRelations().ToArray();
		var roundTripChildMap = relations.ToChildMap();

		Assert.Equal(ChildMap.Render(), roundTripChildMap.Render());
	}

	[Fact]
	public void ChildMapToRootIds()
	{
		var expected = new HashSet<int> { -1, 0 };
		var actual = ChildMap.ToRootIds().ToHashSet();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void ChildMapToRootIds_WithEmptyMap()
	{
		var emptyMap = new MultiMap<int>();
		var actual = emptyMap.ToRootIds();

		Assert.Empty(actual);
	}

	[Fact]
	public void ChildMapToRootIds_WithSingleIsolatedNode()
	{
		var singleNodeMap = new MultiMap<int>();
		singleNodeMap.Add(42);
		var actual = singleNodeMap.ToRootIds();

		Assert.Equal(new[] { 42 }, actual);
	}
}
