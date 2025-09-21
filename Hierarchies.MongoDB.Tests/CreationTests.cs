namespace Loken.Hierarchies.Data.MongoDB;

[Collection("MongoDB")]
public class CreationTests : IClassFixture<DbFixture>
{
	private readonly DbFixture Fixture;

	public CreationTests(DbFixture fixture)
	{
		Fixture = fixture;
	}

	[Fact]
	public void CreateRelationIndexes()
	{
		var collection = Fixture.GetDatabase().GetHierarchies<string>().CreateRelationIndexes();

		var indexNames = collection.Indexes.List()
			.ToList()
			.Select(d => d.GetValue("name").AsString)
			.ToArray();

		Assert.Equivalent("_id_|Id_Concept_Type|Concept_Type_Id|Type_Concept_Id".Split('|'), indexNames);
	}
}
