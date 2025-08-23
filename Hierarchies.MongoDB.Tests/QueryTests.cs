namespace Loken.Hierarchies.Data.MongoDB;

[Collection("MongoDB")]
public class QueryTests : IClassFixture<DbFixture>
{
	private readonly DbFixture Fixture;

	private const string ChildMap = """
		A:A1,A2
		A1:A11,A12
		B:B1
		B1:B12
		""";

	public QueryTests(DbFixture fixture)
	{
		Fixture = fixture;
	}

	[Fact]
	public void GetRelations()
	{
		var collection = Fixture.GetDatabase().GetHierarchies<string>().CreateRelationIndexes();
		var childMap = MultiMap.Parse<string>(ChildMap);
		var hc = Hierarchies.CreateFromChildMap(childMap);
		collection.InsertRelations(hc, "companies");

		var descendantMap = hc.ToMap(RelType.Descendants);
		var ancestorMap = hc.ToMap(RelType.Ancestors);

		var readChildMap = collection.GetRelations<string>("companies", RelType.Children).ToMap();
		var readDescendantsMap = collection.GetRelations<string>("companies", RelType.Descendants).ToMap();
		var readAncestorsMap = collection.GetRelations<string>("companies", RelType.Ancestors).ToMap();

		Assert.Equivalent(childMap, readChildMap);
		Assert.Equivalent(descendantMap, readDescendantsMap);
		Assert.Equivalent(ancestorMap, readAncestorsMap);
	}

	[Fact]
	public void GetChildren()
	{
		var collection = Fixture.GetDatabase().GetHierarchies<string>().CreateRelationIndexes();
		var childMap = MultiMap.Parse<string>(ChildMap);
		var hc = Hierarchies.CreateFromChildMap(childMap);
		collection.InsertRelations(hc, "companies", RelType.Children);

		var childrenOfA = collection.GetChildren("companies", "A");

		Assert.Equivalent(childMap["A"], childrenOfA);
	}
}