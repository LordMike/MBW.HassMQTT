#nullable enable
namespace MBW.HassMQTT.DiscoveryModels.Interfaces;

public interface IHasOptimistic
{
    /// <summary>
    /// Controls whether Home Assistant updates the entity state immediately after publishing a command instead of waiting for state confirmation.
    /// </summary>
    public bool? Optimistic { get; set; }
}
