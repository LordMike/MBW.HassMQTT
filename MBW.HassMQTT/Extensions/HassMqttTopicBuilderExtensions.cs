using EnumsNET;
using System;
using MBW.HassMQTT.DiscoveryModels.Helpers;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.Topics;

namespace MBW.HassMQTT.Extensions;

public static class HassMqttTopicBuilderExtensions
{
    public static string GetDiscoveryTopic<TEntity>(this HassMqttTopicBuilder topicBuilder, string deviceId, string entityId) where TEntity : IHassDiscoveryDocument
    {
        ValidateDiscoveryId(deviceId, nameof(deviceId));
        ValidateDiscoveryId(entityId, nameof(entityId));

        // homeassistant/<sensor>/<my_device>/<my_entity>/config
        return topicBuilder.GetDiscoveryTopic(DiscoveryHelper.GetDeviceType<TEntity>().AsString(EnumFormat.EnumMemberValue), deviceId, entityId, "config");
    }

    private static void ValidateDiscoveryId(string value, string parameterName)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException("Value cannot be null or empty.", parameterName);

        foreach (char character in value)
        {
            if (character is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9' or '_' or '-')
                continue;

            throw new ArgumentException(
                "Home Assistant MQTT discovery IDs may only contain ASCII letters, digits, underscores, and hyphens.",
                parameterName);
        }
    }

    public static string GetAttributesTopic(this HassMqttTopicBuilder topicBuilder, string deviceId, string entityId)
    {
        // <prefix>/<my_device>/<my_entity>/attributes
        return topicBuilder.GetServiceTopic(deviceId, entityId, "attributes");
    }

    public static string GetEntityTopic(this HassMqttTopicBuilder topicBuilder, string deviceId, string entityId, string kind)
    {
        // <prefix>/<my_device>/<my_entity>/<kind>
        return topicBuilder.GetServiceTopic(deviceId, entityId, kind);
    }
}
