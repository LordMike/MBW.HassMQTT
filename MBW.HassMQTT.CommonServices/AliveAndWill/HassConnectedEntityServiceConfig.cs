using System.Reflection;

namespace MBW.HassMQTT.CommonServices.AliveAndWill
{
    public class HassConnectedEntityServiceConfig
    {
        public string DeviceId { get; set; }
        public string EntityId { get; set; } = "status";

        public string OkMessage { get; set; } = "ok";
        public string ProblemMessage { get; set; } = "problem";

        public string DiscoveryDeviceName { get; set; }
        public string DiscoveryEntityName { get; set; }

        public HassConnectedEntityServiceConfig()
        {
            DeviceId = Assembly.GetEntryAssembly()?.GetName().Name ?? "TODO_ReplaceMe_HassConnectedEntityServiceConfig";
            DiscoveryDeviceName = DeviceId;
            DiscoveryEntityName = $"{DeviceId} Status";
        }
    }
}