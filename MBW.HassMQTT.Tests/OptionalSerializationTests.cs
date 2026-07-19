#nullable enable

using System;
using MBW.HassMQTT.DiscoveryModels;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Models;
using MBW.HassMQTT.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace MBW.HassMQTT.Tests;

public class OptionalSerializationTests
{
    [Fact]
    public void Optional_distinguishes_unset_null_and_value()
    {
        Optional<string?> unset = default;
        Optional<string?> explicitNull = null;
        Optional<string?> value = "CLOSE";

        Assert.False(unset.IsSet);
        Assert.Throws<InvalidOperationException>(() => unset.Value);
        Assert.False(unset.TryGetValue(out _));

        Assert.True(explicitNull.IsSet);
        Assert.Null(explicitNull.Value);
        Assert.True(explicitNull.TryGetValue(out string? nullValue));
        Assert.Null(nullValue);

        Assert.True(value.IsSet);
        Assert.Equal("CLOSE", value.Value);
        Assert.NotEqual(unset, explicitNull);
        Assert.NotEqual(explicitNull, value);
        Assert.Equal(Optional<string?>.Unset, unset);
        Assert.Equal("Unset", unset.ToString());
        Assert.Equal("null", explicitNull.ToString());

        Optional<int> explicitDefault = Optional<int>.FromValue(0);
        Assert.True(explicitDefault.IsSet);
        Assert.Equal(0, explicitDefault.Value);
        Assert.NotEqual(default, explicitDefault);
    }

    [Fact]
    public void Property_notifications_distinguish_unset_and_explicit_null()
    {
        var cover = new MqttCover("homeassistant/cover/example/config", "example");
        int changes = 0;
        cover.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(MqttCover.PayloadClose))
                changes++;
        };

        cover.PayloadClose = null;
        cover.PayloadClose = default;

        Assert.Equal(2, changes);
    }

    [Fact]
    public void Serializer_omits_unset_and_preserves_explicit_null_and_value()
    {
        var cover = new MqttCover("homeassistant/cover/example/config", "example");

        Assert.Null(Serialize(cover).Property("payload_close"));

        cover.PayloadClose = null;
        Assert.Equal(JTokenType.Null, Serialize(cover)["payload_close"]!.Type);

        cover.PayloadClose = "CLOSE";
        Assert.Equal("CLOSE", (string?)Serialize(cover)["payload_close"]);

        cover.PayloadClose = default;
        Assert.Null(Serialize(cover).Property("payload_close"));
    }

    [Fact]
    public void Deserializer_distinguishes_missing_null_and_value()
    {
        MqttCover missing = DeserializeCover("{}");
        MqttCover explicitNull = DeserializeCover("""{ "payload_close": null }""");
        MqttCover value = DeserializeCover("""{ "payload_close": "CLOSE" }""");

        Assert.False(missing.PayloadClose.IsSet);
        Assert.True(explicitNull.PayloadClose.IsSet);
        Assert.Null(explicitNull.PayloadClose.Value);
        Assert.True(value.PayloadClose.IsSet);
        Assert.Equal("CLOSE", value.PayloadClose.Value);
    }

    [Fact]
    public void Optional_enum_uses_existing_string_enum_contract()
    {
        var sensor = new MqttSensor("homeassistant/sensor/example/config", "example")
        {
            DeviceClass = HassSensorDeviceClass.Temperature,
        };

        Assert.Equal("temperature", (string?)Serialize(sensor)["device_class"]);

        sensor.DeviceClass = null;
        Assert.Equal(JTokenType.Null, Serialize(sensor)["device_class"]!.Type);
    }

    [Fact]
    public void Explicit_null_is_rejected_for_non_nullable_optional_value()
    {
        Assert.Throws<JsonSerializationException>(() =>
            JObject.Parse("""{ "value": null }""")
                .ToObject<NonNullableOptionalDocument>(CustomJsonSerializer.Serializer));
    }

    private static JObject Serialize(object value) => JObject.FromObject(value, CustomJsonSerializer.Serializer);

    private static MqttCover DeserializeCover(string json) =>
        JObject.Parse(json).ToObject<MqttCover>(CustomJsonSerializer.Serializer)!;

    private sealed class NonNullableOptionalDocument
    {
        public Optional<int> Value { get; set; }
    }
}
