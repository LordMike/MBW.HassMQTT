#nullable enable

using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Interfaces;

public interface IHasMessageExpiryInterval
{
    /// <summary>
    /// Controls how long queued or retained messages sent from Home Assistant persist at the broker
    /// for offline subscribers.
    /// </summary>
    MessageExpiryInterval? MessageExpiryInterval { get; set; }
}
