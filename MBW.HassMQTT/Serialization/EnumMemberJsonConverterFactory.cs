#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MBW.HassMQTT.Serialization;

internal sealed class EnumMemberJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsEnum;

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
        (JsonConverter)Activator.CreateInstance(typeof(EnumMemberJsonConverter<>).MakeGenericType(typeToConvert))!;

    private sealed class EnumMemberJsonConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
    {
        public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String && EnumMemberValue<TEnum>.TryParseWireValue(reader.GetString()!, out TEnum value))
                return value;
            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt64(out long number))
                return (TEnum)Enum.ToObject(typeof(TEnum), number);
            throw new JsonException($"Invalid {typeof(TEnum).Name} value.");
        }

        public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        {
            if (!EnumMemberValue<TEnum>.TryGetWireValue(value, out string? wireValue))
                throw new JsonException($"Unsupported {typeof(TEnum).Name} value: {value}");
            writer.WriteStringValue(wireValue);
        }

        public override TEnum ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (EnumMemberValue<TEnum>.TryParseWireValue(reader.GetString()!, out TEnum value))
                return value;
            throw new JsonException($"Invalid {typeof(TEnum).Name} dictionary key.");
        }

        public override void WriteAsPropertyName(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        {
            if (!EnumMemberValue<TEnum>.TryGetWireValue(value, out string? wireValue))
                throw new JsonException($"Unsupported {typeof(TEnum).Name} dictionary key: {value}");
            writer.WritePropertyName(wireValue!);
        }

    }
}

internal static class EnumMemberValue<TEnum> where TEnum : struct, Enum
{
    private static readonly bool IsFlags = typeof(TEnum).IsDefined(typeof(FlagsAttribute), false);
    private static readonly Dictionary<TEnum, string> ToWire = CreateToWire();
    private static readonly Dictionary<string, TEnum> FromWire = CreateFromWire();

    public static bool TryGetWireValue(TEnum value, out string? wireValue)
    {
        if (ToWire.TryGetValue(value, out wireValue))
            return true;
        if (!IsFlags)
            return false;

        string[] names = value.ToString().Split(',');
        if (names.Length < 2)
            return false;

        List<string> wireNames = new(names.Length);
        foreach (string name in names)
        {
            if (!Enum.TryParse(name.Trim(), false, out TEnum member) || !ToWire.TryGetValue(member, out string? memberWireValue))
                return false;
            wireNames.Add(memberWireValue);
        }

        wireValue = string.Join(", ", wireNames);
        return true;
    }

    public static bool TryParseWireValue(string wireValue, out TEnum value)
    {
        if (FromWire.TryGetValue(wireValue, out value))
            return true;
        if (!IsFlags)
            return false;

        string[] wireNames = wireValue.Split(',');
        if (wireNames.Length < 2)
            return false;

        List<string> memberNames = new(wireNames.Length);
        foreach (string wireName in wireNames)
        {
            if (!FromWire.TryGetValue(wireName.Trim(), out TEnum member))
                return false;
            memberNames.Add(member.ToString());
        }

        return Enum.TryParse(string.Join(", ", memberNames), false, out value);
    }

    private static Dictionary<TEnum, string> CreateToWire()
    {
        Dictionary<TEnum, string> result = new();
        foreach (FieldInfo field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            TEnum value = (TEnum)field.GetValue(null)!;
            string wire = field.GetCustomAttribute<EnumMemberAttribute>()?.Value
                ?? JsonNamingPolicy.SnakeCaseLower.ConvertName(field.Name);
            result[value] = wire;
        }
        return result;
    }

    private static Dictionary<string, TEnum> CreateFromWire()
    {
        Dictionary<string, TEnum> result = new(StringComparer.OrdinalIgnoreCase);
        foreach ((TEnum value, string wire) in ToWire)
            result[wire] = value;
        return result;
    }
}
