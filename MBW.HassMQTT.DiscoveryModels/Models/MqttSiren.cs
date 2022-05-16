#nullable enable
using System.Collections.Generic;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models;

/// <summary>
/// https://www.home-assistant.io/integrations/siren.mqtt/
///
/// The `mqtt` siren platform lets you control your MQTT enabled sirens and text based notification devices.
/// </summary>
/// <remarks>
/// In an ideal scenario, the MQTT device will have a `state_topic` to publish state changes. If these
/// messages are published with a `RETAIN` flag, the MQTT siren will receive an instant state update
/// after subscription, and will start with the correct state. Otherwise, the initial state of the
/// siren will be `false` / `off`.
///
/// When a `state_topic` is not available, the siren will work in optimistic mode.In this mode, the siren
/// will immediately change state after every command.Otherwise, the siren will wait for state confirmation
/// from the device (message from `state_topic`).
///
/// Optimistic mode can be forced, even if the `state_topic` is available.Try to enable it, if
/// experiencing incorrect operation.
/// </remarks>
[DeviceType(HassDeviceType.Siren)]
[PublicAPI]
public class MqttSiren : MqttSensorDiscoveryBase<MqttSiren, MqttSiren.MqttSirenValidator>, IHasUniqueId, IHasAvailability, IHasQos, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasRetain, IHasEntityCategory, IHasObjectId, IHasEncoding
{
    public MqttSiren(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
    {
    }

    /// <summary>
    /// A list of available tones the siren supports. When configured, this enables the support for setting a `tone` and enables the `tone` state attribute.
    /// </summary>
    public string? AvailableTones { get; set; }

    /// <summary>
    /// Defines a template to generate the payload to send to `command_topic`. The variable `value` will be assigned with the configured `payload_on` or `payload_off` setting. The siren turn on service parameters `tone`, `volume_level` or `duration` can be used as variables in the template. When operation in optimistic mode the corresponding state attributes will be set. Turn parameters will be filtered if a device misses the support.
    /// </summary>
    public string? CommandTemplate { get; set; }

    /// <summary>
    /// Defines a template to generate the payload to send to `command_topic` when the siren turn off service is called. By default `command_template` will be used as template for service turn off. The variable `value` will be assigned with the configured `payload_off` setting.
    /// </summary>
    public string? CommandOffTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to publish commands to change the siren state. Without command template a JSON payload is published. When the siren turn on service is called, the startup parameters will be added to the JSON payload. The `state` value of the JSON payload will be set to the the `payload_on` or `payload_off` configured payload.
    /// </summary>
    public string? CommandTopic { get; set; }

    /// <summary>
    /// The name to use when displaying this siren.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The payload that represents the available state.
    /// </summary>
    public string? PayloadAvailable { get; set; }

    /// <summary>
    /// The payload that represents the unavailable state.
    /// </summary>
    public string? PayloadNotAvailable { get; set; }

    /// <summary>
    /// The payload that represents `off` state. If specified, will be used for both comparing to the value in the `state_topic` (see `value_template` and `state_off` for details) and sending as `off` command to the `command_topic`.
    /// </summary>
    public string? PayloadOff { get; set; }

    /// <summary>
    /// The payload that represents `on` state. If specified, will be used for both comparing to the value in the `state_topic` (see `value_template` and `state_on`  for details) and sending as `on` command to the `command_topic`.
    /// </summary>
    public string? PayloadOn { get; set; }

    /// <summary>
    /// The payload that represents the `off` state. Used when value that represents `off` state in the `state_topic` is different from value that should be sent to the `command_topic` to turn the device `off`.
    /// </summary>
    public string? StateOff { get; set; }

    /// <summary>
    /// The payload that represents the `on` state. Used when value that represents `on` state in the `state_topic` is different from value that should be sent to the `command_topic` to turn the device `on`.
    /// </summary>
    public string? StateOn { get; set; }

    /// <summary>
    /// The MQTT topic subscribed to receive state updates. When a JSON payload is detected, the `state` value of the JSON payload should supply the `payload_on` or `payload_off` defined payload to turn the siren on or off. Additional the state attributes `duration`, `tone` and `volume_level` can be updated. Use `value_template` to update render custom payload to a compliant JSON payload. Attributes will only be set if the function is supported by the device and a valid value is supplied. The initial state will be `unknown`. The state will be reset to `unknown` if a `None` payload or `null` JSON value is received as a state update.
    /// </summary>
    public string? StateTopic { get; set; }

    /// <summary>
    /// Defines a template to extract device's state from the `state_topic`. To determine the siren's state result of this template will be compared to `state_on` and `state_off`. Alternatively `value_template` can be used to render to a valid JSON payload.
    /// </summary>
    public string? StateValueTemplate { get; set; }

    /// <summary>
    /// Set to `true` if the MQTT siren supports the `duration` service turn on parameter and enables the `duration` state attribute.
    /// </summary>
    public bool? SupportDuration { get; set; }

    /// <summary>
    /// Set to `true` if the MQTT siren supports the `volume_set` service turn on parameter and enables the `volume_level` state attribute.
    /// </summary>
    public bool? SupportVolumeSet { get; set; }

    public string? UniqueId { get; set; }
    public IList<AvailabilityModel>? Availability { get; set; }
    public AvailabilityMode? AvailabilityMode { get; set; }
    public MqttQosLevel? Qos { get; set; }
    public string? JsonAttributesTemplate { get; set; }
    public string? JsonAttributesTopic { get; set; }
    public string? Icon { get; set; }
    public bool? EnabledByDefault { get; set; }
    public bool? Retain { get; set; }
    public EntityCategory? EntityCategory { get; set; }
    public string? ObjectId { get; set; }
    public string? Encoding { get; set; }

    public class MqttSirenValidator : MqttSensorDiscoveryBaseValidator<MqttSiren>
    {
        public MqttSirenValidator()
        {
            TopicAndTemplate(s => s.CommandTopic, s => s.CommandTemplate, s => s.CommandOffTemplate);
            TopicAndTemplate(s => s.StateTopic, s => s.StateValueTemplate);
        }
    }
}