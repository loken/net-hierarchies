# Loken.Hierarchies ![Nuget](https://img.shields.io/nuget/v/Loken.Hierarchies)

.NET 8 library for working with hierarchies of identifiers and identifiable items.

Traverse, search, and reason about hierarchical data with deterministic order, identity-aware traversal, and fast enumeration.


## Quick start

Install the package into your .NET 8 project:

```shell
dotnet add package Loken.Hierarchies
```

Create a hierarchy from items with a parent ID or externalized relations, traverse and search:

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
var prunedHierarchy = hierarchy1.Search("a", SearchInclude.Matches | SearchInclude.Descendants);

// Clone the entire hierarchy.
var clonedHierarchy = hierarchy1.Clone();
```

> **Tip**: Extensions live in the same namespace as the core types. Use IntelliSense on `Hierarchy<TItem, TId>` and `Node<TItem>` or see [Discoverability and API surface](#discoverability-and-api-surface).

*Next*: Jump to [Building hierarchies](#building-hierarchies).


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
- TypeScript sibling with near-identical APIs: https://www.npmjs.com/package/@loken/hierarchies


## Concepts

### Preamble: Prepare items and relations

A hierarchy can hold IDs or Items represented by IDs.

Let's prepare some items and relations to use for our examples.

When the Item contains a parent ID, we can derive relations from the items.

```csharp
record Item(string? ParentId, string Id, string Name);
Item root = new (null, "r", "root");
Item a  = new ("r", "a", "branch-A");
Item b  = new ("r", "b", "branch-B");
Item a1 = new ("a", "a1", "leaf-A1");
Item a2 = new ("a", "a2", "leaf-A2");
Item[] items = [ root, a, b, a1, a2 ];
```

When it does not, we must provide the structure through other means such as relations or a child map. Let's prepare both.

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

### Item vs ID hierarchies

- Use a hierarchy of items when the dataset is small enough to keep in memory and you want to traverse and query rich objects directly.
- Use a hierarchy of IDs when items are too large, numerous or remote; traverse IDs first, then fetch the matching entities from your data source.

### Identification delegates

Why delegates to identify items? We don't force an interface or use reflection. Passing `identify(item)` (and `identifyParent(item)`) lets you pick any key shape, including composites, hashes, or adapters over legacy models, without changing your types.

### Relations

We support several representations and conversions:

Use `Relation<TId>` for portable storage and diffs, `MultiMap<TId>` for fast in-memory graph construction, and `HierarchyRelation<TId>` to tag/query relations in databases by concept and type.

1. `Relation<TId>` holds a parent-to-child relation.
2. `MultiMap<TId>` is used for representing a child map for ID relations. You can build it directly, or use helpers like `MultiMap.Parse<TId>(text)` and `map.Render()` to serialize as text.
3. `HierarchyRelation<TId>` describes ID-to-target relations with a `Type` and `Concept`.

Use extension methods to map or convert between structure representations for IDs: nodes, relations, child maps, and text.

```csharp
// Mapping between structure representations
var relationsFromHierarchy = hierarchy1.ToRelations();
var childMapFromHierarchy  = hierarchy1.ToChildMap();
var nodesFromRelations     = relationsFromHierarchy.ToNodes();
var nodesFromChildMap      = childMapFromHierarchy.ToNodes();
// Serialize between child map and text
var textChildMap           = childMap.Render();
var parsedChildMap         = MultiMap.Parse<string>(textChildMap);
```

### Discoverability and API surface

Functionality is provided via factory classes and extension methods:

- Factories: `Nodes` and `Hierarchies` (creation from items, relations, child maps, or other hierarchies)
- Extensions: Most features are exposed as extension methods in the same namespace as the core types (`Loken.Hierarchies`). With the project's global usings, they're immediately discoverable via IntelliSense on `Hierarchy<TItem, TId>`, `Node<TItem>`, `Relation<TId>`, and `MultiMap<TId>`. Start with `Get*`/`Find*` on `Hierarchy<,>` for queries and `To*` methods for mapping/conversion.

*Next*: See [Building hierarchies](#building-hierarchies) to construct graphs, or jump to [Query and traversal](#query-and-traversal) to work with existing ones.


## Building hierarchies

There are many ways of creating a hierarchy. We can build it imperatively, use known relations encoded into the items as a parent-child relationship or use an external list of relations or a child map.

### Build imperatively

Create an empty hierarchy, then create nodes and attach them to each other or to the hierarchy directly.

```csharp
var hierarchy = Hierarchies.Create<Item, string>(i => i.Id);

// Create nodes and attach them to each other
var branchNodes = Nodes.CreateMany(a, b);
var rootNode    = Nodes.Create(root).Attach(branchNodes);

// Attach the root node as a hierarchy root. (Yes we can have multiple roots.)
hierarchy.AttachRoot(rootNode);

// Create some leaf nodes and attach them to the "a" branch of the hierarchy directly.
var leafNodes = Nodes.CreateMany(a1, a2);
hierarchy.Attach("a", leafNodes);
```

### Create from items with parent IDs

We can provide the structure implicitly by providing a mapping delegate. The delegate may provide the parent ID from a property, as shown below, or through any other means such as another data structure.

```csharp
var parentedHc = Hierarchies.CreateFromParents(items, item => item.Id, item => item.ParentId);
```
Related variants exist when your models expose other shapes:
- Children by ID list: `Hierarchies.CreateFromChildren(items, identify, getChildIds)`.
- Root items with children references: `Hierarchies.CreateFromChildren(roots, identify, getChildren)`.
- Leaf items with parent reference: `Hierarchies.CreateFromParents(items, identify, getParent)`.

### Create from relations or child map

You can create an ID-hierarchy or an item-hierarchy from relations or a child map.

```csharp
var idHierarchyFromRelations   = Hierarchies.CreateFromRelations(relations);
var itemHierarchyFromRelations = Hierarchies.CreateFromRelations(items, item => item.Id, relations);

var idHierarchyFromMap         = Hierarchies.CreateFromChildMap(childMap);
var itemHierarchyFromMap       = Hierarchies.CreateFromChildMap(items, item => item.Id, childMap);
```

### Create from hierarchy

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


## Query and traversal

Query and traverse hierarchies using the high-level `Hierarchy<TItem, TId>` API.

- `Get*` methods will throw if you pass an ID which does not exist in the hierarchy. If you don't know, use `Find*` instead!
- For methods traversing ancestors or descendants, you can provide an optional `includeSelf` flag to specify whether the provided IDs should be included in the retrieval or search.
- For methods traversing descendants you may also specify `TraversalType.DepthFirst` if you don't want the default, which is `TraversalType.BreadthFirst`.

### Get by ID

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

### Find by predicate

Lookup all nodes, items or IDs matching a predicate.

```csharp
hierarchy.Find(n => n.Item.Name == "a");
hierarchy.FindItems(n => n.Item.Name == "a");
hierarchy.FindIds(n => n.Item.Name == "a");
```

### Get descendants and ancestors

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
```
> **NB!** Similar methods exist for ancestors: GetAncestors, GetAncestorItems, GetAncestorIds

### Find descendants and ancestors

Search within descendants or ancestors of nodes matching the ID(s) of the first parameter, looking for nodes matching the ID(s) or predicate of the second parameter.

```csharp
// Find the first matching descendant of a single starting node.
hierarchy.FindDescendant("a", "a2");
hierarchy.FindDescendant("a", ["a1", "a2"]);
hierarchy.FindDescendant("a", n => n.Item.Id.EndsWith('2'));

// Find the first matching descendant of multiple starting nodes.
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
> **NB!** Similar methods exist for ancestors: FindAncestor, FindAncestors, FindAncestorItems, FindAncestorIds

### Search for sub-hierarchy

Create a new hierarchy with nodes for a subset of matching nodes.

The included nodes depend on the `SearchInclude` `Flags` enum parameter:
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

### Node APIs

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
var nextNodes = Traverse.Graph(rootNode, node => node.Children);

// Advanced traversal with signal controller (prune/skip/early stop)
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

## Performance

- For `Traverse`, prefer the simple `next` delegate and use `signal` only when you need pruning or early stop.
- Enable cycle detection only when you expect cycles; it adds hashing per node.
- Nodes are double-linked for fast up/down traversal. This trades a small memory overhead for speed; be mindful with very large graphs.
- Eager vs. lazy choices:
   - Results are often eager lists to avoid iterator overhead.
   - Many collection parameters are overloaded to reuse runtime types and avoid clones.
   - Passing one method's result into another typically avoids extra allocations.
   - Some properties like `Node.Children` are lazy and cached to keep edits and repeated access cheap.


## Semantics

### Ordering and identity

- Sibling order is deterministic. Children are kept in insertion order, and traversals enumerate children in that order. When constructing from relations or a child map, the source order is preserved.
- Cycle detection and visited semantics are identity-based. When `detectCycles` is enabled, traversal tracks visited nodes by reference. Separate `Node<T>` instances that wrap the same `Item` are considered distinct for visitation.
- Equality: `Node<T>` equality and hash code are based on the wrapped `Item`. This enables JSON round trips to compare equal by value. When you need identity semantics (e.g., visited sets, membership, caches keyed by node), use `ReferenceEqualityComparer<Node<T>>` with your `HashSet`/`Dictionary`.

### Serialization

Nodes serialize and deserialize cleanly. Equality is based on `Item`, so round trips compare equal by value. See `JsonConvert_BothWays_Works` for an end-to-end example.


## Data management and persistence

### Managing relational data

1. Store `HierarchyRelation<TId>`s to your database when you need to query your hierarchies. They contain a `Concept` and `Type` so you can keep many relations in the same table/collection.
   - `Concept`: Specify what the relations are for.
   - `Type`: Specify if the relations are for `Children`, `Descendants` or `Ancestors`. Yes, you can generate all of these from a `Hierarchy<TId>` to support advanced queries.
2. Produce a `HierarchyDiff<TId>` when you need to create batched database updates.

> **Note**: In-memory synchronization of `Hierarchy<TId>` instances using diffs is not yet supported. For now, reconstruct hierarchies from a complete set of relations or a child map. Database synchronization is available via satellite assemblies.

```csharp
var modifiedIds = hierarchy.CloneIds().Attach("a", "a3");

var diff = HierarchyChanges.Difference(hierarchy, modifiedIds, "companies", RelType.Children);
// The diff has `Deleted`, `Inserted` and `Removed` properties containing the changes.
// In this case that "a3" was inserted.
```

### Persistence

- Store structure as `HierarchyRelation<TId>`s in your database.
- Store the item models separately and hydrate an in-memory hierarchy of the items based on the associated relations.
- After in-memory edits, compute diffs with `HierarchyChanges.Differences(..)` (by concept & rel type) to get one or more `HierarchyDiff<TId>` instances and apply them as batch updates.
- Satellite packages can help automate this wiring (see MongoDB extensions for examples).
- Use `Attach`/`Detach` on nodes or the hierarchy helpers to modify an existing hierarchy or node graph.


## Branding

Nodes can be branded with ownership tokens to prevent cross-hierarchy contamination. Hierarchies automatically brand and debrand their nodes when you attach or detach a node, respectively. Since the hierarchy brand is internal, you cannot attach a node to more than one hierarchy at a time.

Branding is not serialized, but a new brand is created when you create a new hierarchy.

Manual branding is only needed if you're working with `Node`s that are not part of a `Hierarchy`. In short, it's a low-level node feature you usually don't need to know about.

> **NB!** Just don't try to add a node from one hierarchy to another without first detaching it!


## Docs and snippets

README code snippets are compile-verified in `Hierarchies.Tests/ReadmeSnippetsTests.cs`.
Section headings mirror region names in that test to keep docs and examples in sync.


## TypeScript sibling library

Prefer TypeScript on the front end or in Node.js? The sibling package `@loken/hierarchies` exposes the same core constructs and near-identical APIs:

- Shared concepts: `Hierarchies`, `Nodes`, `Relations`, `MultiMap`
- Parity: Similar method names and shapes across languages
- Differences: .NET emphasizes extension methods for discoverability; TypeScript prefers static factory methods

See the package on npm: https://www.npmjs.com/package/@loken/hierarchies


## Feedback and contribution

Please see the [repository root](https://github.com/loken/loken-hierarchies-net#feedback--contribution).