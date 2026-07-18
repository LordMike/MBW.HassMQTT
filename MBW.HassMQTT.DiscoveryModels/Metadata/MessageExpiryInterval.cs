#nullable enable

namespace MBW.HassMQTT.DiscoveryModels.Metadata;

/// <summary>
/// The duration for which queued or retained messages sent from Home Assistant persist at the broker
/// for offline subscribers.
/// </summary>
public class MessageExpiryInterval
{
    /// <summary>
    /// The number of days.
    /// </summary>
    public int? Days { get; set; }

    /// <summary>
    /// The number of hours.
    /// </summary>
    public int? Hours { get; set; }

    /// <summary>
    /// The number of minutes.
    /// </summary>
    public int? Minutes { get; set; }

    /// <summary>
    /// The number of seconds.
    /// </summary>
    public int? Seconds { get; set; }
}
