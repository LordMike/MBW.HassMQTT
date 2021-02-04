using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/number.mqtt/
    /// </summary>
    [DeviceType(HassDeviceType.Number)]
    [PublicAPI]
    public class MqttNumber : MqttEntitySensorDiscoveryBase
    {
        public MqttNumber(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }
        
        /// <summary>
        /// The MQTT topic to publish commands to change the number.
        /// </summary>
        public string CommandTopic { get; set; }

        /// <summary>
        /// Icon for the number.
        /// </summary>
        public string Icon { get; set; }

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
        /// The maximum QoS level of the state topic. Default is 0 and will also be used to publishing messages.
        /// </summary>
        public MqttQosLevel Qos { get; set; }

        /// <summary>
        /// If the published message should have the retain flag on or not.
        /// </summary>
        public bool Retain { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive number values.
        /// </summary>
        public string StateTopic { get; set; }
    }
}