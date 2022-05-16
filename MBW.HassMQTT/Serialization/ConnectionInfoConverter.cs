using System;
using MBW.HassMQTT.DiscoveryModels.Metadata;
using Newtonsoft.Json;

namespace MBW.HassMQTT.Serialization;

internal class ConnectionInfoConverter : JsonConverter<ConnectionInfo>
{
    public override void WriteJson(JsonWriter writer, ConnectionInfo value, JsonSerializer serializer)
    {
        writer.WriteStartArray();
        writer.WriteValue(value.Type);
        writer.WriteValue(value.Value);
        writer.WriteEndArray();
    }

    public override ConnectionInfo ReadJson(JsonReader reader, Type objectType, ConnectionInfo existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        string type = reader.ReadAsString();
        string value = reader.ReadAsString();

        reader.Read();

        return new ConnectionInfo
        {
            Type = type,
            Value = value
        };
    }
}