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
    /// https://www.home-assistant.io/integrations/camera.mqtt/
    ///
    /// The mqtt camera platform allows you to integrate the content of an image file sent through
    /// MQTT into Home Assistant as a camera. Every time a message under the topic in the configuration
    /// is received, the image displayed in Home Assistant will also be updated.
    /// 
    /// This can be used with an application or a service capable of sending images through MQTT.
    /// </summary>
    [DeviceType(HassDeviceType.Camera)]
    [PublicAPI]
    public class MqttCamera : MqttSensorDiscoveryBase<MqttCamera, MqttCamera.MqttCameraValidator>, IHasUniqueId, IHasAvailability, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasEntityCategory
    {
        public MqttCamera(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// The name of the camera.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The MQTT topic to subscribe to.
        /// </summary>
        public string Topic { get; set; }

        public string? UniqueId { get; set; }
        public IList<AvailabilityModel>? Availability { get; set; }
        public AvailabilityMode? AvailabilityMode { get; set; }
        public string? JsonAttributesTemplate { get; set; }
        public string? JsonAttributesTopic { get; set; }
        public string? Icon { get; set; }
        public bool? EnabledByDefault { get; set; }
        public EntityCategory? EntityCategory { get; set; }

        public class MqttCameraValidator : MqttSensorDiscoveryBaseValidator<MqttCamera>
        {
            public MqttCameraValidator()
            {
            }
        }
    }
}