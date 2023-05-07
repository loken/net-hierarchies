using Loken.System.Collections.MultiMap;

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
	public void UpdateRelations_WithChanges_CausesStoredRelationsToMatch()
	{
		var collection = Fixture.GetDatabase().GetHierarchies<string>().CreateRelationIndexes();
		var childMap = """
			A:A1,A2
			A2:A21
			B:B1
			B1:B11,B12,B13
			""".ParseMultiMap();

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
