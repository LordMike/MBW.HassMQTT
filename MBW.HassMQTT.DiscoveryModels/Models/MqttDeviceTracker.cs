using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/device_tracker.mqtt/
    /// </summary>
    [DeviceType(HassDeviceType.DeviceTracker)]
    [PublicAPI]
    public class MqttDeviceTracker : MqttEntitySensorDiscoveryBase
    {
        public MqttDeviceTracker(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }
        
        /// <summary>
        /// The name of the MQTT device_tracker.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The icon for the device tracker.
        /// </summary>
        public string Icon { get; set; }
        
        /// <summary>
        /// State topic
        /// </summary>
        public string StateTopic { get; set; }

        /// <summary>
        /// Defines a template that returns a device tracker state.
        /// </summary>
        public string ValueTemplate { get; set; }

        /// <summary>
        /// The QoS level of the topic.
        /// </summary>
        public MqttQosLevel Qos { get; set; }

        /// <summary>
        /// The payload value that represents the ‘home’ state for the device.
        /// </summary>
        public string PayloadHome { get; set; }

        /// <summary>
        /// The payload value that represents the ‘not_home’ state for the device.
        /// </summary>
        public string PayloadNotHome { get; set; }

        /// <summary>
        /// Attribute of a device tracker that affects state when being used to track a person. Valid options are gps, router, bluetooth, or bluetooth_le.
        /// </summary>
        public DeviceTrackerSourceType? SourceType { get; set; }
    }
}