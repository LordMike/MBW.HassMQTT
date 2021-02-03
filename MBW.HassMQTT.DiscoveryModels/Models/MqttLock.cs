using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/lock.mqtt/
    /// </summary>
    [DeviceType(HassDeviceType.Lock)]
    [PublicAPI]
    public class MqttLock : MqttEntitySensorDiscoveryBase
    {
        public MqttLock(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the lock state.
        /// </summary>
        public string CommandTopic { get; set; }

        /// <summary>
        /// The name of the lock.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Flag that defines if lock works in optimistic mode.
        /// </summary>
        public string Optimistic { get; set; }

        /// <summary>
        /// The payload that represents enabled/locked state.
        /// </summary>
        public string PayloadLock { get; set; }

        /// <summary>
        /// The payload that represents disabled/unlocked state.
        /// </summary>
        public string PayloadUnlock { get; set; }

        /// <summary>
        /// The maximum QoS level of the state topic.
        /// </summary>
        public MqttQosLevel Qos { get; set; }

        /// <summary>
        /// If the published message should have the retain flag on or not.
        /// </summary>
        public bool Retain { get; set; }

        /// <summary>
        /// The value that represents the lock to be in locked state
        /// </summary>
        public string StateLocked { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive state updates.
        /// </summary>
        public string StateTopic { get; set; }

        /// <summary>
        /// The value that represents the lock to be in unlocked state
        /// </summary>
        public string StateUnlocked { get; set; }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract a value from the payload.
        /// </summary>
        public string ValueTemplate { get; set; }
    }
}