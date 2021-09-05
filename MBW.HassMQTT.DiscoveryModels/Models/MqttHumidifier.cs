using System.Collections.Generic;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/humidifier.mqtt/
    ///
    /// The mqtt humidifier platform lets you control your MQTT enabled humidifiers.
    /// </summary>
    /// <remarks>
    /// In an ideal scenario, the MQTT device will have a state_topic to publish state changes.
    /// If these messages are published with a RETAIN flag, the MQTT humidifier will receive an instant
    /// state update after subscription and will start with the correct state. Otherwise, the initial
    /// state of the humidifier will be false / off.
    /// 
    /// When a state_topic is not available, the humidifier will work in optimistic mode.In this mode,
    /// the humidifier will immediately change state after every command.Otherwise, the humidifier will
    /// wait for state confirmation from the device (message from state_topic).
    /// 
    /// Optimistic mode can be forced even if a state_topic is available.Try to enable it if you are
    /// experiencing incorrect humidifier operation.
    /// </remarks>
    [DeviceType(HassDeviceType.Humidifier)]
    [PublicAPI]
    public class MqttHumidifier : MqttSensorDiscoveryBase, IHasUniqueId, IHasAvailability, IHasQos, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasRetain
    {
        public MqttHumidifier(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

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