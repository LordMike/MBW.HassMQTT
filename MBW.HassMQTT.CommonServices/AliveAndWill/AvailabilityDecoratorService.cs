using MBW.HassMQTT.DiscoveryModels;

namespace MBW.HassMQTT.CommonServices.AliveAndWill
{
    public class AvailabilityDecoratorService
    {
        private readonly HassConnectedEntityService _connectedEntityService;

        public AvailabilityDecoratorService(HassConnectedEntityService connectedEntityService)
        {
            _connectedEntityService = connectedEntityService;
        }

        public void ApplyAvailabilityInformation(MqttEntitySensorDiscoveryBase discovery)
        {
            discovery.AvailabilityTopic = _connectedEntityService.StateTopic;
            discovery.PayloadAvailable = _connectedEntityService.OkMessage;
            discovery.PayloadNotAvailable = _connectedEntityService.ProblemMessage;
        }
    }
}