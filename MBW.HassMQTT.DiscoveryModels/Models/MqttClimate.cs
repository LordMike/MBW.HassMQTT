#nullable enable
using FluentValidation;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;
using System;
using System.Collections.Generic;

namespace MBW.HassMQTT.DiscoveryModels.Models;

/// <summary>
/// https://www.home-assistant.io/integrations/climate.mqtt/
///
/// The mqtt climate platform lets you control your MQTT enabled HVAC devices.
/// </summary>
[DeviceType(HassDeviceType.Climate)]
[PublicAPI]
public class MqttClimate : MqttSensorDiscoveryBase<MqttClimate, MqttClimate.MqttClimateValidator>, IHasUniqueId,
    IHasAvailability, IHasQos, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasRetain, IHasEntityCategory,
    IHasObjectId, IHasEncoding
{
    public MqttClimate(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
    {
    }

    /// <summary>
    /// A template to render the value received on the `action_topic` with.
    /// </summary>
    public string? ActionTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to subscribe for changes of the current action. If this is set, the climate graph
    /// uses the value received as data source. Valid values: `off`, `heating`, `cooling`, `drying`, `idle`, `fan`.
    /// </summary>
    public string? ActionTopic { get; set; }

    /// <summary>
    /// The MQTT topic to publish commands to switch auxiliary heat.
    /// </summary>
    public string? AuxCommandTopic { get; set; }

    /// <summary>
    /// A template to render the value received on the `aux_state_topic` with.
    /// </summary>
    public string? AuxStateTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to subscribe for changes of the auxiliary heat mode. If this is not set, the auxiliary
    /// heat mode works in optimistic mode (see below).
    /// </summary>
    public string? AuxStateTopic { get; set; }

    /// <summary>
    /// A template with which the value received on `current_humidity_topic` will be rendered.
    /// </summary>
    public string? CurrentHumidityTemplate { get; set; }

    /// <summary>
    /// The MQTT topic on which to listen for the current humidity. A `"None"` value received will reset the current humidity. Empty values (`'''`) will be ignored. 
    /// </summary>
    public string? CurrentHumidityTopic { get; set; }

    /// <summary>
    /// A template with which the value received on `current_temperature_topic` will be rendered.
    /// </summary>
    public string? CurrentTemperatureTemplate { get; set; }

    /// <summary>
    /// The MQTT topic on which to listen for the current temperature. A `"None"` value received will reset the current temperature. Empty values (`'''`) will be ignored.
    /// </summary>
    public string? CurrentTemperatureTopic { get; set; }

    /// <summary>
    /// A template to render the value sent to the `fan_mode_command_topic` with.
    /// </summary>
    public string? FanModeCommandTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to publish commands to change the fan mode.
    /// </summary>
    public string? FanModeCommandTopic { get; set; }

    /// <summary>
    /// A template to render the value received on the `fan_mode_state_topic` with.
    /// </summary>
    public string? FanModeStateTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to subscribe for changes of the HVAC fan mode. If this is not set, the fan mode works in optimistic mode (see below).
    /// </summary>
    public string? FanModeStateTopic { get; set; }

    /// <summary>
    /// A list of supported fan modes.
    /// </summary>
    public IList<string>? FanModes { get; set; }

    /// <summary>
    /// Set the initial target temperature. The default value depends on the temperature unit and will be 21° or 69.8°F.
    /// </summary>
    public int? Initial { get; set; }

    /// <summary>
    /// The minimum target humidity percentage that can be set.
    /// </summary>
    /// <remarks>Default value: 99</remarks>
    public int? MaxHumidity { get; set; }

    /// <summary>
    /// Maximum set point available. The default value depends on the temperature unit, and will be 35°C or 95°F.
    /// </summary>
    public float? MaxTemp { get; set; }

    /// <summary>
    /// min_humidity
    /// </summary>
    /// <remarks>Default value: 30</remarks>
    public int? MinHumidity { get; set; }

    /// <summary>
    /// Minimum set point available. The default value depends on the temperature unit, and will be 7°C or 44.6°F.
    /// </summary>
    public float? MinTemp { get; set; }

    /// <summary>
    /// A template to render the value sent to the `mode_command_topic` with.
    /// </summary>
    public string? ModeCommandTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to publish commands to change the HVAC operation mode. Use `power_command_topic` if you only want to publish the power state.
    /// </summary>
    public string? ModeCommandTopic { get; set; }

    /// <summary>
    /// A template to render the value received on the `mode_state_topic` with.
    /// </summary>
    public string? ModeStateTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to subscribe for changes of the HVAC operation mode. If this is not set, the operation mode works in optimistic mode (see below).
    /// </summary>
    public string? ModeStateTopic { get; set; }

    /// <summary>
    /// A list of supported modes. Needs to be a subset of the default values.
    /// </summary>
    public IList<string>? Modes { get; set; }

    /// <summary>
    /// The name of the HVAC.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Flag that defines if the climate works in optimistic mode
    /// </summary>
    /// <remarks>Default value: `true` if no state topic defined, else `false`.</remarks>
    public bool? Optimistic { get; set; }

    /// <summary>
    /// The MQTT topic to publish commands to change the HVAC operation mode. Use `power_command_topic` if you only want to publish the power state.
    /// </summary>
    public string? PayloadOff { get; set; }

    /// <summary>
    /// The payload sent to turn the device on.
    /// </summary>
    public string? PayloadOn { get; set; }

    /// <summary>
    /// A template to render the value sent to the `power_command_topic` with. The `value` parameter is the payload set for `payload_on` or `payload_off`.
    /// </summary>
    public string? PowerCommandTemplate { get; set; }
    
    /// <summary>
    /// The MQTT topic to publish commands to change the HVAC power state. Sends the payload configured with `payload_on` if the climate is turned on via the `climate.turn_on`, or the payload configured with `payload_off` if the climate is turned off via the `climate.turn_off` service. The climate device reports it's state back via `mode_command_topic`. Note that when this option is used in `optimistic` mode, service `climate.turn_on` will send a the message configured with `payload_on` to the device but will not update the state of the climate.
    /// </summary>
    public string? PowerCommandTopic { get; set; }
    
    /// <summary>
    /// The desired precision for this device. Can be used to match your actual thermostat's precision. Supported values are `0.1`, `0.5` and `1.0`.
    /// </summary>
    public float? Precision { get; set; }

    /// <summary>
    /// Defines a template to generate the payload to send to `preset_mode_command_topic`.
    /// </summary>
    public string? PresetModeCommandTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to publish commands to change the preset mode.
    /// </summary>
    public string? PresetModeCommandTopic { get; set; }

    /// <summary>
    /// The MQTT topic subscribed to receive climate speed based on presets. When preset 'none' is received or `None` the `preset_mode` will be reset.
    /// </summary>
    public string? PresetModeStateTopic { get; set; }

    /// <summary>
    /// Defines a template to extract the `preset_mode` value from the payload received on `preset_mode_state_topic`.
    /// </summary>
    public string? PresetModeValueTemplate { get; set; }

    /// <summary>
    /// List of preset modes this climate is supporting. Common examples include `eco`, `away`, `boost`, `comfort`, `home`, `sleep` and `activity`.
    /// </summary>
    public IList<string>? PresetModes { get; set; }

    /// <summary>
    /// A template to render the value sent to the `swing_mode_command_topic` with.
    /// </summary>
    public string? SwingModeCommandTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to publish commands to change the swing mode.
    /// </summary>
    public string? SwingModeCommandTopic { get; set; }

    /// <summary>
    /// A template to render the value received on the `swing_mode_state_topic` with.
    /// </summary>
    public string? SwingModeStateTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to subscribe for changes of the HVAC swing mode. If this is not set, the swing mode works in optimistic mode (see below).
    /// </summary>
    public string? SwingModeStateTopic { get; set; }

    /// <summary>
    /// A list of supported swing modes.
    /// </summary>
    public IList<string>? SwingModes { get; set; }

    /// <summary>
    /// Defines a template to generate the payload to send to `target_humidity_command_topic`.
    /// </summary>
    public string? TargetHumidityCommandTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to publish commands to change the target humidity.
    /// </summary>
    public string? TargetHumidityCommandTopic { get; set; }

    /// <summary>
    /// The MQTT topic subscribed to receive the target humidity. If this is not set, the target humidity works in optimistic mode (see below). A `"None"` value received will reset the target humidity. Empty values (`'''`) will be ignored.
    /// </summary>
    public string? TargetHumidityStateTopic { get; set; }

    /// <summary>
    /// Defines a template to extract a value for the climate `target_humidity` state.
    /// </summary>
    public string? TargetHumidityStateTemplate { get; set; }

    /// <summary>
    /// A template to render the value sent to the `temperature_command_topic` with.
    /// </summary>
    public string? TemperatureCommandTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to publish commands to change the target temperature.
    /// </summary>
    public string? TemperatureCommandTopic { get; set; }

    /// <summary>
    /// A template to render the value sent to the `temperature_high_command_topic` with.
    /// </summary>
    public string? TemperatureHighCommandTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to publish commands to change the high target temperature.
    /// </summary>
    public string? TemperatureHighCommandTopic { get; set; }

    /// <summary>
    /// A template to render the value received on the `temperature_high_state_topic` with. A `"None"` value received will reset the temperature high set point. Empty values (`'''`) will be ignored.
    /// </summary>
    public string? TemperatureHighStateTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to subscribe for changes in the target high temperature. If this is not set, the target high temperature works in optimistic mode (see below).
    /// </summary>
    public string? TemperatureHighStateTopic { get; set; }

    /// <summary>
    /// A template to render the value sent to the `temperature_low_command_topic` with.
    /// </summary>
    public string? TemperatureLowCommandTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to publish commands to change the target low temperature.
    /// </summary>
    public string? TemperatureLowCommandTopic { get; set; }

    /// <summary>
    /// A template to render the value received on the `temperature_low_state_topic` with. A `"None"` value received will reset the temperature low set point. Empty values (`'''`) will be ignored.
    /// </summary>
    public string? TemperatureLowStateTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to subscribe for changes in the target low temperature. If this is not set, the target low temperature works in optimistic mode (see below).
    /// </summary>
    public string? TemperatureLowStateTopic { get; set; }

    /// <summary>
    /// A template to render the value received on the `temperature_state_topic` with.
    /// </summary>
    public string? TemperatureStateTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to subscribe for changes in the target temperature. If this is not set, the target temperature works in optimistic mode (see below). A `"None"` value received will reset the temperature set point. Empty values (`'''`) will be ignored.
    /// </summary>
    public string? TemperatureStateTopic { get; set; }

    /// <summary>
    /// Defines the temperature unit of the device, `C` or `F`. If this is not set, the temperature unit is set to the system temperature unit.
    /// </summary>
    public HassTemperatureUnit? TemperatureUnit { get; set; }

    /// <summary>
    /// Step size for temperature set point.
    /// </summary>
    public float? TempStep { get; set; }

    /// <summary>
    /// Default template to render the payloads on *all* `*_state_topic`s with.
    /// </summary>
    public string? ValueTemplate { get; set; }

    public string? UniqueId { get; set; }
    public IList<AvailabilityModel>? Availability { get; set; }
    public AvailabilityMode? AvailabilityMode { get; set; }
    public string? JsonAttributesTemplate { get; set; }
    public string? JsonAttributesTopic { get; set; }
    public string? Icon { get; set; }
    public bool? EnabledByDefault { get; set; }
    public MqttQosLevel? Qos { get; set; }
    public bool? Retain { get; set; }
    public EntityCategory? EntityCategory { get; set; }
    public string? ObjectId { get; set; }
    public string? Encoding { get; set; }

    public class MqttClimateValidator : MqttSensorDiscoveryBaseValidator<MqttClimate>
    {
        public MqttClimateValidator()
        {
            TopicAndTemplate(s => s.ActionTopic, s => s.ActionTemplate);
            TopicAndTemplate(s => s.AuxStateTopic, s => s.AuxStateTemplate);
            TopicAndTemplate(s => s.PresetModeCommandTopic, s => s.PresetModeCommandTemplate);
            TopicAndTemplate(s => s.PresetModeStateTopic, s => s.PresetModeValueTemplate);
            TopicAndTemplate(s => s.CurrentTemperatureTopic, s => s.CurrentTemperatureTemplate);
            TopicAndTemplate(s => s.FanModeCommandTopic, s => s.FanModeCommandTemplate);
            TopicAndTemplate(s => s.FanModeStateTopic, s => s.FanModeStateTemplate);
            TopicAndTemplate(s => s.ModeCommandTopic, s => s.ModeCommandTemplate);
            TopicAndTemplate(s => s.ModeStateTopic, s => s.ModeStateTemplate);
            TopicAndTemplate(s => s.SwingModeCommandTopic, s => s.SwingModeCommandTemplate);
            TopicAndTemplate(s => s.SwingModeStateTopic, s => s.SwingModeStateTemplate);
            TopicAndTemplate(s => s.TemperatureCommandTopic, s => s.TemperatureCommandTemplate);
            TopicAndTemplate(s => s.TemperatureHighCommandTopic, s => s.TemperatureHighCommandTemplate);
            TopicAndTemplate(s => s.TemperatureHighStateTopic, s => s.TemperatureHighStateTemplate);
            TopicAndTemplate(s => s.TemperatureLowCommandTopic, s => s.TemperatureLowCommandTemplate);
            TopicAndTemplate(s => s.TemperatureLowStateTopic, s => s.TemperatureLowStateTemplate);
            TopicAndTemplate(s => s.TemperatureStateTopic, s => s.TemperatureStateTemplate);
            TopicAndTemplate(s => s.CurrentHumidityTopic, s => s.CurrentHumidityTemplate);
            TopicAndTemplate(s => s.TargetHumidityCommandTopic, s => s.TargetHumidityCommandTemplate);
            TopicAndTemplate(s => s.TargetHumidityStateTopic, s => s.TargetHumidityStateTemplate);
            TopicAndTemplate(s => s.PowerCommandTopic, s => s.PowerCommandTemplate);

            MinMax(s => s.MinHumidity, s => s.MaxHumidity, 30, 99);
            MinMax(s => s.MinTemp, s => s.MaxTemp, -30, 60, (s => s.Initial, 21));

            RuleFor(s => s.Precision)
                .Must(s => Math.Abs(s!.Value - 0.1f) < float.Epsilon ||
                           Math.Abs(s.Value - 0.5f) < float.Epsilon ||
                           Math.Abs(s.Value - 1f) < float.Epsilon)
                .When(s => s.Precision.HasValue);
        }
    }
}