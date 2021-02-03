using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/alarm_control_panel.mqtt/
    /// </summary>
    [DeviceType(HassDeviceType.AlarmControlPanel)]
    [PublicAPI]
    public class MqttAlarmControlPanel : MqttEntitySensorDiscoveryBase
    {
        public MqttAlarmControlPanel(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// If defined, specifies a code to enable or disable the alarm in the frontend.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// If true the code is required to arm the alarm. If false the code is not validated.
        /// </summary>
        public bool CodeArmRequired { get; set; }

        /// <summary>
        /// If true the code is required to disarm the alarm. If false the code is not validated.
        /// </summary>
        public bool CodeDisarmRequired { get; set; }

        /// <summary>
        /// The [template](/docs/configuration/templating/#processing-incoming-data) used for the command payload. Available variables: `action` and `code`.
        /// </summary>
        public string CommandTemplate { get; set; }

        /// <summary>
        /// The MQTT topic to publish commands to change the alarm state.
        /// </summary>
        public string CommandTopic { get; set; }

        /// <summary>
        /// The name of the alarm.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The payload to set armed-away mode on your Alarm Panel.
        /// </summary>
        public string PayloadArmAway { get; set; }

        /// <summary>
        /// The payload to set armed-home mode on your Alarm Panel.
        /// </summary>
        public string PayloadArmHome { get; set; }

        /// <summary>
        /// The payload to set armed-night mode on your Alarm Panel.
        /// </summary>
        public string PayloadArmNight { get; set; }

        /// <summary>
        /// The payload to set armed-custom-bypass mode on your Alarm Panel.
        /// </summary>
        public string PayloadArmCustomBypass { get; set; }

        /// <summary>
        /// The payload to disarm your Alarm Panel.
        /// </summary>
        public string PayloadDisarm { get; set; }

        /// <summary>
        /// The maximum QoS level of the state topic.
        /// </summary>
        public MqttQosLevel Qos { get; set; }

        /// <summary>
        /// If the published message should have the retain flag on or not.
        /// </summary>
        public bool Retain { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive state updates.
        /// </summary>
        public string StateTopic { get; set; }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the value.
        /// </summary>
        public string ValueTemplate { get; set; }
    }
}