#nullable enable

namespace MBW.HassMQTT.DiscoveryModels.Interfaces;

public interface IHasUniqueId
{
    /// <summary>
    /// An ID that uniquely identifies the entity. Home Assistant raises an error when two entities have the same unique ID. Required for device-based discovery.
    /// </summary>
    public string? UniqueId { get; set; }
}
