using MBW.HassMQTT.DiscoveryModels;
using MBW.HassMQTT.Interfaces;

namespace MBW.HassMQTT.Internal
{
    internal class DiscoveryDocumentBuilder<TEntity> : IDiscoveryDocumentBuilder<TEntity>, ISensorContainer where TEntity : MqttSensorDiscoveryBase
    {
        private HassMqttManager _hassMqttManager;

        public string DeviceId { get; internal set; }

        public string EntityId { get; internal set; }

        HassMqttManager ISensorContainer.HassMqttManager => _hassMqttManager;
        HassMqttManager IDiscoveryDocumentBuilder.HassMqttManager => _hassMqttManager;

        MqttSensorDiscoveryBase ISensorContainer.Discovery => Discovery;
        MqttSensorDiscoveryBase IDiscoveryDocumentBuilder.DiscoveryUntyped => Discovery;

        public TEntity Discovery { get; internal set; }

        internal DiscoveryDocumentBuilder(HassMqttManager hassMqttManager)
        {
            _hassMqttManager = hassMqttManager;
        }
    }
}