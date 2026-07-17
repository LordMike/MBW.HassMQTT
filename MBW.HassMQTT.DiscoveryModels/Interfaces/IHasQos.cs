using MBW.HassMQTT.DiscoveryModels.Enum;

namespace MBW.HassMQTT.DiscoveryModels.Interfaces;

public interface IHasQos
{
    /// <summary>
    /// The maximum MQTT quality-of-service level used when receiving and publishing messages.
    /// </summary>
    /// <remarks>The Home Assistant default is QoS level 0.</remarks>
    MqttQosLevel? Qos { get; set; }
}
