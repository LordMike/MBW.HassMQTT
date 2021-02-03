using System.Collections.Generic;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/scene.mqtt/
    /// </summary>
    [DeviceType(HassDeviceType.Scene)]
    [PublicAPI]
    public class MqttScene : MqttSensorDiscoveryBase, IHasAvailabilityTopic
    {
        public MqttScene(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
            Availability = new List<AvailabilityModel>();
        }

        /// <inheritdoc />
        public IList<AvailabilityModel> Availability { get; set; }

        /// <inheritdoc />
        public AvailabilityMode? AvailabilityMode { get; set; }

        /// <summary>
        /// The MQTT topic to publish commands to change the switch state.
        /// </summary>
        public string CommandTopic { get; set; }

        /// <summary>
        /// Icon for the switch.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// The name to use when displaying this switch.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The payload that represents on state.
        /// If specified, will be used for both comparing to the value in the state_topic (see value_template and state_on for details) and sending as on command to the command_topic.
        /// </summary>
        public string PayloadOn { get; set; }

        /// <summary>
        /// The maximum QoS level to be used when receiving messages.
        /// </summary>
        public MqttQosLevel Qos { get; set; }

        /// <summary>
        /// If the published message should have the retain flag on or not.
        /// </summary>
        public bool Retain { get; set; }
    }
}