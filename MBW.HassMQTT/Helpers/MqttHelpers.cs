using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;
using MBW.HassMQTT.Interfaces;
using MBW.HassMQTT.Serialization;
using MQTTnet;
using MQTTnet.Protocol;

namespace MBW.HassMQTT.Helpers;

public static class MqttHelpers
{
    private static readonly Encoding Encoding = new UTF8Encoding(false);

    public static Task<MqttClientPublishResult> SendJsonAsync<T>(this IHassMqttClient mqttClient, string topic, T document, CancellationToken token = default) =>
        mqttClient.PublishAsync(CreateMessage(topic, JsonSerializer.SerializeToUtf8Bytes(document, HassJson.WireOptions)), token);

    public static Task<MqttClientPublishResult> SendJsonAsync<T>(this IHassMqttClient mqttClient, string topic, T document, JsonTypeInfo<T> jsonTypeInfo, CancellationToken token = default) =>
        mqttClient.PublishAsync(CreateMessage(topic, JsonSerializer.SerializeToUtf8Bytes(document, jsonTypeInfo)), token);

    public static Task<MqttClientPublishResult> SendValueAsync(this IHassMqttClient mqttClient, string topic, string value, CancellationToken token = default) =>
        mqttClient.PublishAsync(CreateMessage(topic, Encoding.GetBytes(value)), token);

    public static MqttApplicationMessage CreateMessage(string topic, byte[] payload) =>
        new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .WithRetainFlag()
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();
}
