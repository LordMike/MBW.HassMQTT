using System;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MBW.HassMQTT.Serialization;

internal sealed class IdentifierCollectionConverter : JsonConverter<ObservableCollection<string>>
{
    public override ObservableCollection<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        ObservableCollection<string> result = new();
        if (reader.TokenType == JsonTokenType.String)
        {
            result.Add(reader.GetString()!);
            return result;
        }
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("Device identifiers must be a string or an array of strings.");
        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException("Device identifiers must be strings.");
            result.Add(reader.GetString()!);
        }
        return result;
    }

    public override void Write(Utf8JsonWriter writer, ObservableCollection<string> value, JsonSerializerOptions options)
    {
        if (value.Count == 1)
        {
            writer.WriteStringValue(value[0]);
            return;
        }
        writer.WriteStartArray();
        foreach (string identifier in value)
            writer.WriteStringValue(identifier);
        writer.WriteEndArray();
    }
}
