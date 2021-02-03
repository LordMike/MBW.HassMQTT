using MBW.HassMQTT.DiscoveryModels;
using MBW.HassMQTT.DiscoveryModels.Availability;

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
            discovery.Availability.Add(new AvailabilityModel
            {
                Topic = _connectedEntityService.StateTopic,
                PayloadAvailable = _connectedEntityService.OkMessage,
                PayloadNotAvailable = _connectedEntityService.ProblemMessage
            });
        }
    }
}