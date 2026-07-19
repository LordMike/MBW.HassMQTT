#nullable enable
namespace MBW.HassMQTT.DiscoveryModels.Interfaces;

public interface IHasName
{
    /// <summary>
    /// The name of the entity. Leave unset to use Home Assistant's default, or explicitly set
    /// <see langword="null" /> when only the device name is relevant.
    /// </summary>
    public Optional<string?> Name { get; set; }
}
