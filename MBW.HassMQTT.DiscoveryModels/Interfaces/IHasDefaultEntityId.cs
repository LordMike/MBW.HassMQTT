#nullable enable

namespace MBW.HassMQTT.DiscoveryModels.Interfaces;

public interface IHasDefaultEntityId
{
    /// <summary>
    /// The entity ID used instead of the name when Home Assistant automatically generates the entity ID.
    /// When used with a unique ID, this is used only when the entity is added for the first time.
    /// </summary>
    string? DefaultEntityId { get; set; }
}
