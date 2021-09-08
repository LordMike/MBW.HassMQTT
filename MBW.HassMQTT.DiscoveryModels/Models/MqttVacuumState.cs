#nullable enable
using System.Collections.Generic;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/vacuum.mqtt/#state-configuration
    /// 
    /// The mqtt vacuum integration allows you to control your MQTT-enabled vacuum. There are two possible message
    /// schemas - legacy and state, chosen by setting the schema configuration parameter. New installations should
    /// use the state schema as legacy is deprecated and might be removed someday in the future. The state schema
    /// is preferred because the vacuum will then be represented as a StateVacuumDevice which is the preferred parent
    /// vacuum entity.
    /// </summary>
    [DeviceType(HassDeviceType.Vacuum)]
    [PublicAPI]
    public class MqttVacuumState : MqttSensorDiscoveryBase, IHasUniqueId, IHasAvailability, IHasQos, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasRetain
    {
        public MqttVacuumState(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// The MQTT topic to publish commands to control the vacuum.
        /// </summary>
        public string? CommandTopic { get; set; }

        /// <summary>
        /// List of possible fan speeds for the vacuum.
        /// </summary>
        public IList<string>? FanSpeedList { get; set; }

        /// <summary>
        /// The name of the vacuum.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The payload to send to the `command_topic` to begin a spot cleaning cycle.
        /// </summary>
        public string? PayloadCleanSpot { get; set; }

        /// <summary>
        /// The payload to send to the `command_topic` to locate the vacuum (typically plays a song).
        /// </summary>
        public string? PayloadLocate { get; set; }

        /// <summary>
        /// The payload to send to the `command_topic` to pause the vacuum.
        /// </summary>
        public string? PayloadPause { get; set; }

        /// <summary>
        /// The payload to send to the `command_topic` to tell the vacuum to return to base.
        /// </summary>
        public string? PayloadReturnToBase { get; set; }

        /// <summary>
        /// The payload to send to the `command_topic` to begin the cleaning cycle.
        /// </summary>
        public string? PayloadStart { get; set; }

        /// <summary>
        /// The payload to send to the `command_topic` to stop cleaning.
        /// </summary>
        public string? PayloadStop { get; set; }

        /// <summary>
        /// The schema to use. Must be `state` to select the state schema.
        /// </summary>
        public string? Schema { get; set; } = "state";

        /// <summary>
        /// The MQTT topic to publish custom commands to the vacuum.
        /// </summary>
        public string? SendCommandTopic { get; set; }

        /// <summary>
        /// The MQTT topic to publish commands to control the vacuum's fan speed.
        /// </summary>
        public string? SetFanSpeedTopic { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive state messages from the vacuum. Messages received on the `state_topic` must be a valid JSON dictionary, with a mandatory `state` key and optionally `battery_level` and `fan_speed` keys as shown in the [example](#state-mqtt-protocol).
        /// </summary>
        public string? StateTopic { get; set; }

        /// <summary>
        /// List of features that the vacuum supports, possible values are:
        /// - start
        /// - stop
        /// - pause
        /// - return_home
        /// - battery
        /// - status
        /// - locate
        /// - clean_spot
        /// - fan_speed
        /// - send_command
        /// </summary>
        public IList<string>? SupportedFeatures { get; set; }

        public string? UniqueId { get; set; }
        public IList<AvailabilityModel>? Availability { get; set; }
        public AvailabilityMode? AvailabilityMode { get; set; }
        public MqttQosLevel? Qos { get; set; }
        public string? JsonAttributesTemplate { get; set; }
        public string? JsonAttributesTopic { get; set; }
        public string? Icon { get; set; }
        public bool? EnabledByDefault { get; set; }
        public bool? Retain { get; set; }
    }
}