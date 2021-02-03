using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/vacuum.mqtt/#legacy-configuration
    /// </summary>
    [DeviceType(HassDeviceType.Vacuum)]
    [PublicAPI]
    public class MqttVacuumLegacy : MqttEntitySensorDiscoveryBase
    {
        public MqttVacuumLegacy(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// Defines a [template](/topics/templating/) to define the battery level of the vacuum. This is required if `battery_level_topic` is set.
        /// </summary>
        public string BatteryLevelTemplate { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive battery level values from the vacuum.
        /// </summary>
        public string BatteryLevelTopic { get; set; }

        /// <summary>
        /// Defines a [template](/topics/templating/) to define the charging state of the vacuum. This is required if `charging_topic` is set.
        /// </summary>
        public string ChargingTemplate { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive charging state values from the vacuum.
        /// </summary>
        public string ChargingTopic { get; set; }

        /// <summary>
        /// Defines a [template](/topics/templating/) to define the cleaning state of the vacuum. This is required if `cleaning_topic` is set.
        /// </summary>
        public string CleaningTemplate { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive cleaning state values from the vacuum.
        /// </summary>
        public string CleaningTopic { get; set; }

        /// <summary>
        /// The MQTT topic to publish commands to control the vacuum.
        /// </summary>
        public string CommandTopic { get; set; }

        /// <summary>
        /// Defines a [template](/topics/templating/) to define the docked state of the vacuum. This is required if `docked_topic` is set.
        /// </summary>
        public string DockedTemplate { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive docked state values from the vacuum.
        /// </summary>
        public string DockedTopic { get; set; }

        /// <summary>
        /// Defines a [template](/topics/templating/) to define potential error messages emitted by the vacuum. This is required if `error_topic` is set.
        /// </summary>
        public string ErrorTemplate { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive error messages from the vacuum.
        /// </summary>
        public string ErrorTopic { get; set; }

        /// <summary>
        /// List of possible fan speeds for the vacuum.
        /// </summary>
        public string[] FanSpeedList { get; set; }

        /// <summary>
        /// Defines a [template](/topics/templating/) to define the fan speed of the vacuum. This is required if `fan_speed_topic` is set.
        /// </summary>
        public string FanSpeedTemplate { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive fan speed values from the vacuum.
        /// </summary>
        public string FanSpeedTopic { get; set; }

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
        /// The payload to send to the `command_topic` to tell the vacuum to return to base.
        /// </summary>
        public string PayloadReturnToBase { get; set; }

        /// <summary>
        /// The payload to send to the `command_topic` to start or pause the vacuum.
        /// </summary>
        public string PayloadStartPause { get; set; }

        /// <summary>
        /// The payload to send to the `command_topic` to stop the vacuum.
        /// </summary>
        public string PayloadStop { get; set; }

        /// <summary>
        /// The payload to send to the `command_topic` to turn the vacuum off.
        /// </summary>
        public string PayloadTurnOff { get; set; }

        /// <summary>
        /// The payload to send to the `command_topic` to begin the cleaning cycle.
        /// </summary>
        public string PayloadTurnOn { get; set; }

        /// <summary>
        /// The maximum QoS level of the state topic.
        /// </summary>
        public MqttQosLevel Qos { get; set; }

        /// <summary>
        /// If the published message should have the retain flag on or not.
        /// </summary>
        public bool Retain { get; set; }

        /// <summary>
        /// The schema to use. Must be `legacy` or omitted to select the legacy schema.
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
        /// List of features that the vacuum supports (possible values are `turn_on`, `turn_off`, `pause`, `stop`, `return_home`, `battery`, `status`, `locate`, `clean_spot`, `fan_speed`, `send_command`).
        /// </summary>
        public string[] SupportedFeatures { get; set; }
    }
}