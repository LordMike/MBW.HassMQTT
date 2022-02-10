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
    /// https://www.home-assistant.io/integrations/humidifier.mqtt/
    ///
    /// The mqtt humidifier platform lets you control your MQTT enabled humidifiers.
    /// </summary>
    /// <remarks>
    /// In an ideal scenario, the MQTT device will have a state_topic to publish state changes.
    /// If these messages are published with a RETAIN flag, the MQTT humidifier will receive an instant
    /// state update after subscription and will start with the correct state. Otherwise, the initial
    /// state of the humidifier will be false / off.
    /// 
    /// When a state_topic is not available, the humidifier will work in optimistic mode.In this mode,
    /// the humidifier will immediately change state after every command.Otherwise, the humidifier will
    /// wait for state confirmation from the device (message from state_topic).
    /// 
    /// Optimistic mode can be forced even if a state_topic is available.Try to enable it if you are
    /// experiencing incorrect humidifier operation.
    /// </remarks>
    [DeviceType(HassDeviceType.Humidifier)]
    [PublicAPI]
    public class MqttHumidifier : MqttSensorDiscoveryBase<MqttHumidifier, MqttHumidifier.MqttHumidifierValidator>, IHasUniqueId, IHasAvailability, IHasQos, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasRetain, IHasEntityCategory, IHasObjectId, IHasEncoding
    {
        public MqttHumidifier(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// Defines a template to generate the payload to send to command_topic.
        /// </summary>
        public string? CommandTemplate { get; set; }

        /// <summary>
        /// The MQTT topic to publish commands to change the humidifier state.
        /// </summary>
        public string CommandTopic { get; set; }

        /// <summary>
        /// The device class of the MQTT device. Must be either humidifier or dehumidifier.
        /// </summary>
        public HumidifierDeviceClass? DeviceClass { get; set; }

        /// <summary>
        /// The minimum target humidity percentage that can be set.
        /// </summary>
        public int? MaxHumidity { get; set; }

        /// <summary>
        /// The maximum target humidity percentage that can be set.
        /// </summary>
        public int? MinHumidity { get; set; }

        /// <summary>
        /// The name of the humidifier.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Flag that defines if humidifier works in optimistic mode
        /// </summary>
        public bool? Optimistic { get; set; }

        /// <summary>
        /// The payload that represents the stop state.
        /// </summary>
        public string? PayloadOff { get; set; }

        /// <summary>
        /// The payload that represents the running state.
        /// </summary>
        public string? PayloadOn { get; set; }

        /// <summary>
        /// A special payload that resets the target_humidity state attribute to None when received at the target_humidity_state_topic.
        /// </summary>
        public string? PayloadResetHumidity { get; set; }

        /// <summary>
        /// A special payload that resets the mode state attribute to None when received at the mode_state_topic.
        /// </summary>
        public string? PayloadResetMode { get; set; }

        /// <summary>
        /// Defines a template to generate the payload to send to target_humidity_command_topic.
        /// </summary>
        public string? TargetHumidityCommandTemplate { get; set; }

        /// <summary>
        /// The MQTT topic to publish commands to change the humidifier target humidity state based on a percentage.
        /// </summary>
        public string TargetHumidityCommandTopic { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive humidifier target humidity.
        /// </summary>
        public string? TargetHumidityStateTopic { get; set; }

        /// <summary>
        /// Defines a template to extract a value for the humidifier target_humidity state.
        /// </summary>
        public string? TargetHumidityStateTemplate { get; set; }

        /// <summary>
        /// Defines a template to generate the payload to send to mode_command_topic.
        /// </summary>
        public string? ModeCommandTemplate { get; set; }

        /// <summary>
        /// The MQTT topic to publish commands to change the mode on the humidifier. This attribute ust be configured together
        /// with the modes attribute.
        /// </summary>
        public string? ModeCommandTopic { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive the humidifier mode.
        /// </summary>
        public string? ModeStateTopic { get; set; }

        /// <summary>
        /// Defines a template to extract a value for the humidifier mode state.
        /// </summary>
        public string? ModeStateTemplate { get; set; }

        /// <summary>
        /// List of available modes this humidifier is capable of running at. Common examples include normal, eco, away,
        /// boost, comfort, home, sleep, auto and baby. These examples offer built-in translations but other custom modes
        /// are allowed as well. This attribute ust be configured together with the mode_command_topic attribute.
        /// </summary>
        public IList<string>? Modes { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive state updates.
        /// </summary>
        public string? StateTopic { get; set; }

        /// <summary>
        /// Defines a template to extract a value from the state.
        /// </summary>
        public string? StateValueTemplate { get; set; }

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

        public class MqttHumidifierValidator : MqttSensorDiscoveryBaseValidator<MqttHumidifier>
        {
            public MqttHumidifierValidator()
            {
                TopicAndTemplate(s => s.CommandTopic, s => s.CommandTemplate);
                TopicAndTemplate(s => s.TargetHumidityCommandTopic, s => s.TargetHumidityCommandTemplate);
                TopicAndTemplate(s => s.TargetHumidityStateTopic, s => s.TargetHumidityStateTemplate);
                TopicAndTemplate(s => s.ModeCommandTopic, s => s.ModeCommandTemplate);
                TopicAndTemplate(s => s.ModeStateTopic, s => s.ModeStateTemplate);
                TopicAndTemplate(s => s.StateTopic, s => s.StateValueTemplate);

                RuleFor(s => s.DeviceClass)
                    .IsInEnum()
                    .Must(s => s!.Value != HumidifierDeviceClass.Unknown)
                    .When(s => s.DeviceClass.HasValue);
            }
        }
    }
}