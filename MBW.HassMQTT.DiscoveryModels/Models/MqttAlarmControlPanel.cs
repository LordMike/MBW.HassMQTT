using MBW.HassMQTT.DiscoveryModels.Enum;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/alarm_control_panel.mqtt/
    /// </summary>
    [DeviceType(HassDeviceType.AlarmControlPanel)]
    public class MqttAlarmControlPanel : MqttEntitySensorDiscoveryBase
    {
        public MqttAlarmControlPanel(string topic, string uniqueId) : base(topic, uniqueId)
        {
        }

        /// <summary>
        /// If defined, specifies a code to enable or disable the alarm in the frontend.
        /// </summary>
        public string Code
        {
            get => GetValue<string>("code", default);
            set => SetValue("code", value);
        }

        /// <summary>
        /// If true the code is required to arm the alarm. If false the code is not validated.
        /// </summary>
        public bool CodeArmRequired
        {
            get => GetValue<bool>("code_arm_required", default);
            set => SetValue("code_arm_required", value);
        }

        /// <summary>
        /// If true the code is required to disarm the alarm. If false the code is not validated.
        /// </summary>
        public bool CodeDisarmRequired
        {
            get => GetValue<bool>("code_disarm_required", default);
            set => SetValue("code_disarm_required", value);
        }

        /// <summary>
        /// The [template](/docs/configuration/templating/#processing-incoming-data) used for the command payload. Available variables: `action` and `code`.
        /// </summary>
        public string CommandTemplate
        {
            get => GetValue<string>("command_template", default);
            set => SetValue("command_template", value);
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the alarm state.
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
        /// The name of the alarm.
        /// </summary>
        public string Name
        {
            get => GetValue<string>("name", default);
            set => SetValue("name", value);
        }

        /// <summary>
        /// The payload to set armed-away mode on your Alarm Panel.
        /// </summary>
        public string PayloadArmAway
        {
            get => GetValue<string>("payload_arm_away", default);
            set => SetValue("payload_arm_away", value);
        }

        /// <summary>
        /// The payload to set armed-home mode on your Alarm Panel.
        /// </summary>
        public string PayloadArmHome
        {
            get => GetValue<string>("payload_arm_home", default);
            set => SetValue("payload_arm_home", value);
        }

        /// <summary>
        /// The payload to set armed-night mode on your Alarm Panel.
        /// </summary>
        public string PayloadArmNight
        {
            get => GetValue<string>("payload_arm_night", default);
            set => SetValue("payload_arm_night", value);
        }

        /// <summary>
        /// The payload to set armed-custom-bypass mode on your Alarm Panel.
        /// </summary>
        public string PayloadArmCustomBypass
        {
            get => GetValue<string>("payload_arm_custom_bypass", default);
            set => SetValue("payload_arm_custom_bypass", value);
        }

        /// <summary>
        /// The payload to disarm your Alarm Panel.
        /// </summary>
        public string PayloadDisarm
        {
            get => GetValue<string>("payload_disarm", default);
            set => SetValue("payload_disarm", value);
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
        /// The MQTT topic subscribed to receive state updates.
        /// </summary>
        public string StateTopic
        {
            get => GetValue<string>("state_topic", default);
            set => SetValue("state_topic", value);
        }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the value.
        /// </summary>
        public string ValueTemplate
        {
            get => GetValue<string>("value_template", default);
            set => SetValue("value_template", value);
        }
    }
}