using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using MBW.HassMQTT.DiscoveryModels;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Device;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Models;
using Xunit;

namespace MBW.HassMQTT.Tests;

public class DiscoveryModelCapabilityTests
{
    [Fact]
    public void Documented_explicit_null_fields_use_optional_contracts()
    {
        Assert.Equal(typeof(Optional<string>), typeof(IHasName).GetProperty(nameof(IHasName.Name))!.PropertyType);

        Type discoveryDocumentType = typeof(IHassDiscoveryDocument);
        Type[] namedModels = typeof(MqttSensor).Assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && discoveryDocumentType.IsAssignableFrom(type))
            .Where(type => typeof(IHasName).IsAssignableFrom(type))
            .ToArray();
        Assert.NotEmpty(namedModels);
        Assert.All(namedModels, type =>
            Assert.Equal(typeof(Optional<string>), type.GetProperty(nameof(IHasName.Name))!.PropertyType));

        (Type Model, string Property, Type Expected)[] optionalFields =
        {
            (typeof(MqttBinarySensor), nameof(MqttBinarySensor.DeviceClass), typeof(Optional<HassBinarySensorDeviceClass?>)),
            (typeof(MqttButton), nameof(MqttButton.DeviceClass), typeof(Optional<HassButtonDeviceClass?>)),
            (typeof(MqttCover), nameof(MqttCover.DeviceClass), typeof(Optional<string>)),
            (typeof(MqttCover), nameof(MqttCover.PayloadOpen), typeof(Optional<string>)),
            (typeof(MqttCover), nameof(MqttCover.PayloadClose), typeof(Optional<string>)),
            (typeof(MqttCover), nameof(MqttCover.PayloadStop), typeof(Optional<string>)),
            (typeof(MqttEvent), nameof(MqttEvent.DeviceClass), typeof(Optional<HassEventDeviceClass?>)),
            (typeof(MqttHumidifier), nameof(MqttHumidifier.DeviceClass), typeof(Optional<HumidifierDeviceClass?>)),
            (typeof(MqttNumber), nameof(MqttNumber.DeviceClass), typeof(Optional<HassNumberDeviceClass?>)),
            (typeof(MqttSensor), nameof(MqttSensor.DeviceClass), typeof(Optional<HassSensorDeviceClass?>)),
            (typeof(MqttSensor), nameof(MqttSensor.UnitOfMeasurement), typeof(Optional<string>)),
            (typeof(MqttSwitch), nameof(MqttSwitch.DeviceClass), typeof(Optional<HassSwitchDeviceClass?>)),
            (typeof(MqttUpdate), nameof(MqttUpdate.DeviceClass), typeof(Optional<HassUpdateDeviceClass?>)),
            (typeof(MqttValve), nameof(MqttValve.DeviceClass), typeof(Optional<HassValveDeviceClass?>)),
            (typeof(MqttValve), nameof(MqttValve.PayloadOpen), typeof(Optional<string>)),
            (typeof(MqttValve), nameof(MqttValve.PayloadClose), typeof(Optional<string>)),
        };

        Assert.All(optionalFields, field =>
            Assert.Equal(field.Expected, field.Model.GetProperty(field.Property)!.PropertyType));
        Assert.Equal(typeof(string), typeof(MqttDeviceDocument).GetProperty(nameof(MqttDeviceDocument.Name))!.PropertyType);
        Assert.Equal(typeof(string), typeof(MqttValve).GetProperty(nameof(MqttValve.PayloadStop))!.PropertyType);
    }

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
            [nameof(IHasColorTemperatureRange.ColorTempKelvin)] = typeof(IHasColorTemperatureRange),
            [nameof(IHasColorTemperatureRange.MaxKelvin)] = typeof(IHasColorTemperatureRange),
            [nameof(IHasColorTemperatureRange.MaxMireds)] = typeof(IHasColorTemperatureRange),
            [nameof(IHasColorTemperatureRange.MinKelvin)] = typeof(IHasColorTemperatureRange),
            [nameof(IHasColorTemperatureRange.MinMireds)] = typeof(IHasColorTemperatureRange),
            [nameof(IHasAvailabilityPayloads.PayloadAvailable)] = typeof(IHasAvailabilityPayloads),
            [nameof(IHasAvailabilityPayloads.PayloadNotAvailable)] = typeof(IHasAvailabilityPayloads),
            [nameof(IHasEncoding.Encoding)] = typeof(IHasEncoding),
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

    [Fact]
    public void TemporalFamiliesRequireCommandTopic()
    {
        var model = new MqttDate("homeassistant/date/example/config", "example");
        model.Device.Identifiers.Add("example");

        ValidationResult result = MqttDate.Validator.Validate(model);

        Assert.Contains(result.Errors, failure => failure.PropertyName == nameof(MqttDate.CommandTopic));
    }

    [Fact]
    public void ValvePayloadsAreRejectedWhenPositionIsReported()
    {
        var model = new MqttValve("homeassistant/valve/example/config", "example")
        {
            ReportsPosition = true,
            PayloadOpen = "OPEN",
        };
        model.Device.Identifiers.Add("example");

        ValidationResult result = MqttValve.Validator.Validate(model);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(MqttValve.PayloadOpen));

        model.PayloadOpen = null;
        result = MqttValve.Validator.Validate(model);

        Assert.DoesNotContain(result.Errors, error => error.PropertyName == nameof(MqttValve.PayloadOpen));
    }

    [Fact]
    public void Optional_enum_validation_accepts_null_and_rejects_invalid_values()
    {
        var model = new MqttBinarySensor("homeassistant/binary_sensor/example/config", "example")
        {
            StateTopic = "example/state",
            DeviceClass = null,
        };
        model.Device.Identifiers.Add("example");

        ValidationResult result = MqttBinarySensor.Validator.Validate(model);
        Assert.DoesNotContain(result.Errors, error => error.PropertyName == nameof(MqttBinarySensor.DeviceClass));

        model.DeviceClass = (HassBinarySensorDeviceClass)int.MaxValue;
        result = MqttBinarySensor.Validator.Validate(model);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(MqttBinarySensor.DeviceClass));
    }

    [Fact]
    public void SensorOptionsEnforceEnumSchemaRelationships()
    {
        var sensor = new MqttSensor("homeassistant/sensor/example/config", "sensor-example")
        {
            StateTopic = "example/sensor/state",
            DeviceClass = HassSensorDeviceClass.Temperature,
            Options = new List<string> { "hot", "cold" },
            StateClass = HassStateClass.Measurement,
            UnitOfMeasurement = "C",
        };
        sensor.Device.Identifiers.Add("example");

        ValidationResult result = MqttSensor.Validator.Validate(sensor);

        Assert.Contains(result.Errors, failure => failure.PropertyName == nameof(MqttSensor.DeviceClass));
        Assert.Contains(result.Errors, failure => failure.PropertyName == nameof(MqttSensor.StateClass));
        Assert.Contains(result.Errors, failure => failure.PropertyName == nameof(MqttSensor.UnitOfMeasurement));
    }

    [Fact]
    public void SharedColorTemperatureCapabilityValidatesBounds()
    {
        var model = new MqttLightJson("homeassistant/light/example/config", "json-light")
        {
            MinKelvin = 6500,
            MaxKelvin = 2000,
        };
        model.Device.Identifiers.Add("example");

        ValidationResult result = MqttLightJson.Validator.Validate(model);

        Assert.Contains(result.Errors, failure => failure.PropertyName.Contains(nameof(IHasColorTemperatureRange.MinKelvin)));
        Assert.Contains(result.Errors, failure => failure.PropertyName.Contains(nameof(IHasColorTemperatureRange.MaxKelvin)));
    }

    [Fact]
    public void RemovedDiscoveryPropertiesStayRemoved()
    {
        Assert.Null(typeof(MqttClimate).GetProperty("AuxCommandTopic"));
        Assert.Null(typeof(MqttClimate).GetProperty("AuxStateTemplate"));
        Assert.Null(typeof(MqttClimate).GetProperty("AuxStateTopic"));
        Assert.Null(typeof(MqttLightJson).GetProperty("ColorMode"));
        Assert.Null(typeof(MqttVacuum).GetProperty("Schema"));
    }

    [Fact]
    public void EventDeviceClassDoesNotExposeLegacySentinels()
    {
        Assert.DoesNotContain("Null", System.Enum.GetNames(typeof(HassEventDeviceClass)));
        Assert.DoesNotContain("DeviceClass", System.Enum.GetNames(typeof(HassEventDeviceClass)));
    }
}
