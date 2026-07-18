#nullable enable

using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MBW.HassMQTT.DiscoveryModels.Device;

internal sealed class IdentifierCollectionConverter : JsonConverter<ObservableCollection<string>>
{
    public IdentifierCollectionConverter()
    {
    }

    public override void WriteJson(JsonWriter writer, ObservableCollection<string>? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        if (value.Count == 1)
        {
            writer.WriteValue(value[0]);
            return;
        }

        writer.WriteStartArray();
        foreach (string identifier in value)
            writer.WriteValue(identifier);
        writer.WriteEndArray();
    }

    public override ObservableCollection<string> ReadJson(
        JsonReader reader,
        Type objectType,
        ObservableCollection<string>? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        ObservableCollection<string> identifiers = hasExistingValue && existingValue != null
            ? existingValue
            : new ObservableCollection<string>();

        JToken token = JToken.Load(reader);
        switch (token.Type)
        {
            case JTokenType.String:
                identifiers.Add(token.Value<string>()!);
                break;
            case JTokenType.Array:
                foreach (JToken item in token.Children())
                {
                    if (item.Type != JTokenType.String)
                        throw new JsonSerializationException("Device identifiers must be strings.");

                    identifiers.Add(item.Value<string>()!);
                }
                break;
            default:
                throw new JsonSerializationException("Device identifiers must be a string or an array of strings.");
        }

        return identifiers;
    }
}
