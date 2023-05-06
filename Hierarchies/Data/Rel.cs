using System.Runtime.CompilerServices;

namespace Loken.Hierarchies.Data;

/// <summary>
/// Helper class for working with the <see cref="RelType"/> flags.
/// </summary>
public static class Rel
{
	/// <summary>
	/// Yields each of the "specific" relation types.
	/// </summary>
	public static IEnumerable<RelType> Specific
	{
		get
		{
			yield return RelType.Children;
			yield return RelType.Descendants;
			yield return RelType.Ancestors;
		}
	}

	/// <summary>
	/// Get "specific" relation types from the <paramref name="type"/>.
	/// </summary>
	public static IEnumerable<RelType> GetSpecific(RelType type)
	{
		foreach (var specific in Specific)
		{
			if (type.HasFlag(specific))
				yield return specific;
		}
	}

	/// <summary>
	/// Is the <paramref name="type"/> a "specific" relationship type?
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsSpecific(RelType type)
	{
		return Specific.Contains(type);
	}

	/// <summary>
	/// Does the <paramref name="type"/> consist of at least one "specific" relationship type?
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool HasSpecific(RelType type)
	{
		return GetSpecific(type).Any(IsSpecific);
	}

	/// <summary>
	/// Assert that the <paramref name="type"/> is one of the "specific" relationship types.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">When it is not one of the "specific" relationship types.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AssertIsSpecific(RelType type)
	{
		if (!IsSpecific(type))
			throw NotSpecificException(type);
	}

	/// <summary>
	/// Assert that the <paramref name="type"/> has at least one of the "specific" relationship types.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">When it does not have at least one of the "specific" relationship types.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AssertHasSpecific(RelType type)
	{
		if (!HasSpecific(type))
			throw NoSpecificException(type);
	}

	/// <summary>
	/// Create an <see cref="ArgumentOutOfRangeException"/> for the <paramref name="type"/>
	/// which should not be a "specific" relationship type.
	/// It is assumed that the caller checks the <paramref name="type"/>.
	/// The exception is created, but not thrown. The caller should probably throw it!
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Exception NotSpecificException(RelType type)
	{
		return new ArgumentOutOfRangeException(nameof(type), type, $"Valid options: {nameof(RelType.Children)}, {nameof(RelType.Descendants)} or {nameof(RelType.Ancestors)}.");
	}

	/// <summary>
	/// Create an <see cref="ArgumentOutOfRangeException"/> for the <paramref name="type"/>
	/// which should not contain any "specific" relationship types.
	/// It is assumed that the caller checks the <paramref name="type"/>.
	/// The exception is created, but not thrown. The caller should probably throw it!
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Exception NoSpecificException(RelType type)
	{
		return new ArgumentOutOfRangeException(nameof(type), type, $"Valid options: {nameof(RelType.Children)}, {nameof(RelType.Descendants)}, {nameof(RelType.Ancestors)} or any combination thereof.");
	}
}
