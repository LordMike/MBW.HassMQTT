using MBW.HassMQTT.DiscoveryModels.Enum;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/fan.mqtt/
    /// </summary>
    [DeviceType(HassDeviceType.Fan)]
    public class MqttFan : MqttEntitySensorDiscoveryBase
    {
        public MqttFan(string topic, string uniqueId) : base(topic, uniqueId)
        {
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the fan state.
        /// </summary>
        public string CommandTopic
        {
            get => GetValue<string>("command_topic", default);
            set => SetValue("command_topic", value);
        }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the JSON dictionary from messages received on the `json_attributes_topic`. Usage example can be found in [MQTT sensor](/integrations/sensor.mqtt/#json-attributes-template-configuration) documentation.
        /// </summary>
        public string JsonAttributesTemplate
        {
            get => GetValue<string>("json_attributes_template", default);
            set => SetValue("json_attributes_template", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive a JSON dictionary payload and then set as sensor attributes. Usage example can be found in [MQTT sensor](/integrations/sensor.mqtt/#json-attributes-topic-configuration) documentation.
        /// </summary>
        public string JsonAttributesTopic
        {
            get => GetValue<string>("json_attributes_topic", default);
            set => SetValue("json_attributes_topic", value);
        }

        /// <summary>
        /// The name of the fan.
        /// </summary>
        public string Name
        {
            get => GetValue<string>("name", default);
            set => SetValue("name", value);
        }

        /// <summary>
        /// Flag that defines if lock works in optimistic mode
        /// </summary>
        public bool Optimistic
        {
            get => GetValue<bool>("optimistic", default);
            set => SetValue("optimistic", value);
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the oscillation state.
        /// </summary>
        public string OscillationCommandTopic
        {
            get => GetValue<string>("oscillation_command_topic", default);
            set => SetValue("oscillation_command_topic", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive oscillation state updates.
        /// </summary>
        public string OscillationStateTopic
        {
            get => GetValue<string>("oscillation_state_topic", default);
            set => SetValue("oscillation_state_topic", value);
        }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract a value from the oscillation.
        /// </summary>
        public string OscillationValueTemplate
        {
            get => GetValue<string>("oscillation_value_template", default);
            set => SetValue("oscillation_value_template", value);
        }

        /// <summary>
        /// The payload that represents the fan's high speed.
        /// </summary>
        public string PayloadHighSpeed
        {
            get => GetValue<string>("payload_high_speed", default);
            set => SetValue("payload_high_speed", value);
        }

        /// <summary>
        /// The payload that represents the fan's low speed.
        /// </summary>
        public string PayloadLowSpeed
        {
            get => GetValue<string>("payload_low_speed", default);
            set => SetValue("payload_low_speed", value);
        }

        /// <summary>
        /// The payload that represents the fan's medium speed.
        /// </summary>
        public string PayloadMediumSpeed
        {
            get => GetValue<string>("payload_medium_speed", default);
            set => SetValue("payload_medium_speed", value);
        }

        /// <summary>
        /// The payload that represents the stop state.
        /// </summary>
        public string PayloadOff
        {
            get => GetValue<string>("payload_off", default);
            set => SetValue("payload_off", value);
        }

        /// <summary>
        /// The payload that represents the running state.
        /// </summary>
        public string PayloadOn
        {
            get => GetValue<string>("payload_on", default);
            set => SetValue("payload_on", value);
        }

        /// <summary>
        /// The payload that represents the oscillation off state.
        /// </summary>
        public string PayloadOscillationOff
        {
            get => GetValue<string>("payload_oscillation_off", default);
            set => SetValue("payload_oscillation_off", value);
        }

        /// <summary>
        /// The payload that represents the oscillation on state.
        /// </summary>
        public string PayloadOscillationOn
        {
            get => GetValue<string>("payload_oscillation_on", default);
            set => SetValue("payload_oscillation_on", value);
        }

        /// <summary>
        /// The maximum QoS level of the state topic.
        /// </summary>
        public int Qos
        {
            get => GetValue<int>("qos", default);
            set => SetValue("qos", value);
        }

        /// <summary>
        /// If the published message should have the retain flag on or not.
        /// </summary>
        public bool Retain
        {
            get => GetValue<bool>("retain", default);
            set => SetValue("retain", value);
        }

        /// <summary>
        /// The MQTT topic to publish commands to change speed state.
        /// </summary>
        public string SpeedCommandTopic
        {
            get => GetValue<string>("speed_command_topic", default);
            set => SetValue("speed_command_topic", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive speed state updates.
        /// </summary>
        public string SpeedStateTopic
        {
            get => GetValue<string>("speed_state_topic", default);
            set => SetValue("speed_state_topic", value);
        }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract a value from the speed payload.
        /// </summary>
        public string SpeedValueTemplate
        {
            get => GetValue<string>("speed_value_template", default);
            set => SetValue("speed_value_template", value);
        }

        /// <summary>
        /// List of speeds this fan is capable of running at. Valid entries are `off`, `low`, `medium` and `high`.
        /// </summary>
        public string[] Speeds
        {
            get => GetValue<string[]>("speeds", default);
            set => SetValue("speeds", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive state updates.
        /// </summary>
        public string StateTopic
        {
            get => GetValue<string>("state_topic", default);
            set => SetValue("state_topic", value);
        }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract a value from the state.
        /// </summary>
        public string StateValueTemplate
        {
            get => GetValue<string>("state_value_template", default);
            set => SetValue("state_value_template", value);
        }
    }
}