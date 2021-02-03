using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/vacuum.mqtt/#state-configuration
    /// </summary>
    [DeviceType(HassDeviceType.Vacuum)]
    [PublicAPI]
    public class MqttVacuumState : MqttEntitySensorDiscoveryBase
    {
        public MqttVacuumState(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// The MQTT topic to publish commands to control the vacuum.
        /// </summary>
        public string CommandTopic { get; set; }

        /// <summary>
        /// List of possible fan speeds for the vacuum.
        /// </summary>
        public string[] FanSpeedList { get; set; }

        /// <summary>
        /// The name of the vacuum.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The payload to send to the `command_topic` to begin a spot cleaning cycle.
        /// </summary>
        public string PayloadCleanSpot { get; set; }

        /// <summary>
        /// The payload to send to the `command_topic` to locate the vacuum (typically plays a song).
        /// </summary>
        public string PayloadLocate { get; set; }

        /// <summary>
        /// The payload to send to the `command_topic` to pause the vacuum.
        /// </summary>
        public string PayloadPause { get; set; }

        /// <summary>
        /// The payload to send to the `command_topic` to tell the vacuum to return to base.
        /// </summary>
        public string PayloadReturnToBase { get; set; }

        /// <summary>
        /// The payload to send to the `command_topic` to begin the cleaning cycle.
        /// </summary>
        public string PayloadStart { get; set; }

        /// <summary>
        /// The payload to send to the `command_topic` to stop cleaning.
        /// </summary>
        public string PayloadStop { get; set; }

        /// <summary>
        /// The maximum QoS level of the state topic.
        /// </summary>
        public MqttQosLevel Qos { get; set; }

        /// <summary>
        /// If the published message should have the retain flag on or not.
        /// </summary>
        public bool Retain { get; set; }

        /// <summary>
        /// The schema to use. Must be `state` to select the state schema.
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// The MQTT topic to publish custom commands to the vacuum.
        /// </summary>
        public string SendCommandTopic { get; set; }

        /// <summary>
        /// The MQTT topic to publish commands to control the vacuum's fan speed.
        /// </summary>
        public string SetFanSpeedTopic { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive state messages from the vacuum. Messages received on the `state_topic` must be a valid JSON dictionary, with a mandatory `state` key and optionally `battery_level` and `fan_speed` keys as shown in the [example](#state-mqtt-protocol).
        /// </summary>
        public string StateTopic { get; set; }

        /// <summary>
        /// List of features that the vacuum supports (possible values are `start`, `stop`, `pause`, `return_home`, `battery`, `status`, `locate`, `clean_spot`, `fan_speed`, `send_command`).
        /// </summary>
        public string[] SupportedFeatures { get; set; }
    }
}