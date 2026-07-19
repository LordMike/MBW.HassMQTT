using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.Serialization;

internal sealed class ConnectionInfoConverter : JsonConverter<ConnectionInfo>
{
    public override ConnectionInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("A device connection must be a two-element string array.");
        if (!reader.Read() || reader.TokenType != JsonTokenType.String)
            throw new JsonException("A device connection type must be a string.");
        string type = reader.GetString()!;
        if (!reader.Read() || reader.TokenType != JsonTokenType.String)
            throw new JsonException("A device connection value must be a string.");
        string value = reader.GetString()!;
        if (!reader.Read() || reader.TokenType != JsonTokenType.EndArray)
            throw new JsonException("A device connection must contain exactly two values.");
        return new ConnectionInfo { Type = type, Value = value };
    }

    public override void Write(Utf8JsonWriter writer, ConnectionInfo value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        writer.WriteStringValue(value.Type);
        writer.WriteStringValue(value.Value);
        writer.WriteEndArray();
    }
}
