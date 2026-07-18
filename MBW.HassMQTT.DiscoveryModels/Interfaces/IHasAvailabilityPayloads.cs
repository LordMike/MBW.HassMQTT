#nullable enable

namespace MBW.HassMQTT.DiscoveryModels.Interfaces;

public interface IHasAvailabilityPayloads
{
    /// <summary>
    /// The payload that represents the available state.
    /// </summary>
    /// <remarks>The Home Assistant default is <c>online</c>.</remarks>
    string? PayloadAvailable { get; set; }

    /// <summary>
    /// The payload that represents the unavailable state.
    /// </summary>
    /// <remarks>The Home Assistant default is <c>offline</c>.</remarks>
    string? PayloadNotAvailable { get; set; }
}
