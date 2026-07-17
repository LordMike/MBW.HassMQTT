#nullable enable

namespace MBW.HassMQTT.DiscoveryModels.Interfaces;

public interface IHasEntityPicture
{
    /// <summary>
    /// The picture URL for the entity.
    /// </summary>
    string? EntityPicture { get; set; }
}
