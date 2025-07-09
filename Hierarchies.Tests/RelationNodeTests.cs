namespace Loken.Hierarchies;

public class RelationNodeTests
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
	public void ToRelations()
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

		var actual = Roots.ToChildMap(id => id).ToRelations().ToArray();

		Assert.Equal(expected.Length, actual.Length);
		for (int i = 0; i < expected.Length; i++)
		{
			Assert.Equal(expected[i].Parent, actual[i].Parent);
			Assert.Equal(expected[i].Child, actual[i].Child);
			Assert.Equal(expected[i].ParentOnly, actual[i].ParentOnly);
		}
	}

	[Fact]
	public void ToRootNodes()
	{
		var relations = new Relation<int>[]
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

		var actualRoots = relations.ToNodes();

		// Should have 2 roots: the isolated node (-1) and the tree root (0)
		Assert.Equal(2, actualRoots.Length);

		// Check isolated root
		var isolatedRoot = actualRoots.Single(r => r.Item.Equals(-1));
		Assert.True(isolatedRoot.IsLeaf);
		Assert.True(isolatedRoot.IsRoot);

		// Check tree root structure
		var treeRoot = actualRoots.Single(r => r.Item.Equals(0));
		Assert.False(treeRoot.IsLeaf);
		Assert.True(treeRoot.IsRoot);

		// Verify the structure matches the original
		var expectedChildMap = Roots.ToChildMap(id => id).Render();
		var actualChildMap = actualRoots.ToChildMap(id => id).Render();
		Assert.Equal(expectedChildMap, actualChildMap);
	}

	[Fact]
	public void ToRelations_ToRootNodes_RoundTrip()
	{
		// Start with the original structure
		var originalChildMap = Roots.ToChildMap(id => id);

		// Convert to relations and back
		var relations = originalChildMap.ToRelations();
		var relationObjects = relations.Select(r => (Relation<int>)r).ToArray();
		var roundTripRoots = relationObjects.ToNodes();
		var roundTripChildMap = roundTripRoots.ToChildMap(id => id);

		// Should be identical
		Assert.Equal(originalChildMap.Render(), roundTripChildMap.Render());
	}

	[Fact]
	public void ToRootNodes_ToRelations_RoundTrip()
	{
		var originalRelations = new Relation<int>[]
		{
			new(-1),  // One-sided relation - Child defaults to null
			new(0, 1),
			new(1, 11),
			new(1, 12),
		};

		// Convert to nodes and back to relations
		var roots = originalRelations.ToNodes();
		var roundTripRelations = roots.ToChildMap(id => id).ToRelations().ToArray();

		// Should have the same relations (order may differ)
		Assert.Equal(originalRelations.Length, roundTripRelations.Length);

		foreach (var originalRelation in originalRelations)
		{
			Assert.Contains(roundTripRelations, r =>
				r.Parent.Equals(originalRelation.Parent) &&
				Equals(r.Child, originalRelation.Child));
		}
	}
}
