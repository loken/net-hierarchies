# Loken.Hierarchies ![Nuget](https://img.shields.io/nuget/v/Loken.Hierarchies)

.NET 8 library for working with hierarchies of identifiers and identifiable items.

Traverse, search, and reason about hierarchical data with deterministic order, identity-aware traversal, and fast enumeration.


## Quick start

Install the package into your .NET 8 project:

```shell
dotnet add package Loken.Hierarchies
```

Create from items with a parent ID or from items with externalized relations, traverse, and search:

```csharp
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
var prunedHierarchy = hierarchy1.Search(["a"], SearchInclude.Matches | SearchInclude.Descendants);

// Clone the entire hierarchy.
var clonedHierarchy = hierarchy1.Clone();
```

## Features

- Build hierarchies from items, relations, or child maps with simple factory methods
- Deterministic sibling order and stable traversal (preserves source order)
- Fast breadth- or depth first traversal with optional cycle detection
- Advanced traversal with a `signal` delegate for pruning/early stop
- Powerful search: find by predicate or produce pruned hierarchies (matches/ancestors/descendants)
- Convenient mapping: convert between relations, child maps, nodes and text
- Serialization helpers for fixtures, tests and persistence
- Node branding to prevent cross-hierarchy contamination
- Optimized for speed and low memory overhead


## Core usage

### Preamble: Prepare items and relations

A hierarchy can hold IDs or Items represented by IDs.

Let's prepare some items and relations to use for our examples.

When the Item contains a parent ID, we can derive relations from the items.

```csharp
record Item(string? ParentId, string Id, string Name);
Item[] items =
[
   new(null, "r", "root"),
   new("r", "a", "branch-A"),
   new("r", "b", "branch-B"),
   new("a", "a1", "leaf-A1"),
   new("a", "a2", "leaf-A2"),
];
```

When it does not, we must provide the structure through other means such relations or a child map. Let's prepare both.

```csharp
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
```

### Creating hierarchies

There are many ways of creating a hierarchy. We can build it imperatively, use known relations encoded into the items as a parent-child relationship or use an external list of relations or a child-map.

#### Build imperatively

Create an empty hierarchy, then create nodes and attach them to each other or to the hierarchy directly.

```csharp
var hierarchy = Hierarchies.Create<Item, string>(i => i.Id);

// Create nodes and attach them to each other
var branchNodes = Nodes.CreateMany(items[1], items[2]);
var rootNode    = Nodes.Create(items[0]).Attach(branchNodes);

// Attach the root node as a hierarchy root. (Yes we can have multiple roots.)
hierarchy.AttachRoot(rootNode);

// Create some leaf nodes and attach them to the "a" branch of the hierarchy directly.
var leafNodes = Nodes.CreateMany(items[3], items[4]);
hierarchy.Attach("a", leafNodes);
```

#### Create from items with parent IDs

We can provide the structure implicitly by providing a mapping delegate. The delegate may provide the parent ID from a property, as shown below, or though any other means such as another data structure.

```csharp
var parentedHc = Hierarchies.CreateFromParents(items, item => item.Id, item => item.ParentId);
```

#### Create from relations

You can create an ID-hierarchy from other relational structures such as the relations or child-map created in the preamble.

```csharp
   var idHierarchyFromRelations = Hierarchies.CreateFromRelations(relations);
   var idHierarchyFromMap       = Hierarchies.CreateFromChildMap(childMap);
```

#### Create matching

Create a hierarchy that matches the structure of another hierarchy but with different content.

Common scenarios:
1. **Memory optimization** - Convert an item-hierarchy to an ID-hierarchy when you only need structural reasoning
2. **Data hydration** - Build an item-hierarchy from database items matching an existing ID-hierarchy
3. **Multiple representations** - Create hierarchies for different data views of the same conceptual structure

```csharp
// Create ID-hierarchy matching an item-hierarchy's structure
var matchingIdHc = Hierarchies.CreateFromHierarchy(parentedHc);

// Create item-hierarchy matching an ID-hierarchy's structure
var matchingItemHc = Hierarchies.CreateFromHierarchy(items, item => item.Id, matchingIdHc);
```

#### Item vs ID hierarchies

- Use a hierarchy of items when the dataset is small enough to keep in memory and you want to traverse and query rich objects directly.
- Use a hierarchy of IDs when items are too large or remote; traverse IDs first, then fetch the matching entities from your data source.

### Search

Search and query hierarchies using the high-level `Hierarchy<TItem, TId>` API.

- `Get*` methods will throw if you pass an ID which does not exist in the hierarchy. If you don't know, use `Find*` instead!
- For methods traversing ancestors or descendants, you can provide an optional `includeSelf` flag to specify whether the provided IDs should be included in the retrieval or search.
- For methods traversing descendants you may also specify if the `TraversalType` should be breadth-first or depth-first. 

#### Get by ID

Check existence and retrieve specific nodes or items by their IDs.

```csharp
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
```

#### Find by predicate

Lookup all nodes, items or IDs matching a predicate.

```csharp
hierarchy.Find(n => n.Item.Name == "a");
hierarchy.FindItems(n => n.Item.Name == "a");
hierarchy.FindIds(n => n.Item.Name == "a");
```

#### Get descendants and ancestors

Retrieve all descendants or ancestors of one or more IDs.

```csharp
// Get descendants/ancestors by ID
hierarchy.GetDescendants("a");
hierarchy.GetDescendantItems("a");
hierarchy.GetDescendantIds("a");
// Get descendants/ancestors by IDs
hierarchy.GetDescendants(["a", "b"]);
hierarchy.GetDescendantItems(["a", "b"]);
hierarchy.GetDescendantIds(["a", "b"]);
// Similar methods exist for ancestors: GetAncestors, GetAncestorItems, GetAncestorIds
```

#### Find descendants and ancestors

Search within descendants or ancestors of nodes matching the ID(s) of the first parameter, looking for nodes matching the ID(s) or predicate of the second parameter.

```csharp
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
```
> **NB!** Similar methods exist for ancestors: FindAncestor, FindAncestors, etc.

#### Search for sub-hierarchy

Create a new hierarchy with nodes for a subset of matching nodes.

The included nodes depends on the `SearchInclude` `Flags` enum parameter:
- `Matches`: Include the match itself
- `Ancestors`: Include all ancestors of a match.
- `Descendants`: Include all descendants of a match.
- `All`: Include `Matches`, `Ancestors` and `Descendants`.

The result is effectively a pruned clone of the original hierarchy.

```csharp
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
```

### Node and traversal APIs

When you need fine-grained control or are working directly with nodes, use these lower-level APIs.

#### Node retrieval

```csharp
var descendants = branchNodes.GetDescendants(includeSelf: true, type: TraversalType.DepthFirst);
var ancestors   = branchNodes.GetAncestors(includeSelf: true);
```

#### Graph traversal

For advanced scenarios where you need custom traversal logic with pruning or early termination.

```csharp
// Simple traversal
var nextNodes = Traverse.Graph(root, node => node.Children);

// Advanced traversal with signal controller (prune/skip/early stop)
var signalNodes = Traverse.Graph(
   rootNode, (node, signal) =>
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
   },
   detectCycles: false,
   type: TraversalType.DepthFirst);
```

#### Sequence traversal

For traversing non-hierarchy sequences with similar control patterns.

```csharp
var list = new LinkedList<int>(Enumerable.Range(1, 4));
var elements = Traverse.Sequence(list.First!, (el, signal) =>
{
   // Skip odd numbers
   if (el.Value % 2 == 1)
      signal.Skip();
   // Provide next when continuing
   if (el.Next is not null)
      signal.Next(el.Next);
});
```


## Performance tips

- Prefer the simple `next` delegate for enumeration; use `signal` only when you need pruning or early stop.
- Only enable cycle detection if you expect cycles; it adds hashing per node.


## Advanced concepts

### Relations

We support several representations and conversions:

1. `Relation<TId>` holds a parent-to-child relation, with implicit casts to tuple in some APIs.
2. `MultiMap<TId>` is used for representing a child map for ID relations. You can build it directly, or use helpers like `MultiMap.Parse<TId>(text)` and `map.Render()` to serialize as text.
3. `HierarchyRelation<TId>` describes ID-to-targets relations with a `Type` and `Concept`.

#### Managing relational data

1. Store `HierarchyRelation<TId>`s to your database when you need to query your hierarchies. They contain a `Concept` and `Type` so you can keep many relations in the same table/collection.
   - `Concept`: Specify what the relations are for.
   - `Type`: Specify if the relations are for `Children`, `Descendants` or `Ancestors`. Yes, you can generate all of these from a `Hierarchy<TId>` to support advanced queries.
2. Produce a `HierarchyDiff<TId>` when you need to create batched database updates.
- *Note:* In-memory synchronization of `Hierarchy<TId>` instances using diffs is not yet supported. For now, reconstruct hierarchies from a complete set of relations or a child map. Database synchronization is available via satellite assemblies.

```csharp
var modifiedIds = hierarchy.CloneIds().Attach("a", "a3");

var diff = HierarchyChanges.Difference(hierarchy, modifiedIds, "companies", RelType.Children);
// The diff has `Deleted`, `Inserted` and `Removed` properties containing the changes.
// In this case that "a3" was inserted.
```

### Identification delegates

Why delegates to identify items? We don't force an interface or reflection. Passing `identify(item)` (and `identifyParent(item)`) lets you pick any key shape, including composites, hashes, or adapters over legacy models, without changing your types.

### Performance & Memory

Nodes are double-linked for fast up/down traversal. This trades a small memory overhead for speed; be mindful with very large graphs.

The library is mindful about when to use eager vs. lazy flattening:
- Results are often eager lists, avoiding the overhead of enumerators
- Collection parameters are often overloaded with internals trying to reuse the runtime type instead of creating a clone
- The result is that passing the result of one method to another will not create extra clones.
- Some properties like `Node.Children`, on the other hand, are lazily evaluated to minimize churn while modifying a hierarchy.

### Persistence

- Store relations (as `Relation<TId>` or `MultiMap<TId>`) in your database or a fixture.
- After in-memory edits, compute a diff with `HierarchyChanges.Differences(..)` (by concept/rel type) to get a `HierarchyDiff<TId>` and apply it as batch updates.
- Satellite packages can help automate this wiring (see MongoDB extensions for examples).
- Use `Attach`/`Detach` on nodes or the hierarchy helpers to modify an existing graph.

### Branding

Nodes can be branded to prevent cross-hierarchy contamination. Branding is not serialized; reapply brands after deserialization if needed.

### Mapping

There are extension methods for mapping between various structure representations for IDs; nodes, relations, child maps and text.

Small mapping example:

```csharp
// Mapping between structure representations
var relationsFromHierarchy = hierarchy.ToRelations();
var childMapFromHierarchy  = hierarchy.ToChildMap();
var nodesFromRelations     = relationsFromHierarchy.ToNodes();
var nodesFromChildMap      = childMapFromHierarchy.ToNodes();
```

## Semantics

### Ordering and identity

- Sibling order is deterministic. Children are kept in insertion order, and traversals enumerate children in that order. When constructing from relations or a child map, the source order is preserved.
- Cycle detection and visited semantics are identity-based. When `detectCycles` is enabled, traversal tracks visited nodes by reference. Separate `Node<T>` instances that wrap the same `Item` are considered distinct for visitation.
- Equality: `Node<T>` equality and hash code are based on the wrapped `Item`. This enables JSON round trips to compare equal by value. When you need identity semantics (e.g., visited sets, membership, caches keyed by node), use `ReferenceEqualityComparer<Node<T>>` with your `HashSet`/`Dictionary`.

### Serialization

Nodes can be serialized and deserialized directly. Equality based on `Item` makes round trips value-equal. For example, a simple `Node<string>` renders as:

```json
{
   "Children": [
      {
         "Children": null,
         "Item": "a"
      }
   ],
   "Item": "root"
}
```

See the test `JsonConvert_BothWays_Works` for an end-to-end example.

## Docs and snippets

README code snippets are compile-verified in `Hierarchies.Tests/ReadmeSnippetsTests.cs`.
Section headings mirror region names in that test to keep docs and examples in sync.

## Feedback and contribution

Please see the [repository root](https://github.com/loken/loken-hierarchies-net#feedback--contribution).