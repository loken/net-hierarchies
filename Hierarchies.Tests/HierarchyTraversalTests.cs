using Loken.System;
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

	[Fact]
	public void Traverse_Processes_BreadthFirst()
	{
		var expected = "A,B,A1,A2,B1,A11,A12,A21,B12".SplitBy(',');

		var traversed = new List<(string item, uint depth)>();
		_hierarchy.Traverse((item, depth) =>
		{
			traversed.Add((item, depth));
			return false;
		});

		var actualItems = traversed.Select(t => t.item).ToArray();

		Assert.Equivalent(expected, actualItems);
	}

	[Fact]
	public void Traverse_Processes_WithCorrectDepth()
	{
		var traversed = new List<(string item, uint depth)>();
		_hierarchy.Traverse((item, depth) =>
		{
			traversed.Add((item, depth));
			return false;
		});

		// Since the items are set up using a nomenclature such that its depth is the item.Length-1, assert it!
		Assert.All(traversed, ((string item, uint depth) t) => Assert.Equal((uint)t.item.Length - 1, t.depth));
	}
}