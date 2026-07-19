#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using MBW.HassMQTT.DiscoveryModels;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Models;
using MBW.HassMQTT.Serialization;
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
    public void Try_get_value_marks_failed_output_as_maybe_null()
    {
        ParameterInfo valueParameter = typeof(Optional<>).GetMethod(nameof(Optional<object>.TryGetValue))!
            .GetParameters()[0];
        MaybeNullWhenAttribute attribute = Assert.IsType<MaybeNullWhenAttribute>(
            valueParameter.GetCustomAttribute(typeof(MaybeNullWhenAttribute)));

        Assert.False(attribute.ReturnValue);
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

        Assert.False(Serialize(cover).ContainsKey("payload_close"));

        cover.PayloadClose = null;
        Assert.True(Serialize(cover).ContainsKey("payload_close"));
        Assert.Null(Serialize(cover)["payload_close"]);

        cover.PayloadClose = "CLOSE";
        Assert.Equal("CLOSE", Serialize(cover)["payload_close"]!.GetValue<string>());

        cover.PayloadClose = default;
        Assert.False(Serialize(cover).ContainsKey("payload_close"));
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

        Assert.Equal("temperature", Serialize(sensor)["device_class"]!.GetValue<string>());

        sensor.DeviceClass = null;
        JsonObject json = Serialize(sensor);
        Assert.True(json.ContainsKey("device_class"));
        Assert.Null(json["device_class"]);
    }

    [Fact]
    public void Number_unit_distinguishes_missing_null_and_value()
    {
        MqttNumber missing = DeserializeNumber("{}");
        MqttNumber explicitNull = DeserializeNumber("""{ "unit_of_measurement": null }""");
        MqttNumber value = DeserializeNumber("""{ "unit_of_measurement": "°C" }""");

        Assert.False(missing.UnitOfMeasurement.IsSet);
        Assert.False(Serialize(missing).ContainsKey("unit_of_measurement"));

        Assert.True(explicitNull.UnitOfMeasurement.IsSet);
        Assert.Null(explicitNull.UnitOfMeasurement.Value);
        JsonObject explicitNullJson = Serialize(explicitNull);
        Assert.True(explicitNullJson.ContainsKey("unit_of_measurement"));
        Assert.Null(explicitNullJson["unit_of_measurement"]);

        Assert.True(value.UnitOfMeasurement.IsSet);
        Assert.Equal("°C", value.UnitOfMeasurement.Value);
        Assert.Equal("°C", Serialize(value)["unit_of_measurement"]!.GetValue<string>());
    }

    [Fact]
    public void Explicit_null_is_rejected_for_non_nullable_optional_value()
    {
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<NonNullableOptionalDocument>(
                """{ "value": null }""",
                HassJson.DeserializationOptions));
    }

    [Fact]
    public void Snapshot_preserves_optional_presence_states()
    {
        var source = new MqttCover(string.Empty, "source")
        {
            PayloadClose = null,
            PayloadOpen = "OPEN",
        };

        byte[] snapshot = DiscoverySnapshotSerializer.Capture(source);
        MqttCover restored = DiscoverySnapshotSerializer.Restore(
            snapshot,
            () => new MqttCover(string.Empty, "target"));

        Assert.True(restored.PayloadClose.IsSet);
        Assert.Null(restored.PayloadClose.Value);
        Assert.True(restored.PayloadOpen.IsSet);
        Assert.Equal("OPEN", restored.PayloadOpen.Value);
        Assert.False(restored.PayloadStop.IsSet);
    }

    [Fact]
    public void Runtime_serializer_uses_optional_contract()
    {
        var payload = new OptionalPayload();
        Assert.False(ParsePayload(payload).ContainsKey("value"));

        payload.Value = null;
        JsonObject explicitNull = ParsePayload(payload);
        Assert.True(explicitNull.ContainsKey("value"));
        Assert.Null(explicitNull["value"]);

        payload.Value = "set";
        Assert.Equal("set", ParsePayload(payload)["value"]!.GetValue<string>());
    }

    private static JsonObject Serialize<T>(T value) =>
        JsonNode.Parse(DiscoveryJsonSerializer.Serialize(value))!.AsObject();

    private static MqttCover DeserializeCover(string json) =>
        DiscoveryJsonSerializer.Deserialize<MqttCover>(JsonSerializer.SerializeToUtf8Bytes(JsonNode.Parse(json)))!;

    private static MqttNumber DeserializeNumber(string json) =>
        DiscoveryJsonSerializer.Deserialize<MqttNumber>(JsonSerializer.SerializeToUtf8Bytes(JsonNode.Parse(json)))!;

    private static JsonObject ParsePayload(object value) =>
        JsonNode.Parse(PayloadSerializer.Serialize(value))!.AsObject();

    private sealed class NonNullableOptionalDocument
    {
        public Optional<int> Value { get; set; }
    }

    private sealed class OptionalPayload
    {
        public Optional<string?> Value { get; set; }
    }
}
