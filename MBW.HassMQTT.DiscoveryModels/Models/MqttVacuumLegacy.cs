using MBW.HassMQTT.DiscoveryModels.Enum;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/vacuum.mqtt/#legacy-configuration
    /// </summary>
    [DeviceType(HassDeviceType.Vacuum)]
    public class MqttVacuumLegacy : MqttEntitySensorDiscoveryBase
    {
        public MqttVacuumLegacy(string topic, string uniqueId) : base(topic, uniqueId)
        {
        }

        /// <summary>
        /// Defines a [template](/topics/templating/) to define the battery level of the vacuum. This is required if `battery_level_topic` is set.
        /// </summary>
        public string BatteryLevelTemplate
        {
            get => GetValue<string>("battery_level_template", default);
            set => SetValue("battery_level_template", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive battery level values from the vacuum.
        /// </summary>
        public string BatteryLevelTopic
        {
            get => GetValue<string>("battery_level_topic", default);
            set => SetValue("battery_level_topic", value);
        }

        /// <summary>
        /// Defines a [template](/topics/templating/) to define the charging state of the vacuum. This is required if `charging_topic` is set.
        /// </summary>
        public string ChargingTemplate
        {
            get => GetValue<string>("charging_template", default);
            set => SetValue("charging_template", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive charging state values from the vacuum.
        /// </summary>
        public string ChargingTopic
        {
            get => GetValue<string>("charging_topic", default);
            set => SetValue("charging_topic", value);
        }

        /// <summary>
        /// Defines a [template](/topics/templating/) to define the cleaning state of the vacuum. This is required if `cleaning_topic` is set.
        /// </summary>
        public string CleaningTemplate
        {
            get => GetValue<string>("cleaning_template", default);
            set => SetValue("cleaning_template", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive cleaning state values from the vacuum.
        /// </summary>
        public string CleaningTopic
        {
            get => GetValue<string>("cleaning_topic", default);
            set => SetValue("cleaning_topic", value);
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
        /// Defines a [template](/topics/templating/) to define the docked state of the vacuum. This is required if `docked_topic` is set.
        /// </summary>
        public string DockedTemplate
        {
            get => GetValue<string>("docked_template", default);
            set => SetValue("docked_template", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive docked state values from the vacuum.
        /// </summary>
        public string DockedTopic
        {
            get => GetValue<string>("docked_topic", default);
            set => SetValue("docked_topic", value);
        }

        /// <summary>
        /// Defines a [template](/topics/templating/) to define potential error messages emitted by the vacuum. This is required if `error_topic` is set.
        /// </summary>
        public string ErrorTemplate
        {
            get => GetValue<string>("error_template", default);
            set => SetValue("error_template", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive error messages from the vacuum.
        /// </summary>
        public string ErrorTopic
        {
            get => GetValue<string>("error_topic", default);
            set => SetValue("error_topic", value);
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
        /// Defines a [template](/topics/templating/) to define the fan speed of the vacuum. This is required if `fan_speed_topic` is set.
        /// </summary>
        public string FanSpeedTemplate
        {
            get => GetValue<string>("fan_speed_template", default);
            set => SetValue("fan_speed_template", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive fan speed values from the vacuum.
        /// </summary>
        public string FanSpeedTopic
        {
            get => GetValue<string>("fan_speed_topic", default);
            set => SetValue("fan_speed_topic", value);
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
        /// The payload to send to the `command_topic` to tell the vacuum to return to base.
        /// </summary>
        public string PayloadReturnToBase
        {
            get => GetValue<string>("payload_return_to_base", default);
            set => SetValue("payload_return_to_base", value);
        }

        /// <summary>
        /// The payload to send to the `command_topic` to start or pause the vacuum.
        /// </summary>
        public string PayloadStartPause
        {
            get => GetValue<string>("payload_start_pause", default);
            set => SetValue("payload_start_pause", value);
        }

        /// <summary>
        /// The payload to send to the `command_topic` to stop the vacuum.
        /// </summary>
        public string PayloadStop
        {
            get => GetValue<string>("payload_stop", default);
            set => SetValue("payload_stop", value);
        }

        /// <summary>
        /// The payload to send to the `command_topic` to turn the vacuum off.
        /// </summary>
        public string PayloadTurnOff
        {
            get => GetValue<string>("payload_turn_off", default);
            set => SetValue("payload_turn_off", value);
        }

        /// <summary>
        /// The payload to send to the `command_topic` to begin the cleaning cycle.
        /// </summary>
        public string PayloadTurnOn
        {
            get => GetValue<string>("payload_turn_on", default);
            set => SetValue("payload_turn_on", value);
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
        /// The schema to use. Must be `legacy` or omitted to select the legacy schema.
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
        /// List of features that the vacuum supports (possible values are `turn_on`, `turn_off`, `pause`, `stop`, `return_home`, `battery`, `status`, `locate`, `clean_spot`, `fan_speed`, `send_command`).
        /// </summary>
        public string[] SupportedFeatures
        {
            get => GetValue<string[]>("supported_features", default);
            set => SetValue("supported_features", value);
        }
    }
}