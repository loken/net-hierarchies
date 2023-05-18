# Loken.Hierarchies

.NET packages for working with hierarchies of identifiers and identifiable objects.

The idea is that quite often you have a list of items, usually from a database that form a tree hierarchy and you want to traverse it, search it and reason about the parents, children, ancestors and descendants. `Loken.Hierarchies` is built to solve that.

## Packages

![Nuget](https://img.shields.io/nuget/v/Loken.Hierarchies)

Please take a look at the main package documentation:

- [`Loken.Hierarchies`](https://github.com/loken/loken-hierarchies-net/Hierarchies)

If you're interested in a satellite package for a particular database, please have a look at its documentation. It won't repeat information described in the main package documentation, so it's advised that you start there.

Documentation for supported satellite packages:

- [`Loken.Hierarchies.MongoDB`](https://github.com/loken/loken-hierarchies-net/Hierarchies.MongoDB)


## Feedback & Contribution

If you like what you see so far or would like to suggest changes to improve or extend what the library does, please don't hesitate to leave a comment in an issue or even a PR.

You can run the tests by cloning the repo, restoring packages, compiling and running the tests. There is no magic. There is a visual studio solution if you also like that.
