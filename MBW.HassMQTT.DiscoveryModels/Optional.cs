#nullable enable

using System;

namespace MBW.HassMQTT.DiscoveryModels;

/// <summary>
/// Represents whether a value was supplied separately from the value itself, allowing discovery properties to
/// distinguish omission, an explicit <see langword="null" />, and a non-null value.
/// </summary>
/// <typeparam name="T">The type of the contained value.</typeparam>
/// <remarks>
/// <para>
/// The important distinction is <em>presence</em>, not merely nullability. The supported discovery serializers omit
/// an unset <see cref="Optional{T}" />, but serialize a set optional using its contained value, including JSON
/// <see langword="null" />.
/// </para>
/// <list type="bullet">
/// <item><description>
/// <c>default(Optional&lt;T&gt;)</c>, <c>new Optional&lt;T&gt;()</c>, and <see cref="Unset" /> all mean that no value was
/// supplied: <see cref="IsSet" /> is <see langword="false" />, and the owning discovery property is omitted.
/// </description></item>
/// <item><description>
/// <c>Optional&lt;string?&gt; value = null</c> uses the implicit conversion and means that a value was supplied explicitly:
/// <see cref="IsSet" /> is <see langword="true" />, <see cref="Value" /> is <see langword="null" />, and JSON contains
/// the property with a <see langword="null" /> value. Assigning <see langword="null" /> does not mean unset.
/// </description></item>
/// <item><description>
/// <c>Optional&lt;string?&gt; value = "text"</c> means that <c>"text"</c> was supplied and is serialized as that value.
/// </description></item>
/// <item><description>
/// <c>Optional&lt;int&gt;.FromValue(0)</c> is set and contains zero. A contained value equal to <c>default(T)</c> is still
/// different from <c>default(Optional&lt;T&gt;)</c>.
/// </description></item>
/// </list>
/// <para>
/// Use <see cref="Unset" /> or <see langword="default" /> to return a discovery property to the omitted state. Use a
/// nullable contained type, such as <c>Optional&lt;string?&gt;</c> or <c>Optional&lt;SomeEnum?&gt;</c>, when explicit null is a
/// valid value. Do not add another nullable layer such as <c>Optional&lt;T&gt;?</c>; this type already represents absence.
/// </para>
/// </remarks>
public readonly record struct Optional<T>
{
    private readonly T _value;

    private Optional(T value)
    {
        _value = value;
        IsSet = true;
    }

    /// <summary>
    /// Gets whether a value was supplied. This is <see langword="true" /> for both non-null values and an explicit
    /// <see langword="null" />; it is <see langword="false" /> only for an unset optional.
    /// </summary>
    public bool IsSet { get; }

    /// <summary>
    /// Gets the supplied value. This may return <see langword="null" /> when null was explicitly supplied.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// No value was supplied; check <see cref="IsSet" /> or use <see cref="TryGetValue" /> first.
    /// </exception>
    public T Value => IsSet
        ? _value
        : throw new InvalidOperationException("The optional value is not set.");

    /// <summary>
    /// Gets an optional for which no value was supplied. Assigning this value to a discovery property causes that
    /// property to be omitted by the supported serializers.
    /// </summary>
    public static Optional<T> Unset => default;

    /// <summary>
    /// Creates a set optional containing <paramref name="value" />. Passing <see langword="null" /> or
    /// <c>default(T)</c> still creates a set optional; use <see cref="Unset" /> to represent omission.
    /// </summary>
    public static Optional<T> FromValue(T value) => new Optional<T>(value);

    /// <summary>
    /// Attempts to get the supplied value. Returns <see langword="true" /> when null was explicitly supplied, with
    /// <paramref name="value" /> set to <see langword="null" />; returns <see langword="false" /> only when unset.
    /// </summary>
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

    /// <summary>
    /// Creates a set optional from <paramref name="value" />. In particular, assigning <see langword="null" /> to an
    /// <c>Optional&lt;T?&gt;</c> means explicit null, not unset.
    /// </summary>
    public static implicit operator Optional<T>(T value) => FromValue(value);
}
