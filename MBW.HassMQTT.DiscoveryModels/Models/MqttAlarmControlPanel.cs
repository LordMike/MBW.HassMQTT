using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/alarm_control_panel.mqtt/
    /// </summary>
    [DeviceType(HassDeviceType.AlarmControlPanel)]
    public class MqttAlarmControlPanel : MqttEntitySensorDiscoveryBase
    {
        public MqttAlarmControlPanel(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// If defined, specifies a code to enable or disable the alarm in the frontend.
        /// </summary>
        [PublicAPI]
        public string Code
        {
            get => GetValue<string>("code", default);
            set => SetValue("code", value);
        }

        /// <summary>
        /// If true the code is required to arm the alarm. If false the code is not validated.
        /// </summary>
        [PublicAPI]
        public bool CodeArmRequired
        {
            get => GetValue<bool>("code_arm_required", default);
            set => SetValue("code_arm_required", value);
        }

        /// <summary>
        /// If true the code is required to disarm the alarm. If false the code is not validated.
        /// </summary>
        [PublicAPI]
        public bool CodeDisarmRequired
        {
            get => GetValue<bool>("code_disarm_required", default);
            set => SetValue("code_disarm_required", value);
        }

        /// <summary>
        /// The [template](/docs/configuration/templating/#processing-incoming-data) used for the command payload. Available variables: `action` and `code`.
        /// </summary>
        [PublicAPI]
        public string CommandTemplate
        {
            get => GetValue<string>("command_template", default);
            set => SetValue("command_template", value);
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the alarm state.
        /// </summary>
        [PublicAPI]
        public string CommandTopic
        {
            get => GetValue<string>("command_topic", default);
            set => SetValue("command_topic", value);
        }

        /// <summary>
        /// The name of the alarm.
        /// </summary>
        [PublicAPI]
        public string Name
        {
            get => GetValue<string>("name", default);
            set => SetValue("name", value);
        }

        /// <summary>
        /// The payload to set armed-away mode on your Alarm Panel.
        /// </summary>
        [PublicAPI]
        public string PayloadArmAway
        {
            get => GetValue<string>("payload_arm_away", default);
            set => SetValue("payload_arm_away", value);
        }

        /// <summary>
        /// The payload to set armed-home mode on your Alarm Panel.
        /// </summary>
        [PublicAPI]
        public string PayloadArmHome
        {
            get => GetValue<string>("payload_arm_home", default);
            set => SetValue("payload_arm_home", value);
        }

        /// <summary>
        /// The payload to set armed-night mode on your Alarm Panel.
        /// </summary>
        [PublicAPI]
        public string PayloadArmNight
        {
            get => GetValue<string>("payload_arm_night", default);
            set => SetValue("payload_arm_night", value);
        }

        /// <summary>
        /// The payload to set armed-custom-bypass mode on your Alarm Panel.
        /// </summary>
        [PublicAPI]
        public string PayloadArmCustomBypass
        {
            get => GetValue<string>("payload_arm_custom_bypass", default);
            set => SetValue("payload_arm_custom_bypass", value);
        }

        /// <summary>
        /// The payload to disarm your Alarm Panel.
        /// </summary>
        [PublicAPI]
        public string PayloadDisarm
        {
            get => GetValue<string>("payload_disarm", default);
            set => SetValue("payload_disarm", value);
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
        /// The MQTT topic subscribed to receive state updates.
        /// </summary>
        [PublicAPI]
        public string StateTopic
        {
            get => GetValue<string>("state_topic", default);
            set => SetValue("state_topic", value);
        }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the value.
        /// </summary>
        [PublicAPI]
        public string ValueTemplate
        {
            get => GetValue<string>("value_template", default);
            set => SetValue("value_template", value);
        }
    }
}