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
        private static readonly Dictionary<TEnum, string> ToWire = CreateToWire();
        private static readonly Dictionary<string, TEnum> FromWire = CreateFromWire();

        public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String && FromWire.TryGetValue(reader.GetString()!, out TEnum value))
                return value;
            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt64(out long number))
                return (TEnum)Enum.ToObject(typeof(TEnum), number);
            throw new JsonException($"Invalid {typeof(TEnum).Name} value.");
        }

        public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        {
            if (!ToWire.TryGetValue(value, out string? wireValue))
                throw new JsonException($"Unsupported {typeof(TEnum).Name} value: {value}");
            writer.WriteStringValue(wireValue);
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
}
