using EnumsNET;
using MBW.HassMQTT.DiscoveryModels.Helpers;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.Topics;

namespace MBW.HassMQTT.Extensions;

public static class HassMqttTopicBuilderExtensions
{
    public static string GetDiscoveryTopic<TEntity>(this HassMqttTopicBuilder topicBuilder, string deviceId, string entityId) where TEntity : IHassDiscoveryDocument
    {
        // homeassistant/<sensor>/<my_device>/<my_entity>/config
        return topicBuilder.GetDiscoveryTopic(DiscoveryHelper.GetDeviceType<TEntity>().AsString(EnumFormat.EnumMemberValue), deviceId, entityId, "config");
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