#nullable enable
using System.Collections.Generic;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models;

/// <summary>
/// https://www.home-assistant.io/integrations/fan.mqtt/
///
/// The mqtt fan platform lets you control your MQTT enabled fans.
/// </summary>
/// <remarks>
/// In an ideal scenario, the MQTT device will have a state_topic to publish state changes.
/// If these messages are published with a RETAIN flag, the MQTT fan will receive an instant
/// state update after subscription and will start with the correct state. Otherwise, the initial
/// state of the fan will be unknown`. A MQTT device can reset the current state to `unknown`
/// using a `None` payload.
/// 
/// When a state_topic is not available, the fan will work in optimistic mode.In this mode, the
/// fan will immediately change state after every command.Otherwise, the fan will wait for state
/// confirmation from the device (message from state_topic). The initial state is set to `False`
/// / `off` in optimistic mode.
/// 
/// Optimistic mode can be forced even if a state_topic is available. Try to enable it if you are
/// experiencing incorrect fan operation.
/// </remarks>
[DeviceType(HassDeviceType.Fan)]
[PublicAPI]
public class MqttFan : MqttSensorDiscoveryBase<MqttFan, MqttFan.MqttFanValidator>, IHasUniqueId, IHasAvailability, IHasQos, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasRetain, IHasEntityCategory, IHasObjectId, IHasEncoding
{
    public MqttFan(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
    {
    }

    /// <summary>
    /// Defines a template to generate the payload to send to command_topic.
    /// </summary>
    public string? CommandTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to publish commands to change the fan state.
    /// </summary>
    public string CommandTopic { get; set; }

    /// <summary>
    /// The name of the fan.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Flag that defines if lock works in optimistic mode
    /// </summary>
    public bool? Optimistic { get; set; }

    /// <summary>
    /// Defines a template to generate the payload to send to oscillation_command_topic.
    /// </summary>
    public string? OscillationCommandTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to publish commands to change the oscillation state.
    /// </summary>
    public string? OscillationCommandTopic { get; set; }

    /// <summary>
    /// The MQTT topic subscribed to receive oscillation state updates.
    /// </summary>
    public string OscillationStateTopic { get; set; }

    /// <summary>
    /// Defines a [template](/docs/configuration/templating/#using-templates-with-the-mqtt-integration) to extract a value from the oscillation.
    /// </summary>
    public string? OscillationValueTemplate { get; set; }

    /// <summary>
    /// The payload that represents the stop state.
    /// </summary>
    public string? PayloadOff { get; set; }

    /// <summary>
    /// The payload that represents the running state.
    /// </summary>
    public string? PayloadOn { get; set; }

    /// <summary>
    /// The payload that represents the oscillation off state.
    /// </summary>
    public string? PayloadOscillationOff { get; set; }

    /// <summary>
    /// The payload that represents the oscillation on state.
    /// </summary>
    public string? PayloadOscillationOn { get; set; }

    /// <summary>
    /// A special payload that resets the percentage state attribute to None when received at the percentage_state_topic.
    /// </summary>
    public string? PayloadResetPercentage { get; set; }

    /// <summary>
    /// A special payload that resets the preset_mode state attribute to None when received at the preset_mode_state_topic.
    /// </summary>
    public string? PayloadResetPresetMode { get; set; }

    /// <summary>
    /// Defines a template to generate the payload to send to percentage_command_topic.
    /// </summary>
    public string? PercentageCommandTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to publish commands to change the fan speed state based on a percentage.
    /// </summary>
    public string? PercentageCommandTopic { get; set; }

    /// <summary>
    /// The MQTT topic subscribed to receive fan speed based on percentage.
    /// </summary>
    public string? PercentageStateTopic { get; set; }

    /// <summary>
    /// Defines a template to extract the percentage value from the payload received on percentage_state_topic.
    /// </summary>
    public string? PercentageValueTemplate { get; set; }

    /// <summary>
    /// Defines a template to generate the payload to send to preset_mode_command_topic.
    /// </summary>
    public string? PresetModeCommandTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to publish commands to change the preset mode.
    /// </summary>
    public string? PresetModeCommandTopic { get; set; }

    /// <summary>
    /// The MQTT topic subscribed to receive fan speed based on presets.
    /// </summary>
    public string? PresetModeStateTopic { get; set; }

    /// <summary>
    /// Defines a template to extract the preset_mode value from the payload received on preset_mode_state_topic.
    /// </summary>
    public string? PresetModeValueTemplate { get; set; }

    /// <summary>
    /// List of preset modes this fan is capable of running at. Common examples include auto, smart, whoosh, eco and breeze.
    /// </summary>
    public IList<string>? PresetModes { get; set; }

    /// <summary>
    /// The maximum of numeric output range (representing 100 %). The number of speeds within the speed_range / 100 will determine the percentage_step.
    /// </summary>
    public int? SpeedRangeMax { get; set; }

    /// <summary>
    /// The minimum of numeric output range (off not included, so speed_range_min - 1 represents 0 %). The number of speeds within the speed_range / 100 will determine the percentage_step.
    /// </summary>
    public int? SpeedRangeMin { get; set; }

    /// <summary>
    /// The MQTT topic subscribed to receive state updates.
    /// </summary>
    public string? StateTopic { get; set; }

    /// <summary>
    /// Defines a [template](/docs/configuration/templating/#using-templates-with-the-mqtt-integration) to extract a value from the state.
    /// </summary>
    public string? StateValueTemplate { get; set; }

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

    public class MqttFanValidator : MqttSensorDiscoveryBaseValidator<MqttFan>
    {
        public MqttFanValidator()
        {
            TopicAndTemplate(s => s.CommandTopic, s => s.CommandTemplate);
            TopicAndTemplate(s => s.OscillationCommandTopic, s => s.OscillationCommandTemplate);
            TopicAndTemplate(s => s.OscillationStateTopic, s => s.OscillationValueTemplate);
            TopicAndTemplate(s => s.PercentageCommandTopic, s => s.PercentageCommandTemplate);
            TopicAndTemplate(s => s.PercentageStateTopic, s => s.PercentageValueTemplate);
            TopicAndTemplate(s => s.PresetModeCommandTopic, s => s.PresetModeCommandTemplate);
            TopicAndTemplate(s => s.PresetModeStateTopic, s => s.PresetModeValueTemplate);
            TopicAndTemplate(s => s.StateTopic, s => s.StateValueTemplate);

            MinMax(s => s.SpeedRangeMin, s => s.SpeedRangeMax, 1, 100);
        }
    }
}