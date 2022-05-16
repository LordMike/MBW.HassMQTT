using MBW.HassMQTT.DiscoveryModels.Interfaces;

namespace MBW.HassMQTT.Interfaces;

public interface IDiscoveryDocumentBuilder
{
    string DeviceId { get; }
    string EntityId { get; }
    internal HassMqttManager HassMqttManager { get; }

    internal IHassDiscoveryDocument DiscoveryUntyped { get; }
}
    
public interface IDiscoveryDocumentBuilder<TEntity> : IDiscoveryDocumentBuilder where TEntity : IHassDiscoveryDocument
{
    TEntity Discovery { get; }
}