using Loken.System.Collections.MultiMap;

namespace Loken.Hierarchies;

public class HierarchyTraversalTests
{
	private static readonly Hierarchy<string> _hierarchy = Hierarchy.FromChildMap("""
	A:A1,A2
	A1:A11,A12
	A2:A21
	B:B1
	B1:B12
	""".ParseMultiMap());

	[Fact]
	public void GetAncestorIds_WithSelf_ReturnsAncestryInOrder()
	{
		var ancestors = _hierarchy.GetAncestorIds("A12", true).ToList();

		var expected = new List<string> { "A12", "A1", "A" };

		Assert.Equivalent(expected, ancestors);
	}

	[Fact]
	public void GetAncestorIds_WithoutSelf_ReturnsAncestryInOrder()
	{
		var ancestors = _hierarchy.GetAncestorIds("A12", false).ToList();

		var expected = new List<string> { "A1", "A" };

		Assert.Equivalent(expected, ancestors);
	}

	[Fact]
	public void GetDescendantIds_WithSelf_ReturnsDescendants()
	{
		var ancestors = _hierarchy.GetDescendantIds("A1", true).ToList();

		var expected = new List<string> { "A1", "A11", "A12" };

		Assert.Equivalent(expected, ancestors);
	}

	[Fact]
	public void GetDescendantIds_WithoutSelf_ReturnsDescendantsOrderedByDepth()
	{
		var ancestors = _hierarchy.GetDescendantIds("A", false).ToList();

		var expected = new List<string> { "A1", "A2", "A11", "A12", "A21" };

		Assert.Equivalent(expected, ancestors);
	}
}
