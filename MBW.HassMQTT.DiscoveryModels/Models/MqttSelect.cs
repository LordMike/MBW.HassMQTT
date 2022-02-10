#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/select.mqtt/
    ///
    /// The mqtt Select platform allows you to integrate devices that might expose configuration options through MQTT
    /// into Home Assistant as a Select. Every time a message under the topic in the configuration is received,
    /// the select entity will be updated in Home Assistant and vice-versa, keeping the device and Home Assistant in sync.
    /// </summary>
    [DeviceType(HassDeviceType.Select)]
    [PublicAPI]
    public class MqttSelect : MqttSensorDiscoveryBase<MqttSelect, MqttSelect.MqttSelectValidator>, IHasUniqueId, IHasAvailability, IHasQos, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasRetain, IHasEntityCategory, IHasObjectId, IHasEncoding
    {
        public MqttSelect(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        protected override void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (propertyName == nameof(Options))
            {
                // Extra check as arrays are special
                if (before is List<string> valBefore && after is List<string> valAfter &&
                    valBefore.Count == valAfter.Count &&
                    valBefore.SequenceEqual(valAfter, StringComparer.Ordinal))
                {
                    // These arrays are identical
                    return;
                }
            }

            base.OnPropertyChanged(propertyName, before, after);
        }

        /// <summary>
        /// Defines a `template` to generate the payload to send to `command_topic`.
        /// </summary>
        public string? CommandTemplate { get; set; }

        /// <summary>
        /// The MQTT topic to publish commands to change the selected option.
        /// </summary>
        public string CommandTopic { get; set; }

        /// <summary>
        /// The name of the Select.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Flag that defines if the select works in optimistic mode.
        /// Default: true if no state_topic defined, else false.
        /// </summary>
        public bool? Optimistic { get; set; }

        /// <summary>
        /// List of options that can be selected. An empty list or a list with a single item is allowed.
        /// </summary>
        public IList<string> Options { get; set; } = new List<string>();

        /// <summary>
        /// The MQTT topic subscribed to receive update of the selected option.
        /// </summary>
        public string? StateTopic { get; set; }

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
        public string? Icon { get; set; }
        public bool? EnabledByDefault { get; set; }
        public bool? Retain { get; set; }
        public EntityCategory? EntityCategory { get; set; }
        public string? ObjectId { get; set; }
        public string? Encoding { get; set; }

        public class MqttSelectValidator : MqttSensorDiscoveryBaseValidator<MqttSelect>
        {
            public MqttSelectValidator()
            {
                TopicAndTemplate(s => s.StateTopic, s => s.ValueTemplate);
                TopicAndTemplate(s => s.CommandTopic, s => s.CommandTemplate);

                RuleFor(s => s.Options).NotEmpty();
            }
        }
    }
}