# Copilot Instructions for net-hierarchies

## Project Overview
- .NET library for working with hierarchies of identifiers and identifiable objects, typically representing tree structures from databases or in-memory collections.
- Core abstractions: `Hierarchy<TItem, TId>`, `Node<TItem>`, `Relation<TId>`, and `MultiMap<TId>` (from dependencies).
- Hierarchies can be constructed from relations, child maps, or item lists using flexible options and delegates.
- The codebase is modular, with key logic in `Hierarchies/`, `Hierarchies.MongoDB/` (satellite package), and `Traversal/`.

## Key Patterns & Conventions
- **Hierarchy Construction:**
  - Use `Hierarchy.CreateParented()` or `Hierarchy.CreateRelational()` for building hierarchies from items or relations.
  - Relations are typically arrays of `Relation<TId>` records (`Parent`, `Child`) or a `MultiMap<TId>`.
  - Item identification uses `identify(item)` and `identifyParent(item)` delegates.
- **Node Wrapping:**
  - Items are wrapped in `Node<TItem>` for graph traversal and linking.
  - Nodes track parent/child relationships and support `IsRoot`/`IsLeaf`/`IsInternal` checks.
- **Extension Methods Pattern:**
  - Functionality is discoverable through extension methods on `Node<TItem>` and `Hierarchy<TItem, TId>`.
  - Key extension classes: `NodeLinkingExtensions`, `HierarchyLinkingExtensions`, `FindAncestorExtensions`, `FindDescendantExtensions`.
- **Generic Constraints:**
  - All types use `where TItem : notnull` and `where TId : notnull` constraints for null safety.

## Developer Workflows
- **Build:** Use `dotnet build` or Visual Studio. Solution file: `Hierarchies.sln`.
- **Test:** Run `dotnet test` for unit tests using xUnit. Test projects follow `*.Tests` naming pattern.
- **Debug:** Use Visual Studio or VS Code with C# extension for step-through debugging.
- **Packages:** Main package `Loken.Hierarchies`, satellite package `Loken.Hierarchies.MongoDB`.

## Formatting & Style Conventions
- **EditorConfig:** Uses comprehensive `.editorconfig` with specific C# formatting rules.
- **Indentation:** Use tabs, not spaces.
- **Braces:** Allman style - opening braces on new lines (`csharp_new_line_before_open_brace = all`). Braces are optional when the body has a single statement.
 - **Blank lines after blocks:** Prefer a newline after a block before the next statement (enforced via `dotnet_style_allow_statement_immediately_after_block_experimental = false`).
 - **Punctuation:** Use straight ASCII apostrophes (') and hyphens (-). Do not use typographic/curly quotes or em/en dashes in code or docs. Apply this to all generated content and examples.
- **Namespaces:** File-scoped namespaces preferred (`csharp_style_namespace_declarations = file_scoped`).
- **var usage:** Prefer `var` when type is apparent (`csharp_style_var_when_type_is_apparent = true`).
- **Collection expressions:** Prefer C# 12 collection expressions for arrays/lists when readable, e.g. `Item[] items = [ new("1", null, "root"), ... ];`.
- **Target-typed new + explicit types:** Prefer explicit variable types when using target-typed `new(...)` to enable terse initialization and clearer intent, e.g. `Relation<string>[] relations = [ new("1","2") ];`. This is a scoped exception to the general "prefer var when type is apparent" guideline.
- **Expression bodies:** Use for properties, accessors, lambdas; avoid for methods and constructors.
- **Naming:** PascalCase for types and public members, interfaces prefixed with `I`.
 - **Single-line statements:** Do not place embedded statements on the same line as the keyword (enforced via `csharp_style_allow_embedded_statements_on_same_line_experimental = false`).
 - **Raw string literals:** Use C# 11 triple-quoted raw string literals (`"""..."""`) for multi-line fixtures and test data (e.g., rendering/parsing `MultiMap` text). Indentation rules:
   - Start content on a new line after the opening delimiter.
   - Indent all content lines equally using tabs (project standard).
   - Indent the closing delimiter by the same amount as the content lines. The compiler trims that common indentation from each line, producing a left-aligned runtime string.
   - Avoid unintended leading/trailing blank lines in the raw string payload.
   - Prefer raw strings over escaped literals for readability.

## Integration Points
- Depends on `Loken.Extensions.System` and `Loken.Utilities` for core data structures (`MultiMap`, etc.).
- MongoDB satellite package provides expressive extension methods for hierarchy operations on collections.
- Designed for compatibility with other Loken hierarchy libraries (see TypeScript sibling repo for cross-language patterns).

## TypeScript vs .NET Implementation Differences
- **Discoverability Pattern:** While the TypeScript sibling repo uses static methods on `Nodes` and `Hierarchies` classes, this .NET implementation uses extension methods for discoverability.
- **API Surface:** .NET version uses traditional OOP with extension methods, while TypeScript emphasizes functional patterns with static factory methods.
- **Entries:** TS version has some method variants with a *Entries suffix (e.g., `findEntries`, `findDescendantEntries`). In TS that makes sense since various prototypes have a .entries() method which is similar. In .NET this is not idiomatic, so we don't include similar methods in .NET.
- **Singular Find Methods:** TS has singular find methods like `findAncestorItem()`, `findAncestorId()`, `findDescendantItem()`, `findDescendantId()` that return `Item | undefined` or `Id | undefined`. .NET intentionally omits these because the `notnull` constraint on `TId` prevents returning `TId?`, forcing `default(TId)` which creates ambiguity (e.g., `0` for `int` could be a valid ID or "not found"). The plural variants (`FindAncestorItems()`, etc.) returning collections avoid this ambiguity and provide clearer semantics.
- **Conversion Methods:** .NET Provides highly optimized `nodes.ToItems()` and `nodes.ToIds()` methods for converting from various collection types to items and IDs. TS also has some methods for this, but in TS we prefer using a `.map(n => n.item)` statement, while in NET it's important for performance to prefer our extension methods over LINQ.

## Project-Specific Advice
- **Node Branding:** Nodes can be "branded" with ownership tokens. Branded nodes can only be attached to nodes with compatible brands. Hierarchies automatically brand their nodes to prevent cross-hierarchy contamination. The `Brand()` method returns an `Action` delegate for cleanup.
- **Extension Methods:** When adding new functionality, prefer extension methods to maintain discoverability. Group related extensions in dedicated static classes (e.g., `*Extensions.cs`).
- **Global Usings:** Common namespaces are imported globally via `Directory.Build.props` - do not add these `using` lines in files.
  - Always available in all projects:
    - `using Loken.Hierarchies;`
    - `using Loken.Hierarchies.Data;`
    - `using Loken.Hierarchies.Traversal;`
    - `using Loken.System;`
    - `using Loken.System.Collections;`
    - `using Loken.System.ComponentModel;`
    - `using Loken.Utilities;`
    - `using Loken.Utilities.Collections;`
    - `using Loken.Utilities.ComponentModel;`
    - `using Loken.Utilities.IO;`
  - Additionally available in test projects (`*.Tests`):
    - `using Xunit;`
- **Test Namespaces:**
  - Use the same file-scoped namespace as the project under test. Derive it by removing the `.Tests` suffix from the project name.
  - Example: `Something` ⇒ `namespace Loken.Something;` and `Something.Tests` ⇒ `namespace Loken.Something;`.
  - Don't invent additional `.Tests` namespaces in files; `Directory.Build.props` already exposes internals to `*.Tests` assemblies.
- **Avoid redundant usings in tests:** `Directory.Build.props` provides global usings for xUnit and common Loken namespaces. Don't add `using` lines for these; qualify rare framework types with `global::System.*` only when alias conflicts occur. For fixtures or integrations, explicit framework usings (e.g., `Microsoft.Extensions.*`) are okay when needed.
- **Running tests:** Prefer targeting the `Hierarchies.Tests` project directly when you need to exclude MongoDB integration tests, e.g. when no MongoDB instance is available.
- **Satellite Packages:** MongoDB integration is in separate `Hierarchies.MongoDB` package with its own extension methods.
- Tests should use realistic relation and item structures with xUnit patterns.
- Test primarily on the `HCNode` level and don't create duplicate tests for methods which essentially are convenience wrappers, such as overloads or `Hierarchy<,>` variants.

## Key Files
- `Hierarchies/Hierarchy.cs`, `Hierarchies/HierarchyOfIds.cs`, `Hierarchies/HierarchyOfItems.cs`: Core hierarchy logic
- `Hierarchies/Node.cs`: Node wrapper and graph logic
- `Hierarchies/Nodes.cs`: Factory methods for node creation
- `Hierarchies/*Extensions.cs`: Extension methods for various operations (linking, traversal, search)
- `Hierarchies.MongoDB/`: Satellite package for MongoDB integration
- `Directory.Build.props`: Global project settings and using directives
