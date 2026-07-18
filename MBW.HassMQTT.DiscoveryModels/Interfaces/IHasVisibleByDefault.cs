#nullable enable

namespace MBW.HassMQTT.DiscoveryModels.Interfaces;

public interface IHasVisibleByDefault
{
    /// <summary>
    /// Controls whether the entity is visible by default. A value of <see langword="false"/> hides the entity
    /// from dashboards until it is made visible in its settings.
    /// </summary>
    /// <remarks>The Home Assistant default is <see langword="true"/>.</remarks>
    bool? VisibleByDefault { get; set; }
}
