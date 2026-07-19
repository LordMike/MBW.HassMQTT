#nullable enable

using System;
using System.Text.Json;
using MBW.HassMQTT.Internal;
using MBW.HassMQTT.Serialization;

namespace MBW.HassMQTT;

/// <summary>A value that can be published either directly to MQTT or as part of a JSON payload.</summary>
public readonly struct MqttValue : IEquatable<MqttValue>
{
    private readonly MqttValueKind _kind;
    private readonly object? _value;

    /// <summary>A semantic null value.</summary>
    public static MqttValue Null => default;

    private MqttValue(MqttValueKind kind, object value)
    {
        _kind = kind;
        _value = value;
    }

    /// <summary>Returns the underlying JSON-compatible value.</summary>
    /// <remarks>
    /// The result is <see langword="null"/>, a <see cref="string"/>, a <see cref="bool"/>,
    /// a numeric primitive, or an owned <see cref="JsonElement"/>.
    /// </remarks>
    public object? GetValue() => _value;

    /// <summary>Creates a structured value from one complete JSON document.</summary>
    public static MqttValue FromJson(string json)
    {
        ArgumentNullException.ThrowIfNull(json);
        using JsonDocument document = JsonDocument.Parse(json);
        return FromJsonElement(document.RootElement);
    }

    /// <summary>Creates a structured value from one complete UTF-8 JSON document.</summary>
    public static MqttValue FromJson(ReadOnlySpan<byte> utf8Json)
    {
        using JsonDocument document = JsonDocument.Parse(utf8Json.ToArray());
        return FromJsonElement(document.RootElement);
    }

    /// <summary>Creates a string value using the enum's Home Assistant wire name.</summary>
    public static MqttValue FromEnum<TEnum>(TEnum value) where TEnum : struct, Enum
    {
        if (!EnumMemberValue<TEnum>.TryGetWireValue(value, out string? wireValue))
            throw new ArgumentOutOfRangeException(nameof(value), value, $"Unsupported {typeof(TEnum).Name} value");

        return new MqttValue(MqttValueKind.String, wireValue!);
    }

    public static implicit operator MqttValue(string? value) =>
        value is null ? Null : new MqttValue(MqttValueKind.String, value);

    public static implicit operator MqttValue(bool value) => new(MqttValueKind.Boolean, value);
    public static implicit operator MqttValue(byte value) => new(MqttValueKind.Number, value);
    public static implicit operator MqttValue(sbyte value) => new(MqttValueKind.Number, value);
    public static implicit operator MqttValue(short value) => new(MqttValueKind.Number, value);
    public static implicit operator MqttValue(ushort value) => new(MqttValueKind.Number, value);
    public static implicit operator MqttValue(int value) => new(MqttValueKind.Number, value);
    public static implicit operator MqttValue(uint value) => new(MqttValueKind.Number, value);
    public static implicit operator MqttValue(long value) => new(MqttValueKind.Number, value);
    public static implicit operator MqttValue(ulong value) => new(MqttValueKind.Number, value);
    public static implicit operator MqttValue(decimal value) => new(MqttValueKind.Number, value);

    public static implicit operator MqttValue(float value)
    {
        if (!float.IsFinite(value))
            throw new ArgumentOutOfRangeException(nameof(value), value, "MQTT values cannot contain non-finite numbers");
        return new MqttValue(MqttValueKind.Number, value);
    }

    public static implicit operator MqttValue(double value)
    {
        if (!double.IsFinite(value))
            throw new ArgumentOutOfRangeException(nameof(value), value, "MQTT values cannot contain non-finite numbers");
        return new MqttValue(MqttValueKind.Number, value);
    }

    public static implicit operator MqttValue(DateTime value) =>
        new(MqttValueKind.String, value.ToIso8601());

    public static implicit operator MqttValue(DateTimeOffset value) =>
        new(MqttValueKind.String, value.ToIso8601());

    public static implicit operator MqttValue(JsonElement value) => FromJsonElement(value);

    public bool Equals(MqttValue other)
    {
        if (_kind != other._kind)
            return false;

        return _kind switch
        {
            MqttValueKind.Null => true,
            MqttValueKind.String => string.Equals((string?)_value, (string?)other._value, StringComparison.Ordinal),
            MqttValueKind.Boolean => (bool)_value! == (bool)other._value!,
            MqttValueKind.Number or MqttValueKind.Json =>
                MqttValueSerializer.JsonValueEquals(this, other),
            _ => false
        };
    }

    public override bool Equals(object? obj) => obj is MqttValue other && Equals(other);

    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(_kind);
        if (_kind is MqttValueKind.Number or MqttValueKind.Json)
        {
            foreach (byte value in MqttValueSerializer.SerializeJsonValue(this))
                hash.Add(value);
        }
        else
        {
            hash.Add(_value);
        }
        return hash.ToHashCode();
    }

    public static bool operator ==(MqttValue left, MqttValue right) => left.Equals(right);
    public static bool operator !=(MqttValue left, MqttValue right) => !left.Equals(right);

    internal MqttValueKind Kind => _kind;
    internal object? Value => _value;

    private static MqttValue FromJsonElement(JsonElement value)
    {
        if (value.ValueKind == JsonValueKind.Undefined)
            throw new ArgumentException("An undefined JSON element is not a valid MQTT value", nameof(value));

        return new MqttValue(MqttValueKind.Json, value.Clone());
    }
}

internal enum MqttValueKind
{
    Null,
    String,
    Boolean,
    Number,
    Json
}
