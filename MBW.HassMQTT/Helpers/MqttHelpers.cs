using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MBW.HassMQTT.Interfaces;
using MQTTnet;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MBW.HassMQTT.Helpers;

public static class MqttHelpers
{
    private static readonly Encoding Encoding = new UTF8Encoding(false);

    private static byte[] ConvertJson(JToken token)
    {
        using MemoryStream stream = new MemoryStream();
        using (StreamWriter writer = new StreamWriter(stream, Encoding))
        using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
            token.WriteTo(jsonWriter);

        return stream.ToArray();
    }

    public static Task<MqttClientPublishResult> SendJsonAsync(this IHassMqttClient mqttClient, string topic, JToken document, CancellationToken token = default) =>
        mqttClient.PublishAsync(CreateMessage(topic, ConvertJson(document)), token);

    public static Task<MqttClientPublishResult> SendValueAsync(this IHassMqttClient mqttClient, string topic, string value, CancellationToken token = default) =>
        mqttClient.PublishAsync(CreateMessage(topic, Encoding.UTF8.GetBytes(value)), token);

    public static MqttApplicationMessage CreateMessage(string topic, byte[] payload) =>
        new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .WithRetainFlag()
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();
}
