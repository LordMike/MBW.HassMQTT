namespace MBW.HassMQTT.DiscoveryModels.Interfaces
{
    public interface IHasAvailabilityTopic
    {
        /// <summary>
        /// The MQTT topic subscribed to receive availability (online/offline) updates.
        /// </summary>
        string AvailabilityTopic { get; set; }

        /// <summary>
        /// The payload that represents the available state.
        /// </summary>
        string PayloadAvailable { get; set; }

        /// <summary>
        /// The payload that represents the unavailable state.
        /// </summary>
        string PayloadNotAvailable { get; set; }
    }
}