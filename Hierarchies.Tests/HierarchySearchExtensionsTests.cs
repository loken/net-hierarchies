using System.Text.RegularExpressions;

namespace Loken.Hierarchies;

public class HierarchySearchExtensionsTests
{
	private static readonly MultiMap<string> ChildMap = MultiMap.Parse<string>("""
	A:A1,A2,A3
	B:B1,B2
	G:G1,G2,G3,G4,G5
	G1:G11,G12
	G2:G21,G22
	""");

	private record RItem(string Id, string Description);

	private static readonly RItem[] Items = [
		new("A",   "Alpha"),
		new("B",   "Beta"),
		new("G",   "Gamma"),
		new("A1",  "One"),
		new("A2",  "Two"),
		new("A3",  "Three"),
		new("B1",  "One"),
		new("B2",  "Two"),
		new("G1",  "One"),
		new("G2",  "Two"),
		new("G3",  "Three"),
		new("G4",  "Four"),
		new("G5",  "Five"),
		new("G11", "One"),
		new("G12", "Two"),
		new("G21", "One"),
		new("G22", "Two"),
	];

	private static readonly Hierarchy<RItem, string> Source = Hierarchy.CreateMapped(i => i.Id, Items, ChildMap);

	private static IEnumerable<(string Parent, string? Kid)> Canonical(MultiMap<string> map)
		=> map.ToRelations<string>()
			.Select(r => (Parent: r.Parent, Kid: r.ParentOnly ? null : r.Child))
			.OrderBy(t => t.Parent)
			.ThenBy(t => t.Kid);

	[Fact]
	public void Search_WithPredicate_ReturnsMatchesAncestorsDescendantsByDefault()
	{
		var search = new Regex("\\b(One|Beta)\\b", RegexOptions.IgnoreCase);
		var result = Source.Search(Items.Where(i => search.IsMatch(i.Description)).Select(i => i.Id), SearchInclude.All);

		var actual = result.ToChildMap();
		var expected = MultiMap.Parse<string>("""
		A:A1
		B:B1,B2
		G:G1,G2
		G1:G11,G12
		G2:G21
		""");

		Assert.Equal(Canonical(expected), Canonical(actual));
	}

	[Fact]
	public void Search_Ids_DefaultsToAll()
	{
		var result = Source.Search(["G2"]);

		var actual = result.ToChildMap();
		var expected = MultiMap.Parse<string>("""
		G:G2
		G2:G21,G22
		""");

		Assert.Equal(Canonical(expected), Canonical(actual));
	}

	[Fact]
	public void Search_Ids_MatchesOnly()
	{
		var result = Source.Search(["A", "G", "G2"], SearchInclude.Matches);

		var actual = result.ToChildMap();
		var expected = MultiMap.Parse<string>("""
		A
		G:G2
		""");

		Assert.Equal(Canonical(expected), Canonical(actual));
	}

	[Fact]
	public void Search_Ids_AncestorsOnly()
	{
		var result = Source.Search(["A", "G", "G2"], SearchInclude.Ancestors);

		var actual = result.ToChildMap();
		var expected = MultiMap.Parse<string>("""
		G
		""");

		Assert.Equal(Canonical(expected), Canonical(actual));
	}

	[Fact]
	public void Search_Ids_DescendantsOnly()
	{
		var result = Source.Search(["A", "G", "G2"], SearchInclude.Descendants);

		var actual = result.ToChildMap();
		var expected = MultiMap.Parse<string>("""
		A1
		A2
		A3
		G1
		G2:G21,G22
		G1:G11,G12
		G3
		G4
		G5
		""");

		Assert.Equal(Canonical(expected), Canonical(actual));
	}

	[Fact]
	public void Search_Ids_AncestorsAndDescendants_NoMatches()
	{
		var result = Source.Search(["G2"], SearchInclude.Ancestors | SearchInclude.Descendants);

		var actual = result.ToChildMap();
		var expected = MultiMap.Parse<string>("""
		G
		G21
		G22
		""");

		Assert.Equal(Canonical(expected), Canonical(actual));
	}
}
