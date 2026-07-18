using MBW.HassMQTT.DiscoveryModels.Enum;

namespace MBW.HassMQTT.Interfaces;

/// <summary>An immutable built entity with mutable runtime value sources.</summary>
public interface IHassMqttEntity
{
    string DeviceId { get; }
    string EntityId { get; }
    string UniqueId { get; }

    /// <summary>Gets a configured runtime value sender.</summary>
    MqttStateValueTopic GetValueSender(HassTopicKind topicKind);

    /// <summary>Gets the configured JSON attributes sender.</summary>
    MqttAttributesTopic GetAttributesSender();
}
