#nullable enable

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using MBW.HassMQTT.DiscoveryModels;

namespace MBW.HassMQTT.Serialization;

internal sealed class OptionalJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Optional<>);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Type valueType = typeToConvert.GetGenericArguments()[0];
        return (JsonConverter)Activator.CreateInstance(typeof(OptionalJsonConverter<>).MakeGenericType(valueType))!;
    }

    private sealed class OptionalJsonConverter<T> : JsonConverter<Optional<T>>
    {
        public override bool HandleNull => true;

        public override Optional<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null && default(T) is not null)
                throw new JsonException($"Cannot assign null to {typeToConvert} because {typeof(T)} is not nullable.");

            return Optional<T>.FromValue(JsonSerializer.Deserialize<T>(ref reader, options)!);
        }

        public override void Write(Utf8JsonWriter writer, Optional<T> value, JsonSerializerOptions options)
        {
            if (!value.IsSet)
                throw new JsonException("An unset Optional<T> must be omitted by its containing object contract.");

            JsonSerializer.Serialize(writer, value.Value, options);
        }
    }
}
