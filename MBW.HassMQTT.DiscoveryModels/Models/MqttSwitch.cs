#nullable enable
using System.Collections.Generic;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models;

/// <summary>
/// https://www.home-assistant.io/integrations/switch.mqtt
///
/// The mqtt switch platform lets you control your MQTT enabled switches.
/// </summary>
/// <remarks>
/// In an ideal scenario, the MQTT device will have a state_topic to publish state changes. If these messages
/// are published with a RETAIN flag, the MQTT switch will receive an instant state update after subscription,
/// and will start with the correct state. Otherwise, the initial state of the switch will be `unknown`. A MQTT
/// device can reset the current state to `unknown` using a `None` payload.
/// 
/// When a state_topic is not available, the switch will work in optimistic mode.In this mode, the switch will
/// immediately change state after every command.Otherwise, the switch will wait for state confirmation from the
/// device (message from state_topic). The initial state is set to `False` / `off` in optimistic mode.
/// 
/// Optimistic mode can be forced, even if the state_topic is available.Try to enable it, if experiencing incorrect
/// switch operation.
/// </remarks>
[DeviceType(HassDeviceType.Switch)]
[PublicAPI]
public class MqttSwitch : MqttSensorDiscoveryBase<MqttSwitch, MqttSwitch.MqttSwitchValidator>, IHasUniqueId,
    IHasAvailability, IHasQos, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasRetain, IHasEntityCategory,
    IHasObjectId, IHasEncoding, IHasName, IHasOptimistic
{
    public MqttSwitch(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
    {
    }

    /// <summary>
    /// The MQTT topic to publish commands to change the switch state.
    /// </summary>
    public string? CommandTopic { get; set; }

    /// <summary>
    /// The [type/class](/integrations/switch/#device-class) of the switch to set the icon in the frontend.
    /// See https://www.home-assistant.io/integrations/switch/#device-class
    /// </summary>
    /// <remarks>Default value: 'None'</remarks>
    public HassSwitchDeviceClass? DeviceClass { get; set; }

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
    /// The MQTT topic subscribed to receive state updates.
    /// </summary>
    public string? StateTopic { get; set; }

    /// <summary>
    /// Defines a [template](/docs/configuration/templating/#using-templates-with-the-mqtt-integration) to extract device's state from the `state_topic`. To determine the switches's state result of this template will be compared to `state_on` and `state_off`.
    /// </summary>
    public string? ValueTemplate { get; set; }

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
    public string? Name { get; set; }
    public bool? Optimistic { get; set; }

    public class MqttSwitchValidator : MqttSensorDiscoveryBaseValidator<MqttSwitch>
    {
        public MqttSwitchValidator()
        {
            TopicAndTemplate(s => s.StateTopic, s => s.ValueTemplate);
        }
    }
}