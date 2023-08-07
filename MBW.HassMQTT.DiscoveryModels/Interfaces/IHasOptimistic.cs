#nullable enable
namespace MBW.HassMQTT.DiscoveryModels.Interfaces;

public interface IHasOptimistic
{
    /// <summary>
    /// Flag that defines if this entity works in optimistic mode.
    /// </summary>
    public bool? Optimistic { get; set; }
}