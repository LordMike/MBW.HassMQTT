using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;
using MBW.HassMQTT.DiscoveryModels.Models;
using MBW.HassMQTT.Serialization;
using Newtonsoft.Json.Linq;
using Xunit;

namespace MBW.HassMQTT.Tests;

public class DiscoveryModelCapabilityTests
{
    [Fact]
    public void SharedCapabilitiesUseCanonicalInterfaces()
    {
        var capabilityProperties = new Dictionary<string, Type>
        {
            [nameof(IHasDefaultEntityId.DefaultEntityId)] = typeof(IHasDefaultEntityId),
            [nameof(IHasEntityPicture.EntityPicture)] = typeof(IHasEntityPicture),
            [nameof(IHasGroup.Group)] = typeof(IHasGroup),
            [nameof(IHasMessageExpiryInterval.MessageExpiryInterval)] = typeof(IHasMessageExpiryInterval),
            [nameof(IHasVisibleByDefault.VisibleByDefault)] = typeof(IHasVisibleByDefault),
        };

        Type discoveryDocumentType = typeof(IHassDiscoveryDocument);
        IEnumerable<Type> modelTypes = typeof(MqttSensor).Assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && discoveryDocumentType.IsAssignableFrom(type));

        foreach (Type modelType in modelTypes)
        {
            foreach ((string propertyName, Type capabilityType) in capabilityProperties)
            {
                if (modelType.GetProperty(propertyName) != null)
                    Assert.True(capabilityType.IsAssignableFrom(modelType),
                        $"{modelType.Name} exposes {propertyName} without implementing {capabilityType.Name}");
            }
        }
    }

    [Fact]
    public void SharedCapabilitiesSerializeWithDocumentedNames()
    {
        var model = new MqttSensor("homeassistant/sensor/example/config", "example")
        {
            DefaultEntityId = "sensor.example",
            EntityPicture = "https://example.com/entity.png",
            Group = new List<string> { "temperature", "humidity" },
            VisibleByDefault = false,
        };

        JObject json = JObject.FromObject(model, CustomJsonSerializer.Serializer);

        Assert.Equal("sensor.example", json.Value<string>("default_entity_id"));
        Assert.Equal("https://example.com/entity.png", json.Value<string>("entity_picture"));
        Assert.Equal(new[] { "temperature", "humidity" }, json["group"]!.Values<string>());
        Assert.False(json.Value<bool>("visible_by_default"));
        Assert.Null(json["object_id"]);
    }

    [Fact]
    public void MessageExpiryIntervalSerializesAsMap()
    {
        var model = new MqttAlarmControlPanel("homeassistant/alarm_control_panel/example/config", "example")
        {
            MessageExpiryInterval = new MessageExpiryInterval
            {
                Days = 1,
                Hours = 2,
                Minutes = 20,
                Seconds = 30,
            },
        };

        JObject json = JObject.FromObject(model, CustomJsonSerializer.Serializer);
        JObject interval = Assert.IsType<JObject>(json["message_expiry_interval"]);

        Assert.Equal(1, interval.Value<int>("days"));
        Assert.Equal(2, interval.Value<int>("hours"));
        Assert.Equal(20, interval.Value<int>("minutes"));
        Assert.Equal(30, interval.Value<int>("seconds"));
    }

    [Fact]
    public void AvailabilityListAndTopicAreMutuallyExclusive()
    {
        var model = new MqttSensor("homeassistant/sensor/example/config", "example")
        {
            AvailabilityTopic = "example/availability",
            StateTopic = "example/state",
        };
        model.Device.Identifiers.Add("example");
#pragma warning disable 618
        model.Availability!.Add(new AvailabilityModel { Topic = "example/availability-list" });
#pragma warning restore 618

        ValidationResult result = MqttSensor.Validator.Validate(model);

        Assert.Contains(result.Errors, failure => failure.ErrorMessage.Contains("cannot be used together"));
    }
}
