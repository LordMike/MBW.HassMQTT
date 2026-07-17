namespace MBW.HassMQTT.DiscoveryModels.Interfaces;

public interface IHasEnabledByDefault
{
    /// <summary>
    /// Controls whether the entity is enabled when first added.
    /// </summary>
    /// <remarks>The Home Assistant default is <see langword="true" />.</remarks>
    public bool? EnabledByDefault { get; set; }
}
