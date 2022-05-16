#nullable enable

namespace MBW.HassMQTT.DiscoveryModels.Interfaces;

public interface IHasUniqueId
{
    /// <summary>
    /// An ID that uniquely identifies this button entity. If two entities have the same unique ID, Home Assistant will raise an exception.
    /// </summary>
    public string? UniqueId { get; set; }
}