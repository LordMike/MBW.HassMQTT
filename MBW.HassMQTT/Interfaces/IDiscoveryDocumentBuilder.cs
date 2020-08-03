using MBW.HassMQTT.DiscoveryModels;

namespace MBW.HassMQTT.Interfaces
{
    public interface IDiscoveryDocumentBuilder
    {
        string DeviceId { get; }
        string EntityId { get; }
        internal HassMqttManager HassMqttManager { get; }

        internal MqttSensorDiscoveryBase Discovery { get; }
    }
    
    public interface IDiscoveryDocumentBuilder<TEntity> : IDiscoveryDocumentBuilder where TEntity : MqttSensorDiscoveryBase
    {
        TEntity Discovery { get; }
    }
}