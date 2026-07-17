using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
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
            [nameof(IHasColorTemperatureRange.ColorTempKelvin)] = typeof(IHasColorTemperatureRange),
            [nameof(IHasColorTemperatureRange.MaxKelvin)] = typeof(IHasColorTemperatureRange),
            [nameof(IHasColorTemperatureRange.MaxMireds)] = typeof(IHasColorTemperatureRange),
            [nameof(IHasColorTemperatureRange.MinKelvin)] = typeof(IHasColorTemperatureRange),
            [nameof(IHasColorTemperatureRange.MinMireds)] = typeof(IHasColorTemperatureRange),
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

    [Fact]
    public void TemporalFamiliesShareSchemaAndSerializeFamilySpecificFields()
    {
        var date = new MqttDate("homeassistant/date/example/config", "date-example")
        {
            CommandTopic = "example/date/set",
            StateTopic = "example/date/state",
        };
        var dateTime = new MqttDateTime("homeassistant/datetime/example/config", "datetime-example")
        {
            CommandTopic = "example/datetime/set",
            Timezone = "Europe/Copenhagen",
        };
        var time = new MqttTime("homeassistant/time/example/config", "time-example")
        {
            CommandTopic = "example/time/set",
            Icon = "mdi:clock",
        };

        Assert.IsAssignableFrom<IHasAvailability>(date);
        Assert.IsAssignableFrom<IHasDefaultEntityId>(dateTime);
        Assert.IsAssignableFrom<IHasMessageExpiryInterval>(time);

        JObject dateJson = JObject.FromObject(date, CustomJsonSerializer.Serializer);
        JObject dateTimeJson = JObject.FromObject(dateTime, CustomJsonSerializer.Serializer);
        JObject timeJson = JObject.FromObject(time, CustomJsonSerializer.Serializer);

        Assert.Equal("example/date/set", dateJson.Value<string>("command_topic"));
        Assert.Equal("example/date/state", dateJson.Value<string>("state_topic"));
        Assert.Equal("Europe/Copenhagen", dateTimeJson.Value<string>("timezone"));
        Assert.Equal("mdi:clock", timeJson.Value<string>("icon"));
        Assert.Null(dateJson["platform"]);
        Assert.Null(dateTimeJson["platform"]);
        Assert.Null(timeJson["platform"]);
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
    public void NotifyUsesSharedCapabilitiesAndSerializesCommandSchema()
    {
        var model = new MqttNotify("homeassistant/notify/example/config", "notify-example")
        {
            CommandTopic = "example/notify/send",
            CommandTemplate = "{{ message }}",
            PayloadAvailable = "ready",
            MessageExpiryInterval = new MessageExpiryInterval { Seconds = 30 },
        };

        Assert.IsAssignableFrom<IHasAvailabilityPayloads>(model);
        Assert.IsAssignableFrom<IHasMessageExpiryInterval>(model);

        JObject json = JObject.FromObject(model, CustomJsonSerializer.Serializer);

        Assert.Equal("example/notify/send", json.Value<string>("command_topic"));
        Assert.Equal("{{ message }}", json.Value<string>("command_template"));
        Assert.Equal("ready", json.Value<string>("payload_available"));
        Assert.Equal(30, json["message_expiry_interval"]!.Value<int>("seconds"));
    }

    [Fact]
    public void LawnMowerSerializesActivityAndActionTopics()
    {
        var model = new MqttLawnMower("homeassistant/lawn_mower/example/config", "mower-example")
        {
            ActivityStateTopic = "example/mower/activity",
            ActivityValueTemplate = "{{ value_json.activity }}",
            DockCommandTopic = "example/mower/dock",
            PauseCommandTopic = "example/mower/pause",
            StartMowingCommandTopic = "example/mower/start",
            Optimistic = false,
        };

        JObject json = JObject.FromObject(model, CustomJsonSerializer.Serializer);

        Assert.Equal("example/mower/activity", json.Value<string>("activity_state_topic"));
        Assert.Equal("{{ value_json.activity }}", json.Value<string>("activity_value_template"));
        Assert.Equal("example/mower/dock", json.Value<string>("dock_command_topic"));
        Assert.Equal("example/mower/pause", json.Value<string>("pause_command_topic"));
        Assert.Equal("example/mower/start", json.Value<string>("start_mowing_command_topic"));
        Assert.False(json.Value<bool>("optimistic"));
    }

    [Fact]
    public void ValveSerializesPositionAndStateSchema()
    {
        var model = new MqttValve("homeassistant/valve/example/config", "valve-example")
        {
            DeviceClass = HassValveDeviceClass.Water,
            CommandTopic = "example/valve/set",
            StateTopic = "example/valve/state",
            ReportsPosition = true,
            PositionClosed = 0,
            PositionOpen = 100,
            Group = new List<string> { "irrigation" },
        };

        Assert.IsAssignableFrom<IHasGroup>(model);
        Assert.IsAssignableFrom<IHasAvailabilityPayloads>(model);

        JObject json = JObject.FromObject(model, CustomJsonSerializer.Serializer);
        Assert.True(json.Value<bool>("reports_position"));
        Assert.Equal("water", json.Value<string>("device_class"));
        Assert.Equal(0, json.Value<int>("position_closed"));
        Assert.Equal(100, json.Value<int>("position_open"));
        Assert.Equal("example/valve/state", json.Value<string>("state_topic"));

        model.PayloadOpen = "OPEN";
        ValidationResult invalid = MqttValve.Validator.Validate(model);
        Assert.False(invalid.IsValid);
        Assert.Contains(invalid.Errors, error => error.PropertyName == nameof(MqttValve.PayloadOpen));
    }

    [Fact]
    public void WaterHeaterSerializesTemperatureAndModeSchema()
    {
        var model = new MqttWaterHeater("homeassistant/water_heater/example/config", "heater-example")
        {
            CurrentTemperatureTopic = "example/heater/current",
            TemperatureCommandTopic = "example/heater/temperature/set",
            TemperatureStateTopic = "example/heater/temperature/state",
            ModeCommandTopic = "example/heater/mode/set",
            Modes = new List<string> { "off", "eco", "electric" },
            MinTemp = 40,
            MaxTemp = 65,
            Precision = 0.5f,
            TemperatureUnit = MBW.HassMQTT.DiscoveryModels.Enum.HassTemperatureUnit.Celsius,
        };

        JObject json = JObject.FromObject(model, CustomJsonSerializer.Serializer);
        Assert.Equal("example/heater/current", json.Value<string>("current_temperature_topic"));
        Assert.Equal("example/heater/temperature/set", json.Value<string>("temperature_command_topic"));
        Assert.Equal(new[] { "off", "eco", "electric" }, json["modes"]!.Values<string>());
        Assert.Equal(0.5f, json.Value<float>("precision"));
        Assert.Equal("C", json.Value<string>("temperature_unit"));
    }

    [Fact]
    public void AlarmControlPanelSerializesSupportedFeaturesAsDocumentedValues()
    {
        var model = new MqttAlarmControlPanel("homeassistant/alarm_control_panel/example/config", "alarm-example")
        {
            CommandTopic = "example/alarm/set",
            StateTopic = "example/alarm/state",
            SupportedFeatures = new List<HassAlarmControlPanelFeature>
            {
                HassAlarmControlPanelFeature.ArmHome,
                HassAlarmControlPanelFeature.ArmCustomBypass,
                HassAlarmControlPanelFeature.Trigger,
            },
        };

        JObject json = JObject.FromObject(model, CustomJsonSerializer.Serializer);

        Assert.Equal(
            new[] { "arm_home", "arm_custom_bypass", "trigger" },
            json["supported_features"]!.Values<string>());
    }

    [Fact]
    public void ClimateUsesHorizontalSwingSchemaAndRemovesAuxiliaryHeatSchema()
    {
        var model = new MqttClimate("homeassistant/climate/example/config", "climate-example")
        {
            SwingHorizontalModeCommandTopic = "example/climate/swing-horizontal/set",
            SwingHorizontalModeCommandTemplate = "{{ value }}",
            SwingHorizontalModeStateTopic = "example/climate/swing-horizontal/state",
            SwingHorizontalModeStateTemplate = "{{ value_json.swing_horizontal }}",
            SwingHorizontalModes = new List<string> { "on", "off" },
            Initial = 21.5f,
            MinHumidity = 30.5f,
            MaxHumidity = 98.5f,
        };

        JObject json = JObject.FromObject(model, CustomJsonSerializer.Serializer);

        Assert.Equal("example/climate/swing-horizontal/set", json.Value<string>("swing_horizontal_mode_command_topic"));
        Assert.Equal("{{ value_json.swing_horizontal }}", json.Value<string>("swing_horizontal_mode_state_template"));
        Assert.Equal(new[] { "on", "off" }, json["swing_horizontal_modes"]!.Values<string>());
        Assert.Equal(21.5f, json.Value<float>("initial"));
        Assert.Equal(30.5f, json.Value<float>("min_humidity"));
        Assert.Equal(98.5f, json.Value<float>("max_humidity"));
        Assert.Null(typeof(MqttClimate).GetProperty("AuxCommandTopic"));
        Assert.Null(typeof(MqttClimate).GetProperty("AuxStateTemplate"));
        Assert.Null(typeof(MqttClimate).GetProperty("AuxStateTopic"));
    }

    [Fact]
    public void CompactFamilyAdditionsSerializeWithDocumentedNames()
    {
        var cover = new MqttCover("homeassistant/cover/example/config", "cover-example")
        {
            PayloadStopTilt = "HALT_TILT",
        };
        var mqttLock = new MqttLock("homeassistant/lock/example/config", "lock-example")
        {
            PayloadReset = "None",
        };
        var mqttSwitch = new MqttSwitch("homeassistant/switch/example/config", "switch-example")
        {
            CommandTopic = "example/switch/set",
            CommandTemplate = "{{ value | lower }}",
        };
        var sensor = new MqttSensor("homeassistant/sensor/example/config", "sensor-example")
        {
            StateTopic = "example/sensor/state",
            DeviceClass = HassSensorDeviceClass.Enum,
            Options = new List<string> { "auto", "manual" },
        };

        Assert.Equal("HALT_TILT", JObject.FromObject(cover, CustomJsonSerializer.Serializer).Value<string>("payload_stop_tilt"));
        Assert.Equal("None", JObject.FromObject(mqttLock, CustomJsonSerializer.Serializer).Value<string>("payload_reset"));
        Assert.Equal("{{ value | lower }}", JObject.FromObject(mqttSwitch, CustomJsonSerializer.Serializer).Value<string>("command_template"));

        JObject sensorJson = JObject.FromObject(sensor, CustomJsonSerializer.Serializer);
        Assert.Equal("enum", sensorJson.Value<string>("device_class"));
        Assert.Equal(new[] { "auto", "manual" }, sensorJson["options"]!.Values<string>());
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
    public void LightSchemasShareKelvinColorTemperatureCapability()
    {
        IHassDiscoveryDocument[] models =
        {
            new MqttLightDefault("homeassistant/light/default/config", "default-light"),
            new MqttLightJson("homeassistant/light/json/config", "json-light"),
            new MqttLightTemplate("homeassistant/light/template/config", "template-light"),
        };

        foreach (IHassDiscoveryDocument model in models)
        {
            IHasColorTemperatureRange colorTemperature = Assert.IsAssignableFrom<IHasColorTemperatureRange>(model);
            colorTemperature.ColorTempKelvin = true;
            colorTemperature.MinKelvin = 2000;
            colorTemperature.MaxKelvin = 6535;

            JObject json = JObject.FromObject(model, CustomJsonSerializer.Serializer);
            Assert.True(json.Value<bool>("color_temp_kelvin"));
            Assert.Equal(2000, json.Value<int>("min_kelvin"));
            Assert.Equal(6535, json.Value<int>("max_kelvin"));
        }
    }

    [Fact]
    public void JsonLightUsesCurrentFeatureFlagsAndHasNoLegacyColorModeFlag()
    {
        var model = new MqttLightJson("homeassistant/light/example/config", "json-light")
        {
            Flash = true,
            Transition = true,
            SupportedColorModes = new List<string> { "rgbww", "white" },
        };

        JObject json = JObject.FromObject(model, CustomJsonSerializer.Serializer);
        Assert.True(json.Value<bool>("flash"));
        Assert.True(json.Value<bool>("transition"));
        Assert.Equal(new[] { "rgbww", "white" }, json["supported_color_modes"]!.Values<string>());
        Assert.Null(typeof(MqttLightJson).GetProperty("ColorMode"));
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
    public void UpdateSerializesDirectVersionAndProgressFields()
    {
        var model = new MqttUpdate("homeassistant/update/example/config", "update-example")
        {
            DisplayPrecision = 1,
            InstalledVersion = "1.0.0",
            LatestVersion = "1.1.0",
            InProgress = true,
            UpdatePercentage = 42.5f,
        };

        JObject json = JObject.FromObject(model, CustomJsonSerializer.Serializer);
        Assert.Equal(1, json.Value<int>("display_precision"));
        Assert.Equal("1.0.0", json.Value<string>("installed_version"));
        Assert.Equal("1.1.0", json.Value<string>("latest_version"));
        Assert.True(json.Value<bool>("in_progress"));
        Assert.Equal(42.5f, json.Value<float>("update_percentage"));
    }

    [Fact]
    public void VacuumUsesCurrentSegmentAndFeatureSchema()
    {
        var model = new MqttVacuum("homeassistant/vacuum/example/config", "vacuum-example")
        {
            CleanSegmentsCommandTopic = "example/vacuum/segments/set",
            CleanSegmentsCommandTemplate = "{{ value | to_json }}",
            Encoding = "utf-8",
            SupportedFeatures = new List<HassVacuumFeature>
            {
                HassVacuumFeature.Start,
                HassVacuumFeature.ReturnHome,
                HassVacuumFeature.CleanSpot,
            },
        };

        Assert.IsAssignableFrom<IHasEncoding>(model);
        JObject json = JObject.FromObject(model, CustomJsonSerializer.Serializer);
        Assert.Equal("example/vacuum/segments/set", json.Value<string>("clean_segments_command_topic"));
        Assert.Equal("{{ value | to_json }}", json.Value<string>("clean_segments_command_template"));
        Assert.Equal(new[] { "start", "return_home", "clean_spot" }, json["supported_features"]!.Values<string>());
        Assert.Null(typeof(MqttVacuum).GetProperty("Schema"));
    }

    [Fact]
    public void ExpandedDeviceClassesSerializeToCurrentWireValues()
    {
        var binarySensor = new MqttBinarySensor("homeassistant/binary_sensor/example/config", "binary-example")
        {
            StateTopic = "example/binary/state",
            DeviceClass = HassBinarySensorDeviceClass.Tamper,
        };
        var button = new MqttButton("homeassistant/button/example/config", "button-example")
        {
            DeviceClass = HassButtonDeviceClass.Identify,
        };
        var mqttEvent = new MqttEvent("homeassistant/event/example/config", "event-example")
        {
            DeviceClass = HassEventDeviceClass.Doorbell,
        };
        var number = new MqttNumber("homeassistant/number/example/config", "number-example")
        {
            DeviceClass = HassNumberDeviceClass.BloodGlucoseConcentration,
        };
        var sensor = new MqttSensor("homeassistant/sensor/example/config", "sensor-example")
        {
            StateTopic = "example/sensor/state",
            DeviceClass = HassSensorDeviceClass.VolumeFlowRate,
        };

        Assert.Equal("tamper", JObject.FromObject(binarySensor, CustomJsonSerializer.Serializer).Value<string>("device_class"));
        Assert.Equal("identify", JObject.FromObject(button, CustomJsonSerializer.Serializer).Value<string>("device_class"));
        Assert.Equal("doorbell", JObject.FromObject(mqttEvent, CustomJsonSerializer.Serializer).Value<string>("device_class"));
        Assert.Equal("blood_glucose_concentration", JObject.FromObject(number, CustomJsonSerializer.Serializer).Value<string>("device_class"));
        Assert.Equal("volume_flow_rate", JObject.FromObject(sensor, CustomJsonSerializer.Serializer).Value<string>("device_class"));
        Assert.DoesNotContain("Null", System.Enum.GetNames(typeof(HassEventDeviceClass)));
        Assert.DoesNotContain("DeviceClass", System.Enum.GetNames(typeof(HassEventDeviceClass)));
    }
}
