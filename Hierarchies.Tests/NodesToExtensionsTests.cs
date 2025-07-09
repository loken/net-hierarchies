namespace Loken.Hierarchies;

public class NodesToExtensionsTests
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
	public void NodesToChildMap()
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
	public void NodesToDescendantMap()
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
	public void NodesToAncestorMap()
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

	[Fact]
	public void NodesToRelations()
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

		var actual = Roots.ToRelations(id => id).ToArray();

		Assert.Equal(expected.Length, actual.Length);
		for (int i = 0; i < expected.Length; i++)
		{
			Assert.Equal(expected[i].Parent, actual[i].Parent);
			Assert.Equal(expected[i].Child, actual[i].Child);
			Assert.Equal(expected[i].ParentOnly, actual[i].ParentOnly);
		}
	}

	[Fact]
	public void NodesToRelations_RelationsToNodes_RoundTrip()
	{
		var relations = Roots.ToRelations(id => id).ToArray();
		var roundTripRoots = relations.ToNodes();

		// Verify structure is preserved by comparing descendant items
		var originalItems = Roots.SelectMany(r => r.ToEnumerable()).Select(n => n.Item).Order().ToArray();
		var roundTripItems = roundTripRoots.SelectMany(r => r.ToEnumerable()).Select(n => n.Item).Order().ToArray();

		Assert.Equal(originalItems, roundTripItems);
	}
}
