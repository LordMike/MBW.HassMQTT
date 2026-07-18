using System;
using MBW.HassMQTT.DiscoveryModels.Enum;
using Newtonsoft.Json;

namespace MBW.HassMQTT.Serialization;

internal class QosLevelConverter : JsonConverter<MqttQosLevel?>
{
    public override void WriteJson(JsonWriter writer, MqttQosLevel? value, JsonSerializer serializer)
    {
        if (!value.HasValue)
        {
            writer.WriteNull();
            return;
        }

        if (!System.Enum.IsDefined(typeof(MqttQosLevel), value.Value))
            throw new JsonSerializationException($"Unsupported MQTT QoS level: {(byte)value.Value}");

        writer.WriteValue((byte)value.Value);
    }

    public override MqttQosLevel? ReadJson(JsonReader reader, Type objectType, MqttQosLevel? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        if (reader.TokenType != JsonToken.Integer)
            throw new JsonSerializationException($"Expected an integer MQTT QoS level, got {reader.TokenType}");

        int value = Convert.ToInt32(reader.Value);
        if (!System.Enum.IsDefined(typeof(MqttQosLevel), (byte)value) || value is < 0 or > 2)
            throw new JsonSerializationException($"Unsupported MQTT QoS level: {value}");

        return (MqttQosLevel)value;
    }
}
