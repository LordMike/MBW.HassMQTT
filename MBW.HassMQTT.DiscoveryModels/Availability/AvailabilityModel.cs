using Newtonsoft.Json;

namespace MBW.HassMQTT.DiscoveryModels.Availability
{
    public class AvailabilityModel
    {
        /// <summary>
        /// An MQTT topic subscribed to receive availability (online/offline) updates.
        /// </summary>
        [JsonProperty("topic")]
        public string Topic { get; set; }

        /// <summary>
        /// The payload that represents the available state.
        /// </summary>
        /// <remarks>Default is 'online'</remarks>
        [JsonProperty("payload_available")]
        public string PayloadAvailable { get; set; }

        /// <summary>
        /// The payload that represents the unavailable state.
        /// </summary>
        /// <remarks>Default is 'offline'</remarks>
        [JsonProperty("payload_not_available")]
        public string PayloadNotAvailable { get; set; }
    }
}