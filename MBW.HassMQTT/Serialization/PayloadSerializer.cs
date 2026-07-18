using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MBW.HassMQTT.Serialization;

internal static class PayloadSerializer
{
    public static byte[] Serialize(object value)
    {
        if (value is string text)
            return Encoding.UTF8.GetBytes(text);
        if (value == null)
            return Array.Empty<byte>();

        lock (CustomJsonSerializer.Serializer)
        {
            JToken converted = JToken.FromObject(value, CustomJsonSerializer.Serializer);
            return Encoding.UTF8.GetBytes(converted.ToString(Formatting.None));
        }
    }
}
