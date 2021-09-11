using MBW.HassMQTT.DiscoveryModels.Enum;

namespace MBW.HassMQTT.DiscoveryModels.Interfaces
{
    public interface IHasQos
    {
        /// <summary>
        /// The maximum QoS level of the state topic.
        /// </summary>
        MqttQosLevel? Qos { get; set; }
    }
}