using System.Collections.Generic;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/number.mqtt/
    ///
    /// The mqtt Number platform allows you to integrate devices that might expose configuration options through
    /// MQTT into Home Assistant as a Number. Every time a message under the topic in the configuration is received,
    /// the number entity will be updated in Home Assisant and vice-versa, keeping the device and Home Assistant in-sync.
    /// </summary>
    [DeviceType(HassDeviceType.Number)]
    [PublicAPI]
    public class MqttNumber : MqttSensorDiscoveryBase, IHasUniqueId, IHasAvailability, IHasQos, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasRetain
    {
        public MqttNumber(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }
        
        /// <summary>
        /// The MQTT topic to publish commands to change the number.
        /// </summary>
        public string CommandTopic { get; set; }

        /// <summary>
        /// The name of the Number.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Flag that defines if number works in optimistic mode.
        /// 
        /// Default: true if no state_topic defined, else false.
        /// </summary>
        public bool Optimistic { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive number values.
        /// </summary>
        public string StateTopic { get; set; }

        public string UniqueId { get; set; }
        public IList<AvailabilityModel> Availability { get; set; }
        public AvailabilityMode? AvailabilityMode { get; set; }
        public MqttQosLevel Qos { get; set; }
        public string JsonAttributesTemplate { get; set; }
        public string JsonAttributesTopic { get; set; }
        public bool? EnabledByDefault { get; set; }
        public string Icon { get; set; }
        public bool Retain { get; set; }
    }
}