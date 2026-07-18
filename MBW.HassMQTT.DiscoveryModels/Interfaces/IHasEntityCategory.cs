#nullable enable

namespace MBW.HassMQTT.DiscoveryModels.Interfaces;

public interface IHasEntityCategory
{
    /// <summary>
    /// The Home Assistant category of the entity, such as configuration or diagnostic.
    /// </summary>
    public EntityCategory? EntityCategory { get; set; }
}
