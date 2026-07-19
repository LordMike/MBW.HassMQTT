using System;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.Interfaces;

namespace MBW.HassMQTT.Extensions;

public static class EntityExtensions
{
    public static IHassMqttEntity SetAttribute(this IHassMqttEntity entity, string name, MqttValue value)
    {
        entity.GetAttributesSender().SetAttribute(name, value);
        return entity;
    }

    public static IHassMqttEntity SetValue(this IHassMqttEntity entity, HassTopicKind topicKind, MqttValue value)
    {
        entity.GetValueSender(topicKind).Value = value;
        return entity;
    }

    public static IHassMqttEntity GetEntity(this HassMqttManager manager, string deviceId, string entityId)
    {
        if (manager.TryGetEntity(deviceId, entityId, out IHassMqttEntity entity))
            return entity;

        throw new InvalidOperationException($"Unable to find entity {deviceId}/{entityId} - has it been built?");
    }
}
