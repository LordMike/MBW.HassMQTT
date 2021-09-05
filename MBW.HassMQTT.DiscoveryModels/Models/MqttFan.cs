using System.Collections.Generic;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/fan.mqtt/
    ///
    /// The mqtt fan platform lets you control your MQTT enabled fans.
    /// </summary>
    /// <remarks>
    /// In an ideal scenario, the MQTT device will have a state_topic to publish state changes.
    /// If these messages are published with a RETAIN flag, the MQTT fan will receive an instant
    /// state update after subscription and will start with the correct state. Otherwise, the initial
    /// state of the fan will be false / off.
    /// 
    /// When a state_topic is not available, the fan will work in optimistic mode.In this mode, the
    /// fan will immediately change state after every command.Otherwise, the fan will wait for state
    /// confirmation from the device (message from state_topic).
    /// 
    /// Optimistic mode can be forced even if a state_topic is available. Try to enable it if you are
    /// experiencing incorrect fan operation.
    /// </remarks>
    [DeviceType(HassDeviceType.Fan)]
    [PublicAPI]
    public class MqttFan : MqttSensorDiscoveryBase, IHasUniqueId, IHasAvailability, IHasQos, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasRetain
    {
        public MqttFan(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the fan state.
        /// </summary>
        public string CommandTopic { get; set; }

        /// <summary>
        /// The name of the fan.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Flag that defines if lock works in optimistic mode
        /// </summary>
        public bool Optimistic { get; set; }

        /// <summary>
        /// The MQTT topic to publish commands to change the oscillation state.
        /// </summary>
        public string OscillationCommandTopic { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive oscillation state updates.
        /// </summary>
        public string OscillationStateTopic { get; set; }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract a value from the oscillation.
        /// </summary>
        public string OscillationValueTemplate { get; set; }

        /// <summary>
        /// The payload that represents the fan's high speed.
        /// </summary>
        public string PayloadHighSpeed { get; set; }

        /// <summary>
        /// The payload that represents the fan's low speed.
        /// </summary>
        public string PayloadLowSpeed { get; set; }

        /// <summary>
        /// The payload that represents the fan's medium speed.
        /// </summary>
        public string PayloadMediumSpeed { get; set; }

        /// <summary>
        /// The payload that represents the stop state.
        /// </summary>
        public string PayloadOff { get; set; }

        /// <summary>
        /// The payload that represents the running state.
        /// </summary>
        public string PayloadOn { get; set; }

        /// <summary>
        /// The payload that represents the oscillation off state.
        /// </summary>
        public string PayloadOscillationOff { get; set; }

        /// <summary>
        /// The payload that represents the oscillation on state.
        /// </summary>
        public string PayloadOscillationOn { get; set; }

        /// <summary>
        /// The MQTT topic to publish commands to change speed state.
        /// </summary>
        public string SpeedCommandTopic { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive speed state updates.
        /// </summary>
        public string SpeedStateTopic { get; set; }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract a value from the speed payload.
        /// </summary>
        public string SpeedValueTemplate { get; set; }

        /// <summary>
        /// List of speeds this fan is capable of running at. Valid entries are `off`, `low`, `medium` and `high`.
        /// </summary>
        public string[] Speeds { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive state updates.
        /// </summary>
        public string StateTopic { get; set; }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract a value from the state.
        /// </summary>
        public string StateValueTemplate { get; set; }

        public string UniqueId { get; set; }
        public IList<AvailabilityModel> Availability { get; set; }
        public AvailabilityMode? AvailabilityMode { get; set; }
        public MqttQosLevel Qos { get; set; }
        public string JsonAttributesTemplate { get; set; }
        public string JsonAttributesTopic { get; set; }
        public string Icon { get; set; }
        public bool? EnabledByDefault { get; set; }
        public bool Retain { get; set; }
    }
}