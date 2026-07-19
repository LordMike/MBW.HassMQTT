#nullable enable

using System;

namespace MBW.HassMQTT.DiscoveryModels;

/// <summary>
/// Represents a value that may be omitted, explicitly set to <see langword="null" />, or set to a non-null value.
/// </summary>
/// <typeparam name="T">The type of the contained value.</typeparam>
public readonly record struct Optional<T>
{
    private readonly T _value;

    private Optional(T value)
    {
        _value = value;
        IsSet = true;
    }

    /// <summary>Gets whether a value was explicitly supplied.</summary>
    public bool IsSet { get; }

    /// <summary>Gets the supplied value.</summary>
    /// <exception cref="InvalidOperationException">No value was supplied.</exception>
    public T Value => IsSet
        ? _value
        : throw new InvalidOperationException("The optional value is not set.");

    /// <summary>Gets an optional value that was not supplied.</summary>
    public static Optional<T> Unset => default;

    /// <summary>Creates an optional containing the supplied value, including an explicit <see langword="null" />.</summary>
    public static Optional<T> FromValue(T value) => new Optional<T>(value);

    /// <summary>Attempts to get the supplied value.</summary>
    public bool TryGetValue(out T value)
    {
        value = _value;
        return IsSet;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return IsSet ? _value?.ToString() ?? "null" : "Unset";
    }

    /// <summary>Creates a set optional from a value, including an explicit <see langword="null" />.</summary>
    public static implicit operator Optional<T>(T value) => FromValue(value);
}
