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
    /// </summary>
    [DeviceType(HassDeviceType.Camera)]
    [PublicAPI]
    public class MqttCamera : MqttSensorDiscoveryBase, IHasAttributesTopic, IHasAvailabilityTopic
    {
        public MqttCamera(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
            Availability = new List<AvailabilityModel>();
        }

        /// <inheritdoc />
        public IList<AvailabilityModel> Availability { get; set; }

        /// <inheritdoc />
        public AvailabilityMode? AvailabilityMode { get; set; }

        /// <inheritdoc cref="IHasAttributesTopic.JsonAttributesTemplate"/>
        public string JsonAttributesTemplate { get; set; }

        /// <inheritdoc cref="IHasAttributesTopic.JsonAttributesTopic"/>
        public string JsonAttributesTopic { get; set; }

        /// <summary>
        /// The name of the camera.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The MQTT topic to subscribe to.
        /// </summary>
        public string Topic { get; set; }
    }
}