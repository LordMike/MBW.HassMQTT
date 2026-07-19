using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using MBW.HassMQTT.DiscoveryModels.Enum;

namespace MBW.HassMQTT.Serialization;

internal sealed class QosLevelConverter : JsonConverter<MqttQosLevel>
{
    public override MqttQosLevel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.Number || !reader.TryGetByte(out byte value) || value > 2)
            throw new JsonException("MQTT QoS must be an integer between 0 and 2.");

        return (MqttQosLevel)value;
    }

    public override void Write(Utf8JsonWriter writer, MqttQosLevel value, JsonSerializerOptions options)
    {
        if (!Enum.IsDefined(typeof(MqttQosLevel), value))
            throw new JsonException($"Unsupported MQTT QoS level: {(byte)value}");

        writer.WriteNumberValue((byte)value);
    }
}
