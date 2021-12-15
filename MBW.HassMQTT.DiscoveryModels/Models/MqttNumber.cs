#nullable enable
using System.Collections.Generic;
using FluentValidation;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/number.mqtt/
    ///
    /// The mqtt Number platform allows you to integrate devices that might expose configuration options through
    /// MQTT into Home Assistant as a Number. Every time a message under the topic in the configuration is received,
    /// the number entity will be updated in Home Assistant and vice-versa, keeping the device and Home Assistant in-sync.
    /// </summary>
    [DeviceType(HassDeviceType.Number)]
    [PublicAPI]
    public class MqttNumber : MqttSensorDiscoveryBase<MqttNumber, MqttNumber.MqttNumberValidator>, IHasUniqueId, IHasAvailability, IHasQos, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasRetain, IHasEntityCategory, IHasObjectId
    {
        public MqttNumber(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// Defines a `template` to generate the payload to send to `command_topic`.
        /// </summary>
        public string? CommandTemplate { get; set; }

        /// <summary>
        /// The MQTT topic to publish commands to change the number.
        /// </summary>
        public string? CommandTopic { get; set; }

        /// <summary>
        /// Minimum value.
        /// </summary>
        public float? Min { get; set; }

        /// <summary>
        /// Maximum value.
        /// </summary>
        public float? Max { get; set; }

        /// <summary>
        /// The name of the Number.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Flag that defines if number works in optimistic mode.
        /// 
        /// Default: true if no state_topic defined, else false.
        /// </summary>
        public bool? Optimistic { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive number values.
        /// </summary>
        public string? StateTopic { get; set; }

        /// <summary>
        /// Step value. Smallest value 0.001.
        /// </summary>
        public float? Step { get; set; }

        /// <summary>
        /// Defines a template to extract the value.
        /// </summary>
        public string? ValueTemplate { get; set; }

        public string? UniqueId { get; set; }
        public IList<AvailabilityModel>? Availability { get; set; }
        public AvailabilityMode? AvailabilityMode { get; set; }
        public MqttQosLevel? Qos { get; set; }
        public string? JsonAttributesTemplate { get; set; }
        public string? JsonAttributesTopic { get; set; }
        public bool? EnabledByDefault { get; set; }
        public string? Icon { get; set; }
        public bool? Retain { get; set; }
        public EntityCategory? EntityCategory { get; set; }
        public string? ObjectId { get; set; }

        public class MqttNumberValidator : MqttSensorDiscoveryBaseValidator<MqttNumber>
        {
            public MqttNumberValidator()
            {
                TopicAndTemplate(s => s.StateTopic, s => s.ValueTemplate);
                TopicAndTemplate(s => s.CommandTopic, s => s.CommandTemplate);

                RuleFor(s => s.Step)
                    .GreaterThanOrEqualTo(0.001f);

                MinMax(s => s.Min, s => s.Max, 1f, 100f);
            }
        }
    }
}