using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/tag.mqtt/
    /// </summary>
    [DeviceType(HassDeviceType.TagScanner)]
    public class MqttTagScanner : MqttSensorDiscoveryBase
    {
        public MqttTagScanner(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// The MQTT topic subscribed to receive tag scanned events.
        /// </summary>
        [PublicAPI]
        public string Topic
        {
            get => GetValue<string>("topic", default);
            set => SetValue("topic", value);
        }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) that returns a tag ID.
        /// </summary>
        [PublicAPI]
        public string ValueTemplate
        {
            get => GetValue<string>("value_template", default);
            set => SetValue("value_template", value);
        }
    }
}