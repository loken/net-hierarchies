namespace Loken.Hierarchies;

public class HierarchyCreationTests
{
	[Fact]
	public void CreateRelational_Ids_ReturnsCorrectRoots()
	{
		var hc = Hierarchy.CreateRelational<string>(
			new("A", "A1"),
			new("A", "A2"),
			new("A2", "A21"),
			new("B", "B1"),
			new("B1", "B11"),
			new("B1", "B12"),
			new("B1", "B13"));

		Assert.Equal(2, hc.Roots.Count);
		Assert.Equivalent(new[] { "A", "B" }, hc.Roots.Select(r => r.Item).ToArray());
	}

	[Fact]
	public void CreateRelational_Items_ReturnsCorrectRoots()
	{
		var hc = Hierarchy.CreateRelational(
			item => item.Id,
			[
				new Item<string>("A"),
				new Item<string>("A1"),
				new Item<string>("A2"),
				new Item<string>("A21"),
				new Item<string>("B"),
				new Item<string>("B1"),
				new Item<string>("B11"),
				new Item<string>("B12"),
				new Item<string>("B13"),
			],
			[
				new("A", "A1"),
				new("A", "A2"),
				new("A2", "A21"),
				new("B", "B1"),
				new("B1", "B11"),
				new("B1", "B12"),
				new("B1", "B13")
			]);

		Assert.Equal(2, hc.Roots.Count);
		Assert.Equivalent(new[] { "A", "B" }, hc.Roots.Select(r => r.Item.Id).ToArray());
	}

	[Fact]
	public void CreateMapped_ReturnsCorrectRoots()
	{
		var childMap = MultiMap.Parse<string>("""
		A:A1,A2
		A1:A11,A12
		B:B1
		B1:B12
		""");

		var hc = Hierarchy.CreateMapped(childMap);

		Assert.Equal(2, hc.Roots.Count);
		Assert.Equivalent(new[] { "A", "B" }, hc.Roots.Select(r => r.Item).ToArray());
	}

	[Fact]
	public void CreateMatching_ReturnsTheSameStructure()
	{
		var childMap = MultiMap.Parse<string>("""
		A:A1,A2
		A2:A21
		B:B1
		B1:B11,B12,B13
		""");

		var idHierarchy = Hierarchy.CreateMapped(childMap);

		var items = new[]
		{
			new Item<string>("A"),
			new Item<string>("A1"),
			new Item<string>("A2"),
			new Item<string>("A21"),
			new Item<string>("B"),
			new Item<string>("B1"),
			new Item<string>("B11"),
			new Item<string>("B12"),
			new Item<string>("B13")
		};
		var matchingHierarchy = Hierarchy.CreateMatching(item => item.Id, items, idHierarchy);

		var idStructure = idHierarchy.ToChildMap().Render();
		var matchingStructure = matchingHierarchy.ToChildMap().Render();
		Assert.Equal(idStructure, matchingStructure);
	}
}
