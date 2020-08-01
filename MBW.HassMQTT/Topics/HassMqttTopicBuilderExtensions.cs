using EnumsNET;
using MBW.HassMQTT.DiscoveryModels;
using MBW.HassMQTT.DiscoveryModels.Helpers;

namespace MBW.HassMQTT.Topics
{
    public static class HassMqttTopicBuilderExtensions
    {
        public static string GetDiscoveryTopic<TEntity>(this HassMqttTopicBuilder topicBuilder, string deviceId, string entityId) where TEntity : MqttSensorDiscoveryBase
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
}