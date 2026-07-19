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

    /// <summary>A template used to render the current temperature received on <see cref="CurrentTemperatureTopic" />.</summary>
    public string? CurrentTemperatureTemplate { get; set; }
    /// <summary>The MQTT topic subscribed to receive the current temperature. A <c>None</c> payload resets the value and an empty payload is ignored.</summary>
    public string? CurrentTemperatureTopic { get; set; }
    /// <summary>The initial target temperature. The documented default is 43.3 °C or 110 °F depending on the configured unit.</summary>
    public int? Initial { get; set; }
    /// <summary>The maximum available temperature set point. The documented default is 60 °C or 140 °F depending on the configured unit.</summary>
    public float? MaxTemp { get; set; }
    /// <summary>The minimum available temperature set point. The documented default is 43.3 °C or 110 °F depending on the configured unit.</summary>
    public float? MinTemp { get; set; }
    /// <summary>A template used to render the value published to <see cref="ModeCommandTopic" />.</summary>
    public string? ModeCommandTemplate { get; set; }
    /// <summary>The MQTT topic to which water-heater operation-mode commands are published.</summary>
    public string? ModeCommandTopic { get; set; }
    /// <summary>A template used to render the operation mode received on <see cref="ModeStateTopic" />.</summary>
    public string? ModeStateTemplate { get; set; }
    /// <summary>The MQTT topic subscribed to receive operation-mode updates. A <c>None</c> payload resets the mode to unknown and an empty payload is ignored.</summary>
    public string? ModeStateTopic { get; set; }
    /// <summary>The supported operation modes, which must be a subset of <c>off</c>, <c>eco</c>, <c>electric</c>, <c>gas</c>, <c>heat_pump</c>, <c>high_demand</c>, and <c>performance</c>.</summary>
    public IList<string>? Modes { get; set; }
    /// <summary>The payload representing the disabled state. The documented default is <c>OFF</c>.</summary>
    public string? PayloadOff { get; set; }
    /// <summary>The payload representing the enabled state. The documented default is <c>ON</c>.</summary>
    public string? PayloadOn { get; set; }
    /// <summary>A template used to render the power command published to <see cref="PowerCommandTopic" />; its value is <see cref="PayloadOn" /> or <see cref="PayloadOff" />.</summary>
    public string? PowerCommandTemplate { get; set; }
    /// <summary>The MQTT topic to which water-heater power commands are published.</summary>
    public string? PowerCommandTopic { get; set; }
    /// <summary>The desired temperature precision. Supported values are <c>0.1</c>, <c>0.5</c>, and <c>1.0</c>.</summary>
    public float? Precision { get; set; }
    /// <summary>A template used to render the target temperature published to <see cref="TemperatureCommandTopic" />.</summary>
    public string? TemperatureCommandTemplate { get; set; }
    /// <summary>The MQTT topic to which target-temperature commands are published.</summary>
    public string? TemperatureCommandTopic { get; set; }
    /// <summary>A template used to render the target temperature received on <see cref="TemperatureStateTopic" />.</summary>
    public string? TemperatureStateTemplate { get; set; }
    /// <summary>The MQTT topic subscribed to receive target-temperature updates. A <c>None</c> payload resets the set point and an empty payload is ignored.</summary>
    public string? TemperatureStateTopic { get; set; }
    /// <summary>The temperature unit used by the device, either <c>C</c> or <c>F</c>. When omitted, Home Assistant uses its system temperature unit.</summary>
    public HassTemperatureUnit? TemperatureUnit { get; set; }
    /// <summary>A template used to extract the target temperature from messages received on <see cref="TemperatureStateTopic" />.</summary>
    public string? ValueTemplate { get; set; }

    /// <inheritdoc />
    public IList<AvailabilityModel>? Availability { get; set; }
    /// <inheritdoc />
    public AvailabilityMode? AvailabilityMode { get; set; }
    /// <inheritdoc />
    public string? AvailabilityTemplate { get; set; }
    /// <inheritdoc />
    public string? AvailabilityTopic { get; set; }
    /// <inheritdoc />
    public string? PayloadAvailable { get; set; }
    /// <inheritdoc />
    public string? PayloadNotAvailable { get; set; }
    /// <inheritdoc />
    public string? DefaultEntityId { get; set; }
    /// <inheritdoc />
    public bool? EnabledByDefault { get; set; }
    /// <inheritdoc />
    public string? Encoding { get; set; }
    /// <inheritdoc />
    public EntityCategory? EntityCategory { get; set; }
    /// <inheritdoc />
    public string? EntityPicture { get; set; }
    /// <inheritdoc />
    public IList<string>? Group { get; set; }
    /// <inheritdoc />
    public string? Icon { get; set; }
    /// <inheritdoc />
    public string? JsonAttributesTemplate { get; set; }
    /// <inheritdoc />
    public string? JsonAttributesTopic { get; set; }
    /// <inheritdoc />
    public MessageExpiryInterval? MessageExpiryInterval { get; set; }
    /// <inheritdoc />
    public Optional<string?> Name { get; set; }
    /// <inheritdoc />
    public bool? Optimistic { get; set; }
    /// <inheritdoc />
    public MqttQosLevel? Qos { get; set; }
    /// <inheritdoc />
    public bool? Retain { get; set; }
    /// <inheritdoc />
    public string? UniqueId { get; set; }
    /// <inheritdoc />
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
