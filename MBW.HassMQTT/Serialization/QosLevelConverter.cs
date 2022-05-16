using System;
using MBW.HassMQTT.DiscoveryModels.Enum;
using Newtonsoft.Json;

namespace MBW.HassMQTT.Serialization;

internal class QosLevelConverter : JsonConverter<MqttQosLevel?>
{
    public override void WriteJson(JsonWriter writer, MqttQosLevel? value, JsonSerializer serializer)
    {
        writer.WriteValue((byte)value.GetValueOrDefault());
    }

    public override MqttQosLevel? ReadJson(JsonReader reader, Type objectType, MqttQosLevel? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        int? value = reader.ReadAsInt32();
        if (value.HasValue)
            return (MqttQosLevel)value;

        return default;
    }
}