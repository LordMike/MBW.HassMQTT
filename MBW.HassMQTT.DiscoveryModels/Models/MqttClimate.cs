using EnumsNET;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/climate.mqtt/
    /// </summary>
    [DeviceType(HassDeviceType.Climate)]
    public class MqttClimate : MqttEntitySensorDiscoveryBase
    {
        public MqttClimate(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// A template to render the value received on the `action_topic` with.
        /// </summary>
        [PublicAPI]
        public string ActionTemplate
        {
            get => GetValue<string>("action_template", default);
            set => SetValue("action_template", value);
        }

        /// <summary>
        /// The MQTT topic to subscribe for changes of the current action. If this is set, the climate graph uses the value received as data source. Valid values: `off`, `heating`, `cooling`, `drying`, `idle`, `fan`.
        /// </summary>
        [PublicAPI]
        public string ActionTopic
        {
            get => GetValue<string>("action_topic", default);
            set => SetValue("action_topic", value);
        }

        /// <summary>
        /// The MQTT topic to publish commands to switch auxiliary heat.
        /// </summary>
        [PublicAPI]
        public string AuxCommandTopic
        {
            get => GetValue<string>("aux_command_topic", default);
            set => SetValue("aux_command_topic", value);
        }

        /// <summary>
        /// A template to render the value received on the `aux_state_topic` with.
        /// </summary>
        [PublicAPI]
        public string AuxStateTemplate
        {
            get => GetValue<string>("aux_state_template", default);
            set => SetValue("aux_state_template", value);
        }

        /// <summary>
        /// The MQTT topic to subscribe for changes of the auxiliary heat mode. If this is not set, the auxiliary heat mode works in optimistic mode (see below).
        /// </summary>
        [PublicAPI]
        public string AuxStateTopic
        {
            get => GetValue<string>("aux_state_topic", default);
            set => SetValue("aux_state_topic", value);
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the away mode.
        /// </summary>
        [PublicAPI]
        public string AwayModeCommandTopic
        {
            get => GetValue<string>("away_mode_command_topic", default);
            set => SetValue("away_mode_command_topic", value);
        }

        /// <summary>
        /// A template to render the value received on the `away_mode_state_topic` with.
        /// </summary>
        [PublicAPI]
        public string AwayModeStateTemplate
        {
            get => GetValue<string>("away_mode_state_template", default);
            set => SetValue("away_mode_state_template", value);
        }

        /// <summary>
        /// The MQTT topic to subscribe for changes of the HVAC away mode. If this is not set, the away mode works in optimistic mode (see below).
        /// </summary>
        [PublicAPI]
        public string AwayModeStateTopic
        {
            get => GetValue<string>("away_mode_state_topic", default);
            set => SetValue("away_mode_state_topic", value);
        }

        /// <summary>
        /// A template with which the value received on `current_temperature_topic` will be rendered.
        /// </summary>
        [PublicAPI]
        public string CurrentTemperatureTemplate
        {
            get => GetValue<string>("current_temperature_template", default);
            set => SetValue("current_temperature_template", value);
        }

        /// <summary>
        /// The MQTT topic on which to listen for the current temperature.
        /// </summary>
        [PublicAPI]
        public string CurrentTemperatureTopic
        {
            get => GetValue<string>("current_temperature_topic", default);
            set => SetValue("current_temperature_topic", value);
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the fan mode.
        /// </summary>
        [PublicAPI]
        public string FanModeCommandTopic
        {
            get => GetValue<string>("fan_mode_command_topic", default);
            set => SetValue("fan_mode_command_topic", value);
        }

        /// <summary>
        /// A template to render the value received on the `fan_mode_state_topic` with.
        /// </summary>
        [PublicAPI]
        public string FanModeStateTemplate
        {
            get => GetValue<string>("fan_mode_state_template", default);
            set => SetValue("fan_mode_state_template", value);
        }

        /// <summary>
        /// The MQTT topic to subscribe for changes of the HVAC fan mode. If this is not set, the fan mode works in optimistic mode (see below).
        /// </summary>
        [PublicAPI]
        public string FanModeStateTopic
        {
            get => GetValue<string>("fan_mode_state_topic", default);
            set => SetValue("fan_mode_state_topic", value);
        }

        /// <summary>
        /// A list of supported fan modes.
        /// </summary>
        [PublicAPI]
        public string[] FanModes
        {
            get => GetValue<string[]>("fan_modes", default);
            set => SetValue("fan_modes", value);
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the hold mode.
        /// </summary>
        [PublicAPI]
        public string HoldCommandTopic
        {
            get => GetValue<string>("hold_command_topic", default);
            set => SetValue("hold_command_topic", value);
        }

        /// <summary>
        /// A template to render the value received on the `hold_state_topic` with.
        /// </summary>
        [PublicAPI]
        public string HoldStateTemplate
        {
            get => GetValue<string>("hold_state_template", default);
            set => SetValue("hold_state_template", value);
        }

        /// <summary>
        /// The MQTT topic to subscribe for changes of the HVAC hold mode. If this is not set, the hold mode works in optimistic mode (see below).
        /// </summary>
        [PublicAPI]
        public string HoldStateTopic
        {
            get => GetValue<string>("hold_state_topic", default);
            set => SetValue("hold_state_topic", value);
        }

        /// <summary>
        /// A list of available hold modes.
        /// </summary>
        [PublicAPI]
        public string[] HoldModes
        {
            get => GetValue<string[]>("hold_modes", default);
            set => SetValue("hold_modes", value);
        }

        /// <summary>
        /// Set the initial target temperature.
        /// </summary>
        [PublicAPI]
        public int Initial
        {
            get => GetValue<int>("initial", default);
            set => SetValue("initial", value);
        }

        /// <summary>
        /// Maximum set point available.
        /// </summary>
        [PublicAPI]
        public float MaxTemp
        {
            get => GetValue<float>("max_temp", default);
            set => SetValue("max_temp", value);
        }

        /// <summary>
        /// Minimum set point available.
        /// </summary>
        [PublicAPI]
        public float MinTemp
        {
            get => GetValue<float>("min_temp", default);
            set => SetValue("min_temp", value);
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the HVAC operation mode.
        /// </summary>
        [PublicAPI]
        public string ModeCommandTopic
        {
            get => GetValue<string>("mode_command_topic", default);
            set => SetValue("mode_command_topic", value);
        }

        /// <summary>
        /// A template to render the value received on the `mode_state_topic` with.
        /// </summary>
        [PublicAPI]
        public string ModeStateTemplate
        {
            get => GetValue<string>("mode_state_template", default);
            set => SetValue("mode_state_template", value);
        }

        /// <summary>
        /// The MQTT topic to subscribe for changes of the HVAC operation mode. If this is not set, the operation mode works in optimistic mode (see below).
        /// </summary>
        [PublicAPI]
        public string ModeStateTopic
        {
            get => GetValue<string>("mode_state_topic", default);
            set => SetValue("mode_state_topic", value);
        }

        /// <summary>
        /// A list of supported modes. Needs to be a subset of the default values.
        /// </summary>
        [PublicAPI]
        public string[] Modes
        {
            get => GetValue<string[]>("modes", default);
            set => SetValue("modes", value);
        }

        /// <summary>
        /// The name of the HVAC.
        /// </summary>
        [PublicAPI]
        public string Name
        {
            get => GetValue<string>("name", default);
            set => SetValue("name", value);
        }

        /// <summary>
        /// The payload that represents disabled state.
        /// </summary>
        [PublicAPI]
        public string PayloadOff
        {
            get => GetValue<string>("payload_off", default);
            set => SetValue("payload_off", value);
        }

        /// <summary>
        /// The payload that represents enabled state.
        /// </summary>
        [PublicAPI]
        public string PayloadOn
        {
            get => GetValue<string>("payload_on", default);
            set => SetValue("payload_on", value);
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the power state. This is useful if your device has a separate power toggle in addition to mode.
        /// </summary>
        [PublicAPI]
        public string PowerCommandTopic
        {
            get => GetValue<string>("power_command_topic", default);
            set => SetValue("power_command_topic", value);
        }

        /// <summary>
        /// The desired precision for this device. Can be used to match your actual thermostat's precision. Supported values are `0.1`, `0.5` and `1.0`.
        /// </summary>
        [PublicAPI]
        public float Precision
        {
            get => GetValue<float>("precision", default);
            set => SetValue("precision", value);
        }

        /// <summary>
        /// The maximum QoS level to be used when receiving and publishing messages.
        /// </summary>
        [PublicAPI]
        public int Qos
        {
            get => GetValue<int>("qos", default);
            set => SetValue("qos", value);
        }

        /// <summary>
        /// Defines if published messages should have the retain flag set.
        /// </summary>
        [PublicAPI]
        public bool Retain
        {
            get => GetValue<bool>("retain", default);
            set => SetValue("retain", value);
        }

        /// <summary>
        /// Set to `false` to suppress sending of all MQTT messages when the current mode is `Off`.
        /// </summary>
        [PublicAPI]
        public bool SendIfOff
        {
            get => GetValue<bool>("send_if_off", default);
            set => SetValue("send_if_off", value);
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the swing mode.
        /// </summary>
        [PublicAPI]
        public string SwingModeCommandTopic
        {
            get => GetValue<string>("swing_mode_command_topic", default);
            set => SetValue("swing_mode_command_topic", value);
        }

        /// <summary>
        /// A template to render the value received on the `swing_mode_state_topic` with.
        /// </summary>
        [PublicAPI]
        public string SwingModeStateTemplate
        {
            get => GetValue<string>("swing_mode_state_template", default);
            set => SetValue("swing_mode_state_template", value);
        }

        /// <summary>
        /// The MQTT topic to subscribe for changes of the HVAC swing mode. If this is not set, the swing mode works in optimistic mode (see below).
        /// </summary>
        [PublicAPI]
        public string SwingModeStateTopic
        {
            get => GetValue<string>("swing_mode_state_topic", default);
            set => SetValue("swing_mode_state_topic", value);
        }

        /// <summary>
        /// A list of supported swing modes.
        /// </summary>
        [PublicAPI]
        public string[] SwingModes
        {
            get => GetValue<string[]>("swing_modes", default);
            set => SetValue("swing_modes", value);
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the target temperature.
        /// </summary>
        [PublicAPI]
        public string TemperatureCommandTopic
        {
            get => GetValue<string>("temperature_command_topic", default);
            set => SetValue("temperature_command_topic", value);
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the high target temperature.
        /// </summary>
        [PublicAPI]
        public string TemperatureHighCommandTopic
        {
            get => GetValue<string>("temperature_high_command_topic", default);
            set => SetValue("temperature_high_command_topic", value);
        }

        /// <summary>
        /// A template to render the value received on the `temperature_high_state_topic` with.
        /// </summary>
        [PublicAPI]
        public string TemperatureHighStateTemplate
        {
            get => GetValue<string>("temperature_high_state_template", default);
            set => SetValue("temperature_high_state_template", value);
        }

        /// <summary>
        /// The MQTT topic to subscribe for changes in the target high temperature. If this is not set, the target high temperature works in optimistic mode (see below).
        /// </summary>
        [PublicAPI]
        public string TemperatureHighStateTopic
        {
            get => GetValue<string>("temperature_high_state_topic", default);
            set => SetValue("temperature_high_state_topic", value);
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the target low temperature.
        /// </summary>
        [PublicAPI]
        public string TemperatureLowCommandTopic
        {
            get => GetValue<string>("temperature_low_command_topic", default);
            set => SetValue("temperature_low_command_topic", value);
        }

        /// <summary>
        /// A template to render the value received on the `temperature_low_state_topic` with.
        /// </summary>
        [PublicAPI]
        public string TemperatureLowStateTemplate
        {
            get => GetValue<string>("temperature_low_state_template", default);
            set => SetValue("temperature_low_state_template", value);
        }

        /// <summary>
        /// The MQTT topic to subscribe for changes in the target low temperature. If this is not set, the target low temperature works in optimistic mode (see below).
        /// </summary>
        [PublicAPI]
        public string TemperatureLowStateTopic
        {
            get => GetValue<string>("temperature_low_state_topic", default);
            set => SetValue("temperature_low_state_topic", value);
        }

        /// <summary>
        /// A template to render the value received on the `temperature_state_topic` with.
        /// </summary>
        [PublicAPI]
        public string TemperatureStateTemplate
        {
            get => GetValue<string>("temperature_state_template", default);
            set => SetValue("temperature_state_template", value);
        }

        /// <summary>
        /// The MQTT topic to subscribe for changes in the target temperature. If this is not set, the target temperature works in optimistic mode (see below).
        /// </summary>
        [PublicAPI]
        public string TemperatureStateTopic
        {
            get => GetValue<string>("temperature_state_topic", default);
            set => SetValue("temperature_state_topic", value);
        }

        /// <summary>
        /// Defines the temperature unit of the device, `C` or `F`. If this is not set, the temperature unit is set to the system temperature unit.
        /// </summary>
        [PublicAPI]
        public HassTemperatureUnit TemperatureUnit
        {
            get => Enums.Parse<HassTemperatureUnit>(GetValue<string>("temperature_unit", default), true,
                EnumFormat.EnumMemberValue);
            set => SetValue("temperature_unit", value.AsString(EnumFormat.EnumMemberValue));
        }

        /// <summary>
        /// Step size for temperature set point.
        /// </summary>
        [PublicAPI]
        public float TempStep
        {
            get => GetValue<float>("temp_step", default);
            set => SetValue("temp_step", value);
        }

        /// <summary>
        /// Default template to render the payloads on *all* `*_state_topic`s with.
        /// </summary>
        [PublicAPI]
        public string ValueTemplate
        {
            get => GetValue<string>("value_template", default);
            set => SetValue("value_template", value);
        }
    }
}