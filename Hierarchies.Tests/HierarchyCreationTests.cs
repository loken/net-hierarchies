using Loken.System.Collections;
using Loken.System.Collections.MultiMap;

namespace Loken.Hierarchies;

public class HierarchyCreationTests
{
	[Fact]
	public void Attach_FromRootsDown_Works()
	{
		var hc = Hierarchy.Create<string>();

		hc.Attach("A");
		hc.Attach("A", "A1");
		hc.Attach("A", "A2");
		hc.Attach("A2", "A21");
		hc.Attach("B");
		hc.Attach("B", "B1");
		hc.Attach("B1", "B11");

		Assert.Equal(2, hc.Roots.Count);
		Assert.Equivalent(new[] { "A", "B" }, hc.Roots.Select(r => r.Value).ToArray());
	}

	[Fact]
	public void Attach_ToNonExistentParent_Throws()
	{
		var hc = Hierarchy.Create<string>();

		Assert.Throws<ArgumentException>(() => hc.Attach("NonExistentParentId", "Node"));
	}

	[Fact]
	public void Attach_PreBuiltRoot_Works()
	{
		var node = Node.Create("A");
		node.Attach("A1");
		node.Attach("A2");
		node.Attach("A21");

		var hc = Hierarchy.Create<string>();
		hc.Attach(node);

		Assert.Equal(1, hc.Roots.Count);
		Assert.Equivalent(new[] { "A" }, hc.Roots.Select(r => r.Value).ToArray());
	}

	[Fact]
	public void BuildStringItems_ReturnsTheCorrectRoots()
	{
		var hc = Hierarchy.FromRelations<string>(
			("A", "A1"),
			("A", "A2"),
			("A2", "A21"),
			("B", "B1"),
			("B1", "B11"),
			("B1", "B12"),
			("B1", "B13"));

		Assert.Equal(2, hc.Roots.Count);
		Assert.Equivalent(new[] { "A", "B" }, hc.Roots.Select(r => r.Value).ToArray());
	}

	[Fact]
	public void BuildObjectItems_ReturnsTheCorrectRoots()
	{
		var hc = Hierarchy.Create(item => item.Id,
				new Item<string>("A"),
				new Item<string>("A1"),
				new Item<string>("A2"),
				new Item<string>("A21"),
				new Item<string>("B"),
				new Item<string>("B1"),
				new Item<string>("B11"),
				new Item<string>("B12"),
				new Item<string>("B13"))
			.UsingRelations(
				("A", "A1"),
				("A", "A2"),
				("A2", "A21"),
				("B", "B1"),
				("B1", "B11"),
				("B1", "B12"),
				("B1", "B13"));

		Assert.Equal(2, hc.Roots.Count);
		Assert.Equivalent(new[] { "A", "B" }, hc.Roots.Select(r => r.Value.Id).ToArray());
	}

	[Fact]
	public void Create_UsingOther()
	{
		var idHierarchy = Hierarchy.FromRelations<string>(
			("A", "A1"),
			("A", "A2"),
			("A2", "A21"),
			("B", "B1"),
			("B1", "B11"),
			("B1", "B12"),
			("B1", "B13"));

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

		var hc = Hierarchy.Create(item => item.Id, items).UsingOther(idHierarchy);

		Assert.Equal(2, hc.Roots.Count);
		Assert.Equivalent(new[] { "A", "B" }, hc.Roots.Select(r => r.Value.Id).ToArray());
	}

	[Fact]
	public void Create_FromChildMap_Dictionary()
	{
		var childMap = new Dictionary<string, ISet<string>>();
		childMap.LazySet("A").AddRange("A1", "A2");
		childMap.LazySet("A1").AddRange("A11", "A12");
		childMap.LazySet("B").AddRange("B1", "B2");

		var hc = Hierarchy.FromChildMap(childMap);

		Assert.Equal(2, hc.Roots.Count);
		Assert.Equivalent(new[] { "A", "B" }, hc.Roots.Select(r => r.Value).ToArray());
	}

	[Fact]
	public void Create_FromChildMap_String()
	{
		var hc = Hierarchy.FromChildMap("""
		A:A1,A2
		A1:A11,A12
		B:B1,B12
		""".ParseMultiMap());

		Assert.Equal(2, hc.Roots.Count);
		Assert.Equivalent(new[] { "A", "B" }, hc.Roots.Select(r => r.Value).ToArray());
	}
}
