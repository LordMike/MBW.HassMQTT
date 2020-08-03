using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/camera.mqtt/
    /// </summary>
    [DeviceType(HassDeviceType.Camera)]
    public class MqttCamera : MqttSensorDiscoveryBase, IHasAttributesTopic
    {
        public MqttCamera(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// The MQTT topic subscribed to receive availability (online/offline) updates.
        /// </summary>
        /// <remarks>This entity does not have the availability payloads</remarks>
        public string AvailabilityTopic
        {
            get => GetValue<string>("availability_topic", default);
            set => SetValue("availability_topic", value);
        }

        /// <inheritdoc cref="IHasAttributesTopic.JsonAttributesTemplate"/>
        public string JsonAttributesTemplate
        {
            get => GetValue<string>("json_attributes_template", default);
            set => SetValue("json_attributes_template", value);
        }

        /// <inheritdoc cref="IHasAttributesTopic.JsonAttributesTopic"/>
        public string JsonAttributesTopic
        {
            get => GetValue<string>("json_attributes_topic", default);
            set => SetValue("json_attributes_topic", value);
        }

        /// <summary>
        /// The name of the camera.
        /// </summary>
        [PublicAPI]
        public string Name
        {
            get => GetValue<string>("name", default);
            set => SetValue("name", value);
        }

        /// <summary>
        /// The MQTT topic to subscribe to.
        /// </summary>
        [PublicAPI]
        public string Topic
        {
            get => GetValue<string>("topic", default);
            set => SetValue("topic", value);
        }
    }
}