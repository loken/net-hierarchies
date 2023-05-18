# Loken.Hierarchies.MongoDB ![Nuget](https://img.shields.io/nuget/v/Loken.Hierarchies.MongoDB)

.NET satellite library for using MongoDB with `Loken.Hierarchies`.

The package provides expressive extension methods for a suite of operations on a collection intended to hold hierarchy relations:

1. Add indexes to the collection.
1. Create a hierarchy directly from the collection.
1. Query the collection to retrieve specific relations or targets.
1. Synchronize a hierarchy with the collection.


## Getting started

Install the package from nuget.org into your .NET 7 or later project using your package manager of choice, or the command line;

```shell
dotnet add package Loken.Hierarchies.MongoDB
```

Read the documentation for the main [`Loken.Hierarchies`](https://github.com/loken/loken-hierarchies-net/Hierarchies) package to get an understanding of how the core library functions before you continue on.


## Collection of relations

Every feature starts with the collection holding the hierarchy relations.

You can retrieve the default collection for a given type of `TId` using an extension method on the `IMongoDatabase`.

```csharp
IMongoCollection<HierarchyRelation<TId>> collection = database.GetHierarchies<TId>();
```

> **NB!** You must use different collections for different types of IDs.

Once you've got this collection, you can use various extension methods on that collection. Explore your intellisense!

You chose which `RelType`s you want and need in your collection: `Ancestors`, `Children` and/or `Descendants`. Usually you want at least Children since you need this `RelType` to create your `Hierarchy`.


## Initialization

Since the `HierarchyRelation<TId>` class has an `Id` property which is not guaranteed to be unique (multiple `Concept`s and/or `Type`s can have the same `Id`) we provide a way to register necessary conventions. Be sure to call it during bootstrapping!

```csharp
HierarchyMongo.RegisterConventions();
```

Before you start populating the hierarchy collection, you should make sure you index it so your queries will be as fast as they can be!

```csharp
collection.CreateRelationIndexes();
```

If you need to quickly seed/re-seed the collection with existing relations, you can do it like this:

```csharp
// You may want to clear existing relations.
collection.ClearRelations<TId>();
// You can insert relations from an existing hierarchy.
collection.InsertRelations(hierarchy, "my-concept");
// You can optionally pass a `RelType` as a filter to both of those to control which kinds of relations are being used.
```


## Changes

You can keep your hierarchy and your stored relations in sync in two ways.

You can keep two copies of your hierarchy in memory, one which reflects what's in the database and another which you modify. You would then generate updates based on a diff between them and update the relations collection accordingly.

The benefit of doing it this way is that you don't have to fetch all relations from the database each time you need to sync.

The downside is that you then need to make sure you know the state of the data. If you have multiple processes pushing updates that might be tricky. It might be worth it though, but it probably requires that you use some distributed caching, messaging etc.

```csharp
collection.UpdateRelations(oldHierarchy, newHierarchy, "my-concept", RelType.All);
```

The more straight forward way of doing it will fetch the child-relations from the collection and using that as the basis for the diff.

```csharp
collection.UpdateRelations(hierarchy, "my-concept", RelType.All);
```

In both of these you can decide which `RelType`s to update.


## Queries

Query the relations using extension methods:

```csharp
// Create an ID hierarchy from the child-relations.
Hierarchy<TId> idHierarchy = collection.ReadHierarchy("my-concept");

// Get relation targets for a specific concept and ID:
ISet<TId> children    = collection.GetChildren("my-concept", id);
ISet<TId> descendants = collection.GetDescendants("my-concept", id);
ISet<TId> ancestors   = collection.GetAncestors("my-concept", id);
ISet<TId> ids         = collection.GetRelationTargets("my-concept", id, type);

// Get entire relation for a specific concept:
IEnumerable<HierarchyRelation<TId>> relations = collection.GetRelations("my-concept", type);
// Get entire relation for a specific concept and ID:
HierarchyRelation<TId>? relations = collection.GetRelations("my-concept", id, type);
```


## Feedback & Contribution

Please see the [repository root](https://github.com/loken/loken-hierarchies-net#feedback--contribution).
