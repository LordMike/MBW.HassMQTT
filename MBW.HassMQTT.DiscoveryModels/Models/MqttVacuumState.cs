using MBW.HassMQTT.DiscoveryModels.Enum;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/vacuum.mqtt/#state-configuration
    /// </summary>
    [DeviceType(HassDeviceType.Vacuum)]
    public class MqttVacuumState : MqttEntitySensorDiscoveryBase
    {
        public MqttVacuumState(string topic, string uniqueId) : base(topic, uniqueId)
        {
        }

        /// <summary>
        /// The MQTT topic to publish commands to control the vacuum.
        /// </summary>
        public string CommandTopic
        {
            get => GetValue<string>("command_topic", default);
            set => SetValue("command_topic", value);
        }

        /// <summary>
        /// List of possible fan speeds for the vacuum.
        /// </summary>
        public string[] FanSpeedList
        {
            get => GetValue<string[]>("fan_speed_list", default);
            set => SetValue("fan_speed_list", value);
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
        /// The name of the vacuum.
        /// </summary>
        public string Name
        {
            get => GetValue<string>("name", default);
            set => SetValue("name", value);
        }

        /// <summary>
        /// The payload to send to the `command_topic` to begin a spot cleaning cycle.
        /// </summary>
        public string PayloadCleanSpot
        {
            get => GetValue<string>("payload_clean_spot", default);
            set => SetValue("payload_clean_spot", value);
        }

        /// <summary>
        /// The payload to send to the `command_topic` to locate the vacuum (typically plays a song).
        /// </summary>
        public string PayloadLocate
        {
            get => GetValue<string>("payload_locate", default);
            set => SetValue("payload_locate", value);
        }

        /// <summary>
        /// The payload to send to the `command_topic` to pause the vacuum.
        /// </summary>
        public string PayloadPause
        {
            get => GetValue<string>("payload_pause", default);
            set => SetValue("payload_pause", value);
        }

        /// <summary>
        /// The payload to send to the `command_topic` to tell the vacuum to return to base.
        /// </summary>
        public string PayloadReturnToBase
        {
            get => GetValue<string>("payload_return_to_base", default);
            set => SetValue("payload_return_to_base", value);
        }

        /// <summary>
        /// The payload to send to the `command_topic` to begin the cleaning cycle.
        /// </summary>
        public string PayloadStart
        {
            get => GetValue<string>("payload_start", default);
            set => SetValue("payload_start", value);
        }

        /// <summary>
        /// The payload to send to the `command_topic` to stop cleaning.
        /// </summary>
        public string PayloadStop
        {
            get => GetValue<string>("payload_stop", default);
            set => SetValue("payload_stop", value);
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
        /// The schema to use. Must be `state` to select the state schema.
        /// </summary>
        public string Schema
        {
            get => GetValue<string>("schema", default);
            set => SetValue("schema", value);
        }

        /// <summary>
        /// The MQTT topic to publish custom commands to the vacuum.
        /// </summary>
        public string SendCommandTopic
        {
            get => GetValue<string>("send_command_topic", default);
            set => SetValue("send_command_topic", value);
        }

        /// <summary>
        /// The MQTT topic to publish commands to control the vacuum's fan speed.
        /// </summary>
        public string SetFanSpeedTopic
        {
            get => GetValue<string>("set_fan_speed_topic", default);
            set => SetValue("set_fan_speed_topic", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive state messages from the vacuum. Messages received on the `state_topic` must be a valid JSON dictionary, with a mandatory `state` key and optionally `battery_level` and `fan_speed` keys as shown in the [example](#state-mqtt-protocol).
        /// </summary>
        public string StateTopic
        {
            get => GetValue<string>("state_topic", default);
            set => SetValue("state_topic", value);
        }

        /// <summary>
        /// List of features that the vacuum supports (possible values are `start`, `stop`, `pause`, `return_home`, `battery`, `status`, `locate`, `clean_spot`, `fan_speed`, `send_command`).
        /// </summary>
        public string[] SupportedFeatures
        {
            get => GetValue<string[]>("supported_features", default);
            set => SetValue("supported_features", value);
        }
    }
}