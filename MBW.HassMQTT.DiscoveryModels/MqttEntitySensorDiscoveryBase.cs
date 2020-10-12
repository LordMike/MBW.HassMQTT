using MBW.HassMQTT.DiscoveryModels.Interfaces;

namespace MBW.HassMQTT.DiscoveryModels
{
    /// <summary>
    /// All MQTT entity discovery types should inherit from this.
    /// Examples that won't inherit from it: device_trigger, tag
    /// https://www.home-assistant.io/docs/mqtt/discovery/
    /// </summary>
    public abstract class MqttEntitySensorDiscoveryBase : MqttSensorDiscoveryBase, IHasAttributesTopic, IHasAvailabilityTopic
    {
        protected MqttEntitySensorDiscoveryBase(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
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

        /// <inheritdoc cref="IHasAvailabilityTopic.AvailabilityTopic"/>
        public string AvailabilityTopic
        {
            get => GetValue<string>("availability_topic", default);
            set => SetValue("availability_topic", value);
        }

        /// <inheritdoc cref="IHasAvailabilityTopic.PayloadAvailable"/>
        public string PayloadAvailable
        {
            get => GetValue<string>("payload_available", default);
            set => SetValue("payload_available", value);
        }

        /// <inheritdoc cref="IHasAvailabilityTopic.PayloadNotAvailable"/>
        public string PayloadNotAvailable
        {
            get => GetValue<string>("payload_not_available", default);
            set => SetValue("payload_not_available", value);
        }
    }
}