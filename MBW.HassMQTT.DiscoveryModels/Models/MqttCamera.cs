using MBW.HassMQTT.DiscoveryModels.Enum;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/camera.mqtt/
    /// </summary>
    [DeviceType(HassDeviceType.Camera)]
    public class MqttCamera : MqttSensorDiscoveryBase
    {
        public MqttCamera(string topic, string uniqueId) : base(topic, uniqueId)
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

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the JSON dictionary from messages received on the `json_attributes_topic`.
        /// </summary>
        public string JsonAttributesTemplate
        {
            get => GetValue<string>("json_attributes_template", default);
            set => SetValue("json_attributes_template", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive a JSON dictionary payload and then set as sensor attributes. Implies `force_update` of the current sensor state when a message is received on this topic.
        /// </summary>
        public string JsonAttributesTopic
        {
            get => GetValue<string>("json_attributes_topic", default);
            set => SetValue("json_attributes_topic", value);
        }

        /// <summary>
        /// The name of the camera.
        /// </summary>
        public string Name
        {
            get => GetValue<string>("name", default);
            set => SetValue("name", value);
        }

        /// <summary>
        /// The MQTT topic to subscribe to.
        /// </summary>
        public string Topic
        {
            get => GetValue<string>("topic", default);
            set => SetValue("topic", value);
        }
    }
}