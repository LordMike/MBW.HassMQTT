using MBW.HassMQTT.DiscoveryModels;
using MBW.HassMQTT.Interfaces;

namespace MBW.HassMQTT.Internal
{
    public class DiscoveryDocumentBuilder<TEntity> : IDiscoveryDocumentBuilder<TEntity>, ISensorContainer where TEntity : MqttSensorDiscoveryBase
    {
        private HassMqttManager _hassMqttManager;

        public string DeviceId { get; internal set; }

        public string EntityId { get; internal set; }

        HassMqttManager ISensorContainer.HassMqttManager => _hassMqttManager;
        HassMqttManager IDiscoveryDocumentBuilder.HassMqttManager => _hassMqttManager;

        MqttSensorDiscoveryBase ISensorContainer.Discovery => Discovery;
        MqttSensorDiscoveryBase IDiscoveryDocumentBuilder.Discovery => Discovery;

        public TEntity Discovery { get; internal set; }

        public DiscoveryDocumentBuilder(HassMqttManager hassMqttManager)
        {
            _hassMqttManager = hassMqttManager;
        }
    }
}