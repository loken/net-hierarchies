namespace Loken.Hierarchies.Data;

public class HierarchyChangesTests
{
	[Fact]
	public void Difference()
	{
		var map = """
		A:A1,A2
		A2:A21
		B:B1
		B1:B11,B12,B13
		""".ParseMultiMap();

		var oldHierarchy = Hierarchy.CreateMapped(map);
		var newHierarchy = Hierarchy.CreateMapped(map);

		newHierarchy
			.Detach("A")
			.AttachRoot("C", "D")
			.Attach("D", "D1")
			.Detach("B11")
			.Attach("B1", "B14");

		var diff = HierarchyChanges.Difference(oldHierarchy, newHierarchy, "companies", RelType.Children);

		var sep = MultiMapSeparators.Create("~");
		var expected = new
		{
			// Detaching A also deletes its attached children.
			deleted = "A,A2".SplitBy(',').ToHashSet(),
			// We've inserted two roots, one of which has a child.
			inserted = "C~D:D1".ParseMultiMap(sep),
			// We've removed a child.
			removed = "B1:B11".ParseMultiMap(sep),
			// We've added a child.
			added = "B1:B14".ParseMultiMap(sep),
		};

		Assert.Equivalent(expected.deleted, diff.Deleted);
		Assert.Equivalent(expected.inserted, diff.Inserted);
		Assert.Equivalent(expected.removed, diff.Removed);
		Assert.Equivalent(expected.added, diff.Added);
	}
}
