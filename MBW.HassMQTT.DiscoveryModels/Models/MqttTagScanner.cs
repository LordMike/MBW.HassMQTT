#nullable enable
using FluentValidation;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/tag.mqtt/
    ///
    /// The mqtt tag scanner platform uses an MQTT message payload to generate tag scanned events.
    /// </summary>
    [DeviceType(HassDeviceType.TagScanner)]
    [PublicAPI]
    public class MqttTagScanner : MqttSensorDiscoveryBase<MqttTagScanner, MqttTagScanner.MqttTagScannerValidator>
    {
        public MqttTagScanner(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// The MQTT topic subscribed to receive tag scanned events.
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) that returns a tag ID.
        /// </summary>
        public string? ValueTemplate { get; set; }

        public class MqttTagScannerValidator : MqttSensorDiscoveryBaseValidator<MqttTagScanner>
        {
            public MqttTagScannerValidator()
            {
                TopicAndTemplate(s => s.Topic, s => s.ValueTemplate);
            }
        }
    }
}