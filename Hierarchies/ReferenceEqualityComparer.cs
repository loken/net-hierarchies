using System.Runtime.CompilerServices;

namespace Loken.Hierarchies;

/// <summary>
/// Reference equality comparer for reference types. Uses object identity (ReferenceEquals) semantics.
/// </summary>
/// <typeparam name="T">Type to compare. Intended for reference types.</typeparam>
/// <remarks>
/// Keep internal. Exposing this publicly invites misuse with value types, where boxing would change semantics
/// and identity-based hashing is almost never what you want. Use only for reference types where instance identity
/// is the desired key (e.g., visited sets, membership by node instance).
/// </remarks>
internal sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T>
{
	public static readonly ReferenceEqualityComparer<T> Instance = new();

	private ReferenceEqualityComparer() { }

	public bool Equals(T? x, T? y) => ReferenceEquals(x, y);

	public int GetHashCode(T obj) => RuntimeHelpers.GetHashCode(obj!);
}
