using System.Collections.Generic;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using Newtonsoft.Json;

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
            Availability = new List<AvailabilityModel>();
        }

        /// <inheritdoc />
        [JsonProperty("json_attributes_template")]
        public string JsonAttributesTemplate { get; set; }

        /// <inheritdoc />
        [JsonProperty("json_attributes_topic")]
        public string JsonAttributesTopic { get; set; }

        [JsonProperty("availability")]
        public IList<AvailabilityModel> Availability { get; set; }

        /// <inheritdoc />
        [JsonProperty("availability_mode")]
        public AvailabilityMode? AvailabilityMode { get; set; }
    }
}