using MBW.HassMQTT.DiscoveryModels.Extensions;
using MBW.HassMQTT.DiscoveryModels.Interfaces;

namespace MBW.HassMQTT.CommonServices.AliveAndWill
{
    public class AvailabilityDecoratorService
    {
        private readonly HassConnectedEntityService _connectedEntityService;

        public AvailabilityDecoratorService(HassConnectedEntityService connectedEntityService)
        {
            _connectedEntityService = connectedEntityService;
        }

        public void ApplyAvailabilityInformation(IHasAvailability discovery)
        {
            discovery.ConfigureAvailability(_connectedEntityService.StateTopic, _connectedEntityService.OkMessage, _connectedEntityService.ProblemMessage);
        }
    }
}