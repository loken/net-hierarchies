namespace Loken.Hierarchies.Data.MongoDB;

[Collection("MongoDB")]
public class UpdateTests : IClassFixture<DbFixture>
{
	private readonly DbFixture Fixture;

	public UpdateTests(DbFixture fixture)
	{
		Fixture = fixture;
	}

	[Fact]
	public void UpdateRelations_UsingStoredState()
	{
		var collection = Fixture.GetDatabase().GetHierarchies<string>().CreateRelationIndexes();
		var childMap = MultiMap.Parse<string>("""
			A:A1,A2
			A2:A21
			B:B1
			B1:B11,B12,B13
			""");

		// Create a hierarchy and store it to the database.
		var hierarchy = Hierarchy.CreateMapped(childMap);
		collection.InsertRelations(hierarchy, "companies", RelType.Children);

		// Make modifications to the hierarchy.
		hierarchy
			.Detach("A")
			.AttachRoot("C", "D")
			.Attach("D", "D1")
			.Detach("B11")
			.Attach("B1", "B14");

		// Update the database using a difference between the stored relations and the hierarchy.
		collection.UpdateRelations(hierarchy, "companies", RelType.Children);

		// The relations in the database should now match the relations in our hierarchy.
		var updatedMap = collection.GetRelations("companies", RelType.Children).ToMap();
		var expectedMap = hierarchy.ToMap(RelType.Children);
		Assert.Equivalent(expectedMap, updatedMap);
	}

	[Fact]
	public void UpdateRelations_UsingKnownState()
	{
		var collection = Fixture.GetDatabase().GetHierarchies<string>().CreateRelationIndexes();
		var childMap = MultiMap.Parse<string>("""
			A:A1,A2
			A2:A21
			B:B1
			B1:B11,B12,B13
			""");

		// Keep an instance of the known state and store it to the database.
		var known = Hierarchy.CreateMapped(childMap);
		collection.InsertRelations(known, "companies", RelType.Children);

		// Create a working copy, modify it and update the database.
		var modified = Hierarchy.CreateMatching(known);
		modified
			.Detach("A")
			.AttachRoot("C", "D")
			.Attach("D", "D1")
			.Detach("B11")
			.Attach("B1", "B14");

		// Update the database with changes that occurred between the known- and modified state.
		collection.UpdateRelations(known, modified, "companies", RelType.Children);

		// The relations in the database should now match the relations in our modified state.
		var updatedMap = collection.GetRelations("companies", RelType.Children).ToMap();
		var expectedMap = modified.ToMap(RelType.Children);
		Assert.Equivalent(expectedMap, updatedMap);
	}
}
