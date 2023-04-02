# Loken.Hierarchies

.NET library for working with hierarchies of identifiers and identifiable objects.

The idea is that quite often you have a list of objects, usually from a database. The entries are related by having a primary key and a foreign key to their parent.

With Loken.Hierarchies you can build up an in-memory Hierarchy of the objects based on their keys and foreign keys. Once you have the Hierarchy, you can traverse it, search it and reason about the parents, children, ancestors and descendants of a Node.

Sometimes you don't want to, or cannot store the parent relationship on the object itself, but rather keep the relationships between the objects stored in a separate location. Loken.Hierarchies supports this as well.

Finally, you may not want to retrieve the full set of objects from the database just to know what the keys of the ancestry of a given id is. Since Loken.Hierarchies supports creating a Hierarchy of keys it lends itself well to support such a scenario. Very useful when you want to build up a query and retrieve only the objects that are in the ancestry.
