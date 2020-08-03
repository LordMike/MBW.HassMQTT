using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/fan.mqtt/
    /// </summary>
    [DeviceType(HassDeviceType.Fan)]
    public class MqttFan : MqttEntitySensorDiscoveryBase
    {
        public MqttFan(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the fan state.
        /// </summary>
        [PublicAPI]
        public string CommandTopic
        {
            get => GetValue<string>("command_topic", default);
            set => SetValue("command_topic", value);
        }

        /// <summary>
        /// The name of the fan.
        /// </summary>
        [PublicAPI]
        public string Name
        {
            get => GetValue<string>("name", default);
            set => SetValue("name", value);
        }

        /// <summary>
        /// Flag that defines if lock works in optimistic mode
        /// </summary>
        [PublicAPI]
        public bool Optimistic
        {
            get => GetValue<bool>("optimistic", default);
            set => SetValue("optimistic", value);
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the oscillation state.
        /// </summary>
        [PublicAPI]
        public string OscillationCommandTopic
        {
            get => GetValue<string>("oscillation_command_topic", default);
            set => SetValue("oscillation_command_topic", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive oscillation state updates.
        /// </summary>
        [PublicAPI]
        public string OscillationStateTopic
        {
            get => GetValue<string>("oscillation_state_topic", default);
            set => SetValue("oscillation_state_topic", value);
        }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract a value from the oscillation.
        /// </summary>
        [PublicAPI]
        public string OscillationValueTemplate
        {
            get => GetValue<string>("oscillation_value_template", default);
            set => SetValue("oscillation_value_template", value);
        }

        /// <summary>
        /// The payload that represents the fan's high speed.
        /// </summary>
        [PublicAPI]
        public string PayloadHighSpeed
        {
            get => GetValue<string>("payload_high_speed", default);
            set => SetValue("payload_high_speed", value);
        }

        /// <summary>
        /// The payload that represents the fan's low speed.
        /// </summary>
        [PublicAPI]
        public string PayloadLowSpeed
        {
            get => GetValue<string>("payload_low_speed", default);
            set => SetValue("payload_low_speed", value);
        }

        /// <summary>
        /// The payload that represents the fan's medium speed.
        /// </summary>
        [PublicAPI]
        public string PayloadMediumSpeed
        {
            get => GetValue<string>("payload_medium_speed", default);
            set => SetValue("payload_medium_speed", value);
        }

        /// <summary>
        /// The payload that represents the stop state.
        /// </summary>
        [PublicAPI]
        public string PayloadOff
        {
            get => GetValue<string>("payload_off", default);
            set => SetValue("payload_off", value);
        }

        /// <summary>
        /// The payload that represents the running state.
        /// </summary>
        [PublicAPI]
        public string PayloadOn
        {
            get => GetValue<string>("payload_on", default);
            set => SetValue("payload_on", value);
        }

        /// <summary>
        /// The payload that represents the oscillation off state.
        /// </summary>
        [PublicAPI]
        public string PayloadOscillationOff
        {
            get => GetValue<string>("payload_oscillation_off", default);
            set => SetValue("payload_oscillation_off", value);
        }

        /// <summary>
        /// The payload that represents the oscillation on state.
        /// </summary>
        [PublicAPI]
        public string PayloadOscillationOn
        {
            get => GetValue<string>("payload_oscillation_on", default);
            set => SetValue("payload_oscillation_on", value);
        }

        /// <summary>
        /// The maximum QoS level of the state topic.
        /// </summary>
        [PublicAPI]
        public int Qos
        {
            get => GetValue<int>("qos", default);
            set => SetValue("qos", value);
        }

        /// <summary>
        /// If the published message should have the retain flag on or not.
        /// </summary>
        [PublicAPI]
        public bool Retain
        {
            get => GetValue<bool>("retain", default);
            set => SetValue("retain", value);
        }

        /// <summary>
        /// The MQTT topic to publish commands to change speed state.
        /// </summary>
        [PublicAPI]
        public string SpeedCommandTopic
        {
            get => GetValue<string>("speed_command_topic", default);
            set => SetValue("speed_command_topic", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive speed state updates.
        /// </summary>
        [PublicAPI]
        public string SpeedStateTopic
        {
            get => GetValue<string>("speed_state_topic", default);
            set => SetValue("speed_state_topic", value);
        }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract a value from the speed payload.
        /// </summary>
        [PublicAPI]
        public string SpeedValueTemplate
        {
            get => GetValue<string>("speed_value_template", default);
            set => SetValue("speed_value_template", value);
        }

        /// <summary>
        /// List of speeds this fan is capable of running at. Valid entries are `off`, `low`, `medium` and `high`.
        /// </summary>
        [PublicAPI]
        public string[] Speeds
        {
            get => GetValue<string[]>("speeds", default);
            set => SetValue("speeds", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive state updates.
        /// </summary>
        [PublicAPI]
        public string StateTopic
        {
            get => GetValue<string>("state_topic", default);
            set => SetValue("state_topic", value);
        }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract a value from the state.
        /// </summary>
        [PublicAPI]
        public string StateValueTemplate
        {
            get => GetValue<string>("state_value_template", default);
            set => SetValue("state_value_template", value);
        }
    }
}