#nullable enable

using System.Collections.Generic;
using System.Text.Json.Serialization;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Device;
using MBW.HassMQTT.DiscoveryModels.Metadata;
using MBW.HassMQTT.DiscoveryModels.Models;
using MBW.HassMQTT.Internal;

namespace MBW.HassMQTT.Serialization;

[JsonSourceGenerationOptions(
    GenerationMode = JsonSourceGenerationMode.Default,
    PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault)]
[JsonSerializable(typeof(MqttAlarmControlPanel))]
[JsonSerializable(typeof(MqttBinarySensor))]
[JsonSerializable(typeof(MqttButton))]
[JsonSerializable(typeof(MqttCamera))]
[JsonSerializable(typeof(MqttClimate))]
[JsonSerializable(typeof(MqttCover))]
[JsonSerializable(typeof(MqttDate))]
[JsonSerializable(typeof(MqttDateTime))]
[JsonSerializable(typeof(MqttDeviceTracker))]
[JsonSerializable(typeof(MqttDeviceTrigger))]
[JsonSerializable(typeof(MqttEvent))]
[JsonSerializable(typeof(MqttFan))]
[JsonSerializable(typeof(MqttHumidifier))]
[JsonSerializable(typeof(MqttImage))]
[JsonSerializable(typeof(MqttLawnMower))]
[JsonSerializable(typeof(MqttLightDefault))]
[JsonSerializable(typeof(MqttLightJson))]
[JsonSerializable(typeof(MqttLightTemplate))]
[JsonSerializable(typeof(MqttLock))]
[JsonSerializable(typeof(MqttNotify))]
[JsonSerializable(typeof(MqttNumber))]
[JsonSerializable(typeof(MqttScene))]
[JsonSerializable(typeof(MqttSelect))]
[JsonSerializable(typeof(MqttSensor))]
[JsonSerializable(typeof(MqttSiren))]
[JsonSerializable(typeof(MqttSwitch))]
[JsonSerializable(typeof(MqttTagScanner))]
[JsonSerializable(typeof(MqttText))]
[JsonSerializable(typeof(MqttTime))]
[JsonSerializable(typeof(MqttUpdate))]
[JsonSerializable(typeof(MqttVacuum))]
#pragma warning disable CS0618
[JsonSerializable(typeof(MqttVacuumState))]
#pragma warning restore CS0618
[JsonSerializable(typeof(MqttValve))]
[JsonSerializable(typeof(MqttWaterHeater))]
[JsonSerializable(typeof(MqttDeviceDocument))]
[JsonSerializable(typeof(AvailabilityModel))]
[JsonSerializable(typeof(ConnectionInfo))]
[JsonSerializable(typeof(MessageExpiryInterval))]
[JsonSerializable(typeof(Dictionary<string, object>))]
[JsonSerializable(typeof(object))]
internal partial class HassJsonSerializerContext : JsonSerializerContext;
