#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using MBW.HassMQTT.DiscoveryModels;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Models;
using MBW.HassMQTT.DiscoveryModels.Serialization;
using MBW.HassMQTT.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
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
    public void Number_unit_distinguishes_missing_null_and_value()
    {
        MqttNumber missing = DeserializeNumber("{}");
        MqttNumber explicitNull = DeserializeNumber("""{ "unit_of_measurement": null }""");
        MqttNumber value = DeserializeNumber("""{ "unit_of_measurement": "°C" }""");

        Assert.False(missing.UnitOfMeasurement.IsSet);
        Assert.Null(Serialize(missing).Property("unit_of_measurement"));

        Assert.True(explicitNull.UnitOfMeasurement.IsSet);
        Assert.Null(explicitNull.UnitOfMeasurement.Value);
        Assert.Equal(JTokenType.Null, Serialize(explicitNull)["unit_of_measurement"]!.Type);

        Assert.True(value.UnitOfMeasurement.IsSet);
        Assert.Equal("°C", value.UnitOfMeasurement.Value);
        Assert.Equal("°C", (string?)Serialize(value)["unit_of_measurement"]);
    }

    [Fact]
    public void Explicit_null_is_rejected_for_non_nullable_optional_value()
    {
        Assert.Throws<JsonSerializationException>(() =>
            JObject.Parse("""{ "value": null }""")
                .ToObject<NonNullableOptionalDocument>(CustomJsonSerializer.Serializer));
    }

    [Fact]
    public void Discovery_models_package_exposes_optional_serialization_support()
    {
        Assert.True(typeof(OptionalJsonConverter).IsPublic);
        Assert.True(typeof(OptionalAwareContractResolver).IsPublic);
        Assert.Equal(typeof(Optional<>).Assembly, typeof(OptionalJsonConverter).Assembly);
        Assert.Equal(typeof(Optional<>).Assembly, typeof(OptionalAwareContractResolver).Assembly);

        JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            ContractResolver = new OptionalAwareContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy(),
            },
            Converters = { new OptionalJsonConverter() },
        });
        var cover = new MqttCover("homeassistant/cover/example/config", "example")
        {
            PayloadClose = null,
        };

        JObject json = JObject.FromObject(cover, serializer);

        Assert.Equal(JTokenType.Null, json["payload_close"]!.Type);
        Assert.Null(json.Property("payload_open"));

        MqttCover roundTripped = json.ToObject<MqttCover>(serializer)!;
        Assert.True(roundTripped.PayloadClose.IsSet);
        Assert.Null(roundTripped.PayloadClose.Value);
        Assert.False(roundTripped.PayloadOpen.IsSet);
    }

    private static JObject Serialize(object value) => JObject.FromObject(value, CustomJsonSerializer.Serializer);

    private static MqttCover DeserializeCover(string json) =>
        JObject.Parse(json).ToObject<MqttCover>(CustomJsonSerializer.Serializer)!;

    private static MqttNumber DeserializeNumber(string json) =>
        JObject.Parse(json).ToObject<MqttNumber>(CustomJsonSerializer.Serializer)!;

    private sealed class NonNullableOptionalDocument
    {
        public Optional<int> Value { get; set; }
    }
}
