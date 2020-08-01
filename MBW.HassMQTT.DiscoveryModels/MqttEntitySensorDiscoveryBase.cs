namespace MBW.HassMQTT.DiscoveryModels
{
    /// <summary>
    /// All MQTT entity discovery types should inherit from this.
    /// Examples that won't inherit from it: device_trigger
    /// https://www.home-assistant.io/docs/mqtt/discovery/
    /// </summary>
    public abstract class MqttEntitySensorDiscoveryBase : MqttSensorDiscoveryBase
    {
        protected MqttEntitySensorDiscoveryBase(string topic, string uniqueId) : base(topic, uniqueId)
        {
        }

        /// <summary>
        /// The MQTT topic subscribed to receive availability (online/offline) updates.
        /// </summary>
        public string AvailabilityTopic
        {
            get => GetValue<string>("availability_topic", default);
            set => SetValue("availability_topic", value);
        }

        /// <summary>
        /// The payload that represents the available state.
        /// </summary>
        public string PayloadAvailable
        {
            get => GetValue<string>("payload_available", default);
            set => SetValue("payload_available", value);
        }

        /// <summary>
        /// The payload that represents the unavailable state.
        /// </summary>
        public string PayloadNotAvailable
        {
            get => GetValue<string>("payload_not_available", default);
            set => SetValue("payload_not_available", value);
        }
    }
}