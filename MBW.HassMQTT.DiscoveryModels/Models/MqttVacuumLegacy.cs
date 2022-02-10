#nullable enable
using System;
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
    /// https://www.home-assistant.io/integrations/vacuum.mqtt/#legacy-configuration
    ///
    /// The mqtt vacuum integration allows you to control your MQTT-enabled vacuum. There are two possible message
    /// schemas - legacy and state, chosen by setting the schema configuration parameter. New installations should
    /// use the state schema as legacy is deprecated and might be removed someday in the future. The state schema
    /// is preferred because the vacuum will then be represented as a StateVacuumDevice which is the preferred parent
    /// vacuum entity.
    /// </summary>
    [DeviceType(HassDeviceType.Vacuum)]
    [PublicAPI]
    public class MqttVacuumLegacy : MqttSensorDiscoveryBase<MqttVacuumLegacy, MqttVacuumLegacy.MqttVacuumLegacyValidator>, IHasUniqueId, IHasAvailability, IHasQos, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasRetain, IHasEntityCategory, IHasObjectId, IHasEncoding
    {
        public MqttVacuumLegacy(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// Defines a [template](/topics/templating/) to define the battery level of the vacuum. This is required if `battery_level_topic` is set.
        /// </summary>
        public string? BatteryLevelTemplate { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive battery level values from the vacuum.
        /// </summary>
        public string? BatteryLevelTopic { get; set; }

        /// <summary>
        /// Defines a [template](/topics/templating/) to define the charging state of the vacuum. This is required if `charging_topic` is set.
        /// </summary>
        public string? ChargingTemplate { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive charging state values from the vacuum.
        /// </summary>
        public string? ChargingTopic { get; set; }

        /// <summary>
        /// Defines a [template](/topics/templating/) to define the cleaning state of the vacuum. This is required if `cleaning_topic` is set.
        /// </summary>
        public string? CleaningTemplate { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive cleaning state values from the vacuum.
        /// </summary>
        public string? CleaningTopic { get; set; }

        /// <summary>
        /// The MQTT topic to publish commands to control the vacuum.
        /// </summary>
        public string? CommandTopic { get; set; }

        /// <summary>
        /// Defines a [template](/topics/templating/) to define the docked state of the vacuum. This is required if `docked_topic` is set.
        /// </summary>
        public string? DockedTemplate { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive docked state values from the vacuum.
        /// </summary>
        public string? DockedTopic { get; set; }

        /// <summary>
        /// Defines a [template](/topics/templating/) to define potential error messages emitted by the vacuum. This is required if `error_topic` is set.
        /// </summary>
        public string? ErrorTemplate { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive error messages from the vacuum.
        /// </summary>
        public string? ErrorTopic { get; set; }

        /// <summary>
        /// List of possible fan speeds for the vacuum.
        /// </summary>
        public IList<string>? FanSpeedList { get; set; }

        /// <summary>
        /// Defines a [template](/topics/templating/) to define the fan speed of the vacuum. This is required if `fan_speed_topic` is set.
        /// </summary>
        public string? FanSpeedTemplate { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive fan speed values from the vacuum.
        /// </summary>
        public string? FanSpeedTopic { get; set; }

        /// <summary>
        /// The name of the vacuum.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The payload to send to the `command_topic` to begin a spot cleaning cycle.
        /// </summary>
        public string? PayloadCleanSpot { get; set; }

        /// <summary>
        /// The payload to send to the `command_topic` to locate the vacuum (typically plays a song).
        /// </summary>
        public string? PayloadLocate { get; set; }

        /// <summary>
        /// The payload to send to the `command_topic` to tell the vacuum to return to base.
        /// </summary>
        public string? PayloadReturnToBase { get; set; }

        /// <summary>
        /// The payload to send to the `command_topic` to start or pause the vacuum.
        /// </summary>
        public string? PayloadStartPause { get; set; }

        /// <summary>
        /// The payload to send to the `command_topic` to stop the vacuum.
        /// </summary>
        public string? PayloadStop { get; set; }

        /// <summary>
        /// The payload to send to the `command_topic` to turn the vacuum off.
        /// </summary>
        public string? PayloadTurnOff { get; set; }

        /// <summary>
        /// The payload to send to the `command_topic` to begin the cleaning cycle.
        /// </summary>
        public string? PayloadTurnOn { get; set; }

        /// <summary>
        /// The schema to use. Must be `legacy` or omitted to select the legacy schema.
        /// </summary>
        public string? Schema { get; set; } = "legacy";

        /// <summary>
        /// The MQTT topic to publish custom commands to the vacuum.
        /// </summary>
        public string? SendCommandTopic { get; set; }

        /// <summary>
        /// The MQTT topic to publish commands to control the vacuum's fan speed.
        /// </summary>
        public string? SetFanSpeedTopic { get; set; }

        /// <summary>
        /// List of features that the vacuum supports, possible values are:
        /// - turn_on
        /// - turn_off
        /// - pause
        /// - stop
        /// - return_home
        /// - battery
        /// - status
        /// - locate
        /// - clean_spot
        /// - fan_speed
        /// - send_command
        /// </summary>
        public IList<string>? SupportedFeatures { get; set; }

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

        public class MqttVacuumLegacyValidator : MqttSensorDiscoveryBaseValidator<MqttVacuumLegacy>
        {
            public MqttVacuumLegacyValidator()
            {
                TopicAndTemplate(s => s.BatteryLevelTopic, s => s.BatteryLevelTemplate);
                TopicAndTemplate(s => s.ChargingTopic, s => s.ChargingTemplate);
                TopicAndTemplate(s => s.CleaningTopic, s => s.CleaningTemplate);
                TopicAndTemplate(s => s.DockedTopic, s => s.DockedTemplate);
                TopicAndTemplate(s => s.ErrorTopic, s => s.ErrorTemplate);
                TopicAndTemplate(s => s.FanSpeedTopic, s => s.FanSpeedTemplate);

                // TODO: Make enum
                HashSet<string> possibleFeatures = new HashSet<string>(StringComparer.Ordinal)
                {
                    "turn_on", "turn_off", "pause", "stop", "return_home", "battery", "status", "locate", "clean_spot",
                    "fan_speed", "send_command"
                };

                RuleFor(s => s.SupportedFeatures)
                    .NotEmpty()
                    .ForEach(x => x.Must(possibleFeatures.Contains).WithMessage("{PropertyName} must be one of " + string.Join(", ", possibleFeatures)))
                    .When(s => s.SupportedFeatures != null);

                RuleFor(s => s.Schema).Equal("legacy").When(s => s != null);

                RuleFor(s => s.FanSpeedList)
                    .NotEmpty()
                    .When(s => s.FanSpeedList != null);
            }
        }
    }
}