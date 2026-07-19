using System;
using System.Text.Json;

namespace MBW.HassMQTT.Serialization;

internal static class DiscoverySnapshotSerializer
{
    public static byte[] Capture<T>(T value) => JsonSerializer.SerializeToUtf8Bytes(value, HassJson.SnapshotOptions);

    public static T Restore<T>(byte[] snapshot, Func<T> factory)
    {
        JsonSerializerOptions options = HassJson.CreateSnapshotOptions(typeof(T), () => factory()!);
        return JsonSerializer.Deserialize<T>(snapshot, options)
            ?? throw new JsonException($"Unable to restore a {typeof(T).Name} discovery snapshot.");
    }
}
