#nullable enable

using System;
using System.Collections.Generic;
using FluentValidation;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models;

/// <summary>Defines an MQTT water heater entity. See https://www.home-assistant.io/integrations/water_heater.mqtt</summary>
[DeviceType(HassDeviceType.WaterHeater)]
[PublicAPI]
public class MqttWaterHeater : MqttSensorDiscoveryBase<MqttWaterHeater, MqttWaterHeater.MqttWaterHeaterValidator>,
    IHasAvailability, IHasAvailabilityPayloads, IHasDefaultEntityId, IHasEnabledByDefault, IHasEncoding,
    IHasEntityCategory, IHasEntityPicture, IHasGroup, IHasIcon, IHasJsonAttributes, IHasMessageExpiryInterval,
    IHasName, IHasOptimistic, IHasQos, IHasRetain, IHasUniqueId, IHasVisibleByDefault
{
    public MqttWaterHeater(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId) { }

    public string? CurrentTemperatureTemplate { get; set; }
    public string? CurrentTemperatureTopic { get; set; }
    public int? Initial { get; set; }
    public float? MaxTemp { get; set; }
    public float? MinTemp { get; set; }
    public string? ModeCommandTemplate { get; set; }
    public string? ModeCommandTopic { get; set; }
    public string? ModeStateTemplate { get; set; }
    public string? ModeStateTopic { get; set; }
    public IList<string>? Modes { get; set; }
    public string? PayloadOff { get; set; }
    public string? PayloadOn { get; set; }
    public string? PowerCommandTemplate { get; set; }
    public string? PowerCommandTopic { get; set; }
    public float? Precision { get; set; }
    public string? TemperatureCommandTemplate { get; set; }
    public string? TemperatureCommandTopic { get; set; }
    public string? TemperatureStateTemplate { get; set; }
    public string? TemperatureStateTopic { get; set; }
    public HassTemperatureUnit? TemperatureUnit { get; set; }
    /// <summary>Legacy template used to extract the target temperature.</summary>
    public string? ValueTemplate { get; set; }

    public IList<AvailabilityModel>? Availability { get; set; }
    public AvailabilityMode? AvailabilityMode { get; set; }
    public string? AvailabilityTemplate { get; set; }
    public string? AvailabilityTopic { get; set; }
    public string? PayloadAvailable { get; set; }
    public string? PayloadNotAvailable { get; set; }
    public string? DefaultEntityId { get; set; }
    public bool? EnabledByDefault { get; set; }
    public string? Encoding { get; set; }
    public EntityCategory? EntityCategory { get; set; }
    public string? EntityPicture { get; set; }
    public IList<string>? Group { get; set; }
    public string? Icon { get; set; }
    public string? JsonAttributesTemplate { get; set; }
    public string? JsonAttributesTopic { get; set; }
    public MessageExpiryInterval? MessageExpiryInterval { get; set; }
    public string? Name { get; set; }
    public bool? Optimistic { get; set; }
    public MqttQosLevel? Qos { get; set; }
    public bool? Retain { get; set; }
    public string? UniqueId { get; set; }
    public bool? VisibleByDefault { get; set; }

    public class MqttWaterHeaterValidator : MqttSensorDiscoveryBaseValidator<MqttWaterHeater>
    {
        public MqttWaterHeaterValidator()
        {
            TopicAndTemplate(x => x.CurrentTemperatureTopic, x => x.CurrentTemperatureTemplate);
            TopicAndTemplate(x => x.ModeCommandTopic, x => x.ModeCommandTemplate);
            TopicAndTemplate(x => x.ModeStateTopic, x => x.ModeStateTemplate);
            TopicAndTemplate(x => x.PowerCommandTopic, x => x.PowerCommandTemplate);
            TopicAndTemplate(x => x.TemperatureCommandTopic, x => x.TemperatureCommandTemplate);
            TopicAndTemplate(x => x.TemperatureStateTopic, x => x.TemperatureStateTemplate);

            RuleFor(x => x.Precision)
                .Must(value => Math.Abs(value!.Value - 0.1f) < float.Epsilon ||
                               Math.Abs(value.Value - 0.5f) < float.Epsilon ||
                               Math.Abs(value.Value - 1f) < float.Epsilon)
                .When(x => x.Precision.HasValue);
        }
    }
}
