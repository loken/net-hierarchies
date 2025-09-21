namespace Loken.Hierarchies;

public class RelationsToExtensionsTests
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

	private static readonly Relation<int>[] Relations = [
		new(-1),  // One-sided relation - Child defaults to null (isolated node)
		new(0, 1),
		new(0, 2),
		new(0, 3),
		new(1, 11),
		new(1, 12),
		new(3, 31),
		new(3, 32),
		new(12, 121),
	];

	private static readonly MultiMap<int> ChildMap = MultiMap.Parse<int>("""
		-1
		0:1,2,3
		1:11,12
		3:31,32
		12:121
		""");

	[Fact]
	public void RelationsToNodes()
	{
		var actualRoots = Relations.ToNodes();

		var expectedIds = Roots.SelectMany(r => r.ToEnumerable()).Select(n => n.Item).Order().ToArray();
		var actualIds = actualRoots.SelectMany(r => r.ToEnumerable()).Select(n => n.Item).Order().ToArray();

		Assert.Equal(expectedIds, actualIds);
	}

	[Fact]
	public void RelationsToChildMap()
	{
		var actual = Relations.ToChildMap();

		Assert.Equal(ChildMap.Render(), actual.Render());
	}

	[Fact]
	public void RelationsToNodes_NodesToRelations_RoundTrip()
	{
		var nodes = Relations.ToNodes();
		var roundTripRelations = nodes.ToRelations(id => id).ToArray();

		Assert.Equal(Relations.Length, roundTripRelations.Length);
		for (int i = 0; i < Relations.Length; i++)
		{
			Assert.Equal(Relations[i].Parent, roundTripRelations[i].Parent);
			Assert.Equal(Relations[i].Child, roundTripRelations[i].Child);
			Assert.Equal(Relations[i].ParentOnly, roundTripRelations[i].ParentOnly);
		}
	}

	[Fact]
	public void RelationsToChildMap_ChildMapToRelations_RoundTrip()
	{
		var childMapFromRelations = Relations.ToChildMap();
		var roundTripRelations = childMapFromRelations.ToRelations().ToArray();

		Assert.Equal(Relations.Length, roundTripRelations.Length);
		for (int i = 0; i < Relations.Length; i++)
		{
			Assert.Equal(Relations[i].Parent, roundTripRelations[i].Parent);
			Assert.Equal(Relations[i].Child, roundTripRelations[i].Child);
			Assert.Equal(Relations[i].ParentOnly, roundTripRelations[i].ParentOnly);
		}
	}
}
