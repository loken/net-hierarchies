namespace Loken.Hierarchies;

public class ReadmeSnippetsTests
{
	private sealed record Item(string? ParentId, string Id, string Name);

	[Fact]
	public void Compile_Readme_Snippets()
	{
		#region Preamble
		Item root = new (null, "r", "root");
		Item a  = new ("r", "a", "branch-A");
		Item b  = new ("r", "b", "branch-B");
		Item a1 = new ("a", "a1", "leaf-A1");
		Item a2 = new ("a", "a2", "leaf-A2");
		Item[] items = [ root, a, b, a1, a2 ];

		Relation<string>[] relations =
		[
			new("r"),
			new("r", "a"),
			new("r", "b"),
			new("a", "a1"),
			new("a", "a2"),
		];
		var childMap = MultiMap.Parse<string>("""
			r:a,b
			a:a1,a2
			""");
		#endregion


		#region Quick start
		var hierarchy1 = Hierarchies.CreateFromParents(items, item => item.Id, item => item.ParentId);
		var hierarchy2 = Hierarchies.CreateFromRelations(items, item => item.Id, [
			new("r", "a"),
			new("r", "b"),
		]);

		// Retrieve and project descendants or ancestors.
		var getNodes = hierarchy1.GetDescendants("a", includeSelf: true);
		var getItems = hierarchy1.GetDescendantItems("a", includeSelf: true);
		var getIds   = hierarchy1.GetDescendantIds("a", includeSelf: true);

		// Find matches by predicate
		var foundNodes = hierarchy1.Find(n => n.Item.Name == "a");
		var foundItems = hierarchy1.FindItems(n => n.Item.Name == "a");
		var foundIds   = hierarchy1.FindIds(n => n.Item.Name == "a");

		// Search to create a pruned clone of the hierarchy.
		var prunedHierarchy = hierarchy1.Search("a", SearchInclude.Matches | SearchInclude.Descendants);

		// Clone the entire hierarchy.
		var clonedHierarchy = hierarchy1.Clone();
		#endregion


		#region Relations
		// Mapping between structure representations
		var relationsFromHierarchy = hierarchy1.ToRelations();
		var childMapFromHierarchy  = hierarchy1.ToChildMap();
		var nodesFromRelations     = relationsFromHierarchy.ToNodes();
		var nodesFromChildMap      = childMapFromHierarchy.ToNodes();
		// Serialize between child map and text
		var textChildMap           = childMap.Render();
		var parsedChildMap         = MultiMap.Parse<string>(textChildMap);
		#endregion


		#region Build imperatively
		var hierarchy = Hierarchies.Create<Item, string>(i => i.Id);

		// Create nodes and attach them to each other
		var branchNodes = Nodes.CreateMany(a, b);
		var rootNode    = Nodes.Create(root).Attach(branchNodes);

		// Attach the root node as a hierarchy root. (Yes we can have multiple roots.)
		hierarchy.AttachRoot(rootNode);

		// Create some leaf nodes and attach them to the "a" branch of the hierarchy directly.
		var leafNodes = Nodes.CreateMany(a1, a2);
		hierarchy.Attach("a", leafNodes);
		#endregion


		#region Create from items with parent IDs
		var parentedHc = Hierarchies.CreateFromParents(items, item => item.Id, item => item.ParentId);
		#endregion


		#region Create from relations or child map
		var idHierarchyFromRelations   = Hierarchies.CreateFromRelations(relations);
		var itemHierarchyFromRelations = Hierarchies.CreateFromRelations(items, item => item.Id, relations);

		var idHierarchyFromMap         = Hierarchies.CreateFromChildMap(childMap);
		var itemHierarchyFromMap       = Hierarchies.CreateFromChildMap(items, item => item.Id, childMap);
		#endregion


		#region Create from hierarchy
		// Create ID-hierarchy matching an item-hierarchy's structure
		var matchingIdHc = Hierarchies.CreateFromHierarchy(parentedHc);

		// Create item-hierarchy matching an ID-hierarchy's structure
		var matchingItemHc = Hierarchies.CreateFromHierarchy(items, item => item.Id, matchingIdHc);
		#endregion


		#region Get by ID
		// Check existence
		hierarchy.Has("a");
		hierarchy.HasAny(["a", "b"]);
		hierarchy.HasAll(["a", "b"]);

		// Get a single item
		hierarchy.GetNode("a");
		hierarchy.GetItem("a");

		// Get multiple items
		hierarchy.GetNodes(["a", "b"]);
		hierarchy.GetItems(["a", "b"]);
		#endregion

		#region Find by predicate
		hierarchy.Find(n => n.Item.Name == "a");
		hierarchy.FindItems(n => n.Item.Name == "a");
		hierarchy.FindIds(n => n.Item.Name == "a");
		#endregion

		#region Get descendants and ancestors
		// Get descendants/ancestors by ID
		hierarchy.GetDescendants("a");
		hierarchy.GetDescendantItems("a");
		hierarchy.GetDescendantIds("a");
		// Get descendants/ancestors by IDs
		hierarchy.GetDescendants(["a", "b"]);
		hierarchy.GetDescendantItems(["a", "b"]);
		hierarchy.GetDescendantIds(["a", "b"]);
		#endregion

		#region Find descendants and ancestors
		// Find the first matching descendant of a single starting node.
		hierarchy.FindDescendant("a", "a2");
		hierarchy.FindDescendant("a", ["a1", "a2"]);
		hierarchy.FindDescendant("a", n => n.Item.Id.EndsWith('2'));

		// Find the first matching descendant of a multiple starting nodes.
		hierarchy.FindDescendant(["a", "b"], "a2");
		hierarchy.FindDescendant(["a", "b"], ["a1", "a2"]);
		hierarchy.FindDescendant(["a", "b"], n => n.Item.Id.EndsWith('2'));

		// Find all matching descendants of a single starting node.
		hierarchy.FindDescendants("a", ["a1", "a2"]);
		hierarchy.FindDescendants("a", n => n.Item.Id.EndsWith('2'));
		hierarchy.FindDescendantIds("a", ["a1", "a2"]);
		hierarchy.FindDescendantIds("a", n => n.Item.Id.EndsWith('2'));
		hierarchy.FindDescendantItems("a", ["a1", "a2"]);
		hierarchy.FindDescendantItems("a", n => n.Item.Id.EndsWith('2'));

		// Find all matching descendants of multiple starting nodes.
		hierarchy.FindDescendants(["a", "b"], ["a1", "a2"]);
		hierarchy.FindDescendants(["a", "b"], n => n.Item.Id.EndsWith('2'));
		hierarchy.FindDescendantIds(["a", "b"], ["a1", "a2"]);
		hierarchy.FindDescendantIds(["a", "b"], n => n.Item.Id.EndsWith('2'));
		hierarchy.FindDescendantItems(["a", "b"], ["a1", "a2"]);
		hierarchy.FindDescendantItems(["a", "b"], n => n.Item.Id.EndsWith('2'));
		#endregion

		#region Search for sub-hierarchy
		// Create a hierarchy consisting of the root, "a", "a1" and "a2", effectively excluding branch "b".
		hierarchy.Search("a", SearchInclude.All);
		// Create a hierarchy consisting of the node "a" and its ancestors "r".
		hierarchy.Search("a", SearchInclude.Matches | SearchInclude.Ancestors);
		// Create a hierarchy consisting of the branch "a" as its root.
		hierarchy.Search("a", SearchInclude.Matches | SearchInclude.Descendants);
		// Create a hierarchy consisting of the branches "a" and "b" as its roots.
		hierarchy.Search(["a", "b"], SearchInclude.Matches | SearchInclude.Descendants);
		// Create a hierarchy consisting of nodes with a single letter ID; "r", "a", "b".
		hierarchy.Search(n => n.Item.Id.Length == 1, SearchInclude.Matches);
		#endregion


		#region Node retrieval
		var descendants = branchNodes.GetDescendants(includeSelf: true, type: TraversalType.DepthFirst);
		var ancestors   = branchNodes.GetAncestors(includeSelf: true);
		#endregion


		#region Graph traversal
		// Simple traversal
		var nextNodes = Traverse.Graph(rootNode, node => node.Children);

		// Advanced traversal with signal control (prune/skip/early stop)
		var signalNodes = Traverse.Graph(rootNode, (node, signal) =>
		{
			// Don't traverse into the children of "a1"
			if (node.Item.Id != "a1")
				signal.Next(node.Children);
			// Don't yield for "a"
			if (node.Parent?.Item.Id == "a")
				signal.Skip();
			// If you reach "x", stop the traversal
			if (node.Item.Id == "x")
				signal.End();
		});
		// Optionally takes detectCycles and TraversalType parameters.
		#endregion


		#region Sequence traversal
		var list = new LinkedList<int>(Enumerable.Range(1, 4));
		var elements = Traverse.Sequence(list.First!, (el, sg) =>
		{
			// Skip odd numbers
			if (el.Value % 2 == 1)
				sg.Skip();
			// Provide next when continuing
			if (el.Next is not null)
				sg.Next(el.Next);
		});
		#endregion


		#region Managing relational data
		var modifiedIds = hierarchy.CloneIds().Attach("a", "a3");

		var diff = HierarchyChanges.Difference(hierarchy, modifiedIds, "companies", RelType.Children);
		// The diff has `Deleted`, `Inserted` and `Removed` properties containing the changes.
		// In this case that "a3" was inserted.
		#endregion
	}
}
