﻿using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MBW.HassMQTT.Helpers;

public static class MqttHelpers
{
    private static readonly Encoding Encoding = new UTF8Encoding(false);

    private static byte[] ConvertJson(JToken token)
    {
        using MemoryStream ms = new MemoryStream();

        using (StreamWriter sw = new StreamWriter(ms, Encoding))
        using (JsonTextWriter jw = new JsonTextWriter(sw))
        {
            token.WriteTo(jw);
        }

        return ms.ToArray();
    }

    public static Task SendJsonAsync(this IManagedMqttClient mqttClient, string topic, JToken doc, CancellationToken token = default)
    {
        return mqttClient.PublishAsync(new MqttApplicationMessage
        {
            Topic = topic,
            Retain = true,
            QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce,
            Payload = ConvertJson(doc)
        }, token);
    }

    public static Task SendValueAsync(this IManagedMqttClient mqttClient, string topic, string value, CancellationToken token = default)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);

        return mqttClient.PublishAsync(new MqttApplicationMessage
        {
            Topic = topic,
            Retain = true,
            QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce,
            Payload = bytes
        }, token);
    }
}