using MBW.HassMQTT.DiscoveryModels.Interfaces;

namespace MBW.HassMQTT.Interfaces;

public interface ISensorContainer
{
    string DeviceId { get; }
    string EntityId { get; }
    internal HassMqttManager HassMqttManager { get; }

    internal IHassDiscoveryDocument Discovery { get; }
}