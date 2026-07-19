#nullable enable

using System;
using System.Text.Json;

namespace MBW.HassMQTT.Serialization;

internal static class DiscoveryJsonSerializer
{
    public static byte[] Serialize<T>(T value) => JsonSerializer.SerializeToUtf8Bytes(value, HassJson.WireOptions);

    public static object? Deserialize(byte[] json, Type type) => JsonSerializer.Deserialize(json, type, HassJson.DeserializationOptions);

    public static T? Deserialize<T>(byte[] json) => JsonSerializer.Deserialize<T>(json, HassJson.DeserializationOptions);
}
