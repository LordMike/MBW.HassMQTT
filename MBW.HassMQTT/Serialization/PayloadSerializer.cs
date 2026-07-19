#nullable enable

using System;
using System.Text;
using System.Text.Json;

namespace MBW.HassMQTT.Serialization;

internal static class PayloadSerializer
{
    public static byte[] Serialize(object? value)
    {
        if (value is string text)
            return Encoding.UTF8.GetBytes(text);
        if (value == null)
            return Array.Empty<byte>();

        return JsonSerializer.SerializeToUtf8Bytes(value, value.GetType(), HassJson.RuntimeOptions);
    }
}
