# Loken.Hierarchies ![Nuget](https://img.shields.io/nuget/v/Loken.Hierarchies)

.NET library for working with hierarchies of identifiers and identifiable items.

The idea is that quite often you have a list of items, usually from a database that form a tree hierarchy and you want to traverse it, search it and reason about the parents, children, ancestors and descendants. `Loken.Hierarchies` is built to solve that.


## Getting started

Install the package from nuget.org into your .NET 7 or later project using your package manager of choice, or the command line;

```shell
dotnet add package Loken.Hierarchies
```

You may want to also use one of the `Loken.Hierarchies.*` satellite packages for a particular database. Look for them in your package manager or see a list at the [repository root](https://github.com/loken/loken-hierarchies-net).


## Hierarchy of Items

Sometimes the items are small, your list short and you can hold all of the items in memory at the same time. In such cases you can create a `Hierarchy<TItem, TId>`, a hierarchy of items, by passing the items along with `identify(item)` and `identifyParent(item)` delegates.

```csharp
var hierarchy = Hierarchy.CreateParented(item => item.Id, item => item.ParentId, items);
```

If you already have a hierarchy of IDs, you can use that to create a hierarchy with matching relations. You must still provide the `identify(item)` delegate so we know which entity goes where.

```csharp
var hierarchy = Hierarchy.CreateMatching(item => item.Id, items);
```


## Hierarchy of IDs

Other times holding all items in memory is not feasible, but you still need to reason around the hierarchical relations so that you can retrieve a parent, ancestors, children or descendants of an entity.

In such cases you can build a `Hierarchy<TId>`, a hierarchy of IDs, based on known relations and then traverse that hierarchy of IDs to find the IDs of a parent, ancestors, children or descendants of an ID. Those IDs can then be used to retrieve the items from a database.

> "How do you know know what the relationships are?"

If you can't or don't want to store your relations separate from your entities, chances are your entities have a foreign key to the ID of its parent. In such cases, you query the entities for a projection of `Relation<TId>` instances, which contains only the ID of a parent and child and create the hierarchy of IDs from that.


```csharp
var idHierarchy = Hierarchy.CreateRelational(relations)
```

If you have a one-to-many parent-to-children relationship table to work with you can retrieve `HierarchyRelation<TId>` instances from that table and create the hierarchy of IDs from that.

```csharp
var idHierarchy = Hierarchy.CreateMapped(relations.ToMap());
```

> NB! Some of the `Loken.Hierarchies.*` satellite packages will help you create and maintain appropriate relationship tables.


## Identity delegates?

> "Why the delegates to identify items?"

We didn't want to enforce applying a particular interface or use reflection to access the primary key of an item. By instead using delegates you can use whatever you like to identify an item.

This opens up scenarios such as not having a database at all, and using the hash code or an index in a global list or some other madness. You can have a primary key that consists of multiple fields, by using a delegate you can combine those two fields into a tuple and use that as an identifier.

## Concepts

Let's describe the conceptually different things included in the package.

### Data structures

Above we've discussed how to create the main data structures: `Hierarchy<TItem, TId>` and `Hierarchy<TId>`, hierarchy of items and hierarchy of IDs, respectively.

A hierarchy contains a tree of `Node<T>`'s. A node is double-linked wrapper for a `T`. The double-link allow us to easily and efficiently traverse both up through the ancestry and down through descendants following links.

The hierarchy also holds a dictionary of the nodes and roots so that it provides O(1) time complexity for looking them up by ID.

#### Memory
Of course, double-linking comes at the cost of memory. We assume that you don't have a huge amount of nodes, and as such we've not optimized for minimal memory consumption. So if you do have a *lot* of nodes, you should check that we don't consume more memory than you can afford. (We've tried to be efficient in not realizing `IEnumerable<T>`s to collections as much as possible, though, so don't think we didn't consider memory at all!)

#### Branding
A node can be "branded" so that it cannot be attached to another node with a different brand. A node held by a hierarchy is automatically branded. This means that unless you want to intentionally break things by using reflection to break the branding protection, each node in a hierarchy can only belong to that hierarchy. Because of this we expose the nodes in the hierarchy.

If you want to use this API yourself, know that when you brand a node, you get a de-branding delegate back. Calling this is the only way to de-brand the node, so make sure you keep track of it!

### Relations

We support quite a few representations for relations and provide extension methods for converting between them.

1. `Relation<TId>` holds a parent-to-child relation and can be implicitly cast to a (parent TId, child TId) tuple used by some extension methods.
2. `HierarchyRelation<TId>` holds an ID-to-targets relation where the meaning of the Targets can be children, descendants or ancestors determined by the `Type` field. It also has a field for the `Concept` of a relation. The idea is that each hierarchy describes a concept. This essentially acts as an identifier for the hierarchy as a whole. Often the concept will match the entity type. But sometimes you will want multiple independent hierarchies of the same type in which case you should chose separate `Concept`s for each hierarchy.
3. `IDictionary<TId, ISet<TID>` is another representation of a `HierarchyRelation<TId>`. In `Loken.Extensions.System` this is called a "MultiMap", and there are some convenient helpers in there for parsing a string into a MultiMap or rendering a MultiMap into a string. This means you could store your relations in a file using these extensions. We don't necessarily suggest that you do this, but it's an option for some quick prototyping etc.

### Traversal

An essential part of any tree/hierarchy/graph library is traversal. The `Loken.Hierarchies.Traversal` namespace provides a static `Traverse` class which exposes methods for traversal with multiple overloads for simple or complex traversal of a single or multiple nodes to use as roots. It has a separate, but similar, set of methods for traversing a graph or a sequence.

#### Traverse next
For simple node enumeration you simply pass the starting point and a delegate describing where to find the next nodes or node.

Here is an example of what simple traversal might look like for both Graph and Sequence:

```csharp
var nodes = Traverse.Graph(root, node => node.Children);
```
```csharp
var nodes = Traverse.Sequence(first, current => current.Next);
```

#### Traverse with signal
If you want to stop traversal at some condition like a certain depth or the contents of a node or skip some nodes from the output you can use the other delegate which takes a `signal` property. You call members on the `signal` to signal what to do. In the case of the graph traversal the `signal` exposes the current `Depth` as a property.

Here is an example of what complex traversal using signal might look like for both Graph and Sequence:

```csharp
var nodes = Traverse.Graph(root, (node, signal) =>
{
   // Exclude children of the node with item 3.
   if (node.Item != 3)
      signal.Next(node.Children);

   // Skip children of of the node with item 1.
   if (node.Parent?.Item == 1)
      signal.Skip();
}, false);
```
```csharp
var list = new LinkedList<int>(Enumerable.Range(1, 4));
var elements = Traverse.Sequence(list.First!, (el, signal) =>
{
   // Skip odd numbers.
   if (el.Value % 2 == 1)
      signal.Skip();

   // By not providing the child as a next value at el 3
   // we don't iterate into el 4 which would otherwise not be skipped.
   if (el.Value < 3)
      signal.Next(el.Next);
});
```

#### Options
By default traversal type is `TraversalType.BreadthFirst`. But you can specify `TraversalType.DepthFirst` though the optional `type` argument.

It is assumed that the graph is a tree, but if there can be cycles in your nodes, you can enable cycle detection through the optional `detectCycles` property (`false` by default).

#### Traverse a `Hierarchy` or `Node`
We provide some extensions for traversal of a `Hierarchy` or one or more `Node`s as a slightly higher abstraction than the static `Traverse` class.

These don't give you the option of breaking cycles, but they do give you the option of deciding whether to includeSelf, meaning include the node, nodes, id or ids you're starting at.

These are the signatures for the `NodeTraversalExtensions`:

```csharp
IEnumerable<Node<TItem>> nodes =  node.GetDescendants(bool includeSelf, TraversalType type = TraversalType.BreadthFirst);
IEnumerable<Node<TItem>> nodes = nodes.GetDescendants(bool includeSelf, TraversalType type = TraversalType.BreadthFirst);

IEnumerable<Node<TItem>> nodes =  node.GetAncestors(bool includeSelf);
IEnumerable<Node<TItem>> nodes = nodes.GetAncestors(bool includeSelf);
```

These are the signatures for the `HierarchyTraversalExtensions`:

```csharp
IEnumerable<Node<TItem>> nodes = hierarchy.GetDescendantNodes(TId id, bool includeSelf, TraversalType type = TraversalType.BreadthFirst);
IEnumerable<TItem> items       = hierarchy.GetDescendants(    TId id, bool includeSelf, TraversalType type = TraversalType.BreadthFirst);
IEnumerable<TId> ids           = hierarchy.GetDescendantIds(  TId id, bool includeSelf, TraversalType type = TraversalType.BreadthFirst);

IEnumerable<Node<TItem>> nodes = hierarchy.GetAncestorNodes(TId id, bool includeSelf);
IEnumerable<TItem> items       = hierarchy.GetAncestors(    TId id, bool includeSelf);
IEnumerable<TId> ids           = hierarchy.GetAncestorIds(  TId id, bool includeSelf);
```

### Search

You can combine the `HierarchyTraversalExtensions`, mentioned above, in combination with LINQ to search for something.

If you need to know the depth of a node during your search, you may want to use the `NodeFindExtensions` which provide some `Find` methods for starting the search at one or more root nodes and searching through their descendants using a `(Node<TItem> node, int depth) => bool` matching function.

These methods return an `IEnumerable<Node<TItem>`, so you can use LINQ to control your search further here as well.

### Changes

You will probably want to move nodes from one parent to another. We use the method names `Attach` and `Detach` for this. You detach one node from another and then attach it to another.

If you store your relations in the database separate from a parent property on the entity, you will want to update those relations once you've performed your changes to your `Hierarchy`.

We provide a few static `HierarchyChanges.Differences(..)` methods to help with this. You pass the old and new variant of the hierarchy in one representation or another along with a concept and `RelType`, and out you get a `HierarchyDiff<TId>`. This is written so that it should be as easy as possible to turn this diff into a batch operation of updates.

Your `Loken.Hierarchies.*` satellite package may provide you with further extensions which does that for you, or you will have to do that yourself if it doesn't or there is no such satellite package for your database. In that case you can have a look at how this is done in the `HierarchyMongoDiffExtensions` in the `Loken.Hierarchies.MongoDB` package to gain some inspiration.

This is the tool you need for keeping your in-memory hierarchy in sync with your database relations.

### Mapping

There are quite a few extension methods for mapping between relations, mapping from items to nodes, mapping items to IDs etc. Please explore the tests!


## Feedback & Contribution

Please see the [repository root](https://github.com/loken/loken-hierarchies-net#feedback--contribution).