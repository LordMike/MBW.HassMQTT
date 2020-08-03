using MBW.HassMQTT.DiscoveryModels;

namespace MBW.HassMQTT.Interfaces
{
    public interface ISensorContainer
    {
        string DeviceId { get; }
        string EntityId { get; }
        internal HassMqttManager HassMqttManager { get; }

        internal MqttSensorDiscoveryBase Discovery { get; }
    }
}