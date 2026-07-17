#nullable enable
namespace MBW.HassMQTT.DiscoveryModels.Interfaces;

public interface IHasName
{
    /// <summary>
    /// The name of the entity. Can be <see langword="null" /> when only the device name is relevant.
    /// </summary>
    public string? Name { get; set; }
}
