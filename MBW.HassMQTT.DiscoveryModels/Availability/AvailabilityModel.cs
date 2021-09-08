#nullable enable

namespace MBW.HassMQTT.DiscoveryModels.Availability
{
    public class AvailabilityModel
    {
        /// <summary>
        /// An MQTT topic subscribed to receive availability (online/offline) updates.
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// The payload that represents the available state.
        /// </summary>
        /// <remarks>Default is 'online'</remarks>
        public string? PayloadAvailable { get; set; }

        /// <summary>
        /// The payload that represents the unavailable state.
        /// </summary>
        /// <remarks>Default is 'offline'</remarks>
        public string? PayloadNotAvailable { get; set; }
    }
}