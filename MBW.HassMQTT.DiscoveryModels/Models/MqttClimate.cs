#nullable enable
using System.Collections.Generic;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/climate.mqtt/
    ///
    /// The mqtt climate platform lets you control your MQTT enabled HVAC devices.
    /// </summary>
    [DeviceType(HassDeviceType.Climate)]
    [PublicAPI]
    public class MqttClimate : MqttSensorDiscoveryBase, IHasUniqueId, IHasAvailability, IHasQos, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasRetain
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
        /// The MQTT topic to publish commands to change the away mode.
        /// </summary>
        public string? AwayModeCommandTopic { get; set; }

        /// <summary>
        /// A template to render the value received on the `away_mode_state_topic` with.
        /// </summary>
        public string? AwayModeStateTemplate { get; set; }

        /// <summary>
        /// The MQTT topic to subscribe for changes of the HVAC away mode. If this is not set, the away mode works in optimistic mode (see below).
        /// </summary>
        public string? AwayModeStateTopic { get; set; }

        /// <summary>
        /// A template with which the value received on `current_temperature_topic` will be rendered.
        /// </summary>
        public string? CurrentTemperatureTemplate { get; set; }

        /// <summary>
        /// The MQTT topic on which to listen for the current temperature.
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
        /// A template to render the value sent to the `hold_command_topic` with.
        /// </summary>
        public string? HoldCommandTemplate { get; set; }

        /// <summary>
        /// The MQTT topic to publish commands to change the hold mode.
        /// </summary>
        public string? HoldCommandTopic { get; set; }

        /// <summary>
        /// A template to render the value received on the `hold_state_topic` with.
        /// </summary>
        public string? HoldStateTemplate { get; set; }

        /// <summary>
        /// The MQTT topic to subscribe for changes of the HVAC hold mode. If this is not set, the hold mode works in optimistic mode (see below).
        /// </summary>
        public string? HoldStateTopic { get; set; }

        /// <summary>
        /// A list of available hold modes.
        /// </summary>
        public IList<string>? HoldModes { get; set; }

        /// <summary>
        /// Set the initial target temperature.
        /// </summary>
        public int? Initial { get; set; }

        /// <summary>
        /// Maximum set point available.
        /// </summary>
        public float? MaxTemp { get; set; }

        /// <summary>
        /// Minimum set point available.
        /// </summary>
        public float? MinTemp { get; set; }

        /// <summary>
        /// A template to render the value sent to the `mode_command_topic` with.
        /// </summary>
        public string? ModeCommandTemplate { get; set; }

        /// <summary>
        /// The MQTT topic to publish commands to change the HVAC operation mode.
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
        /// The payload that represents disabled state.
        /// </summary>
        public string? PayloadOff { get; set; }

        /// <summary>
        /// The payload that represents enabled state.
        /// </summary>
        public string? PayloadOn { get; set; }

        /// <summary>
        /// The MQTT topic to publish commands to change the power state. This is useful if your device has a separate power toggle in addition to mode.
        /// </summary>
        public string? PowerCommandTopic { get; set; }

        /// <summary>
        /// The desired precision for this device. Can be used to match your actual thermostat's precision. Supported values are `0.1`, `0.5` and `1.0`.
        /// </summary>
        public float? Precision { get; set; }

        /// <summary>
        /// Set to `false` to suppress sending of all MQTT messages when the current mode is `Off`.
        /// </summary>
        public bool? SendIfOff { get; set; }

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
        /// A template to render the value received on the `temperature_high_state_topic` with.
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
        /// A template to render the value received on the `temperature_low_state_topic` with.
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
        /// The MQTT topic to subscribe for changes in the target temperature. If this is not set, the target temperature works in optimistic mode (see below).
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
    }
}