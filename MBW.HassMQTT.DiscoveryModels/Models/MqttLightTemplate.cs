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
    /// https://www.home-assistant.io/integrations/light.mqtt/#template-schema
    ///
    /// The mqtt light platform with template schema lets you control a MQTT-enabled light that receive
    /// commands on a command topic and optionally sends status update on a state topic. It is format-agnostic
    /// so you can use any data format you want (i.e., string, JSON), just configure it with templating.
    /// 
    /// This schema supports on/off, brightness, RGB colors, XY colors, color temperature, transitions,
    /// short/long flashing and effects.
    /// </summary>
    /// <remarks>
    /// In an ideal scenario, the MQTT device will have a state topic to publish state changes. If these
    /// messages are published with the RETAIN flag, the MQTT light will receive an instant state update
    /// after subscription and will start with the correct state. Otherwise, the initial state of the light
    /// will be off.
    /// 
    /// When a state topic is not available, the light will work in optimistic mode.In this mode, the light
    /// will immediately change state after every command.Otherwise, the light will wait for state confirmation
    /// from the device (message from state_topic).
    /// 
    /// Optimistic mode can be forced, even if state topic is available.Try enabling it if the light is operating
    /// incorrectly.
    /// </remarks>
    [DeviceType(HassDeviceType.Light)]
    [PublicAPI]
    public class MqttLightTemplate : MqttSensorDiscoveryBase<MqttLightTemplate, MqttLightTemplate.MqttLightTemplateValidator>, IHasUniqueId, IHasAvailability, IHasQos, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasRetain, IHasEntityCategory
    {
        public MqttLightTemplate(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// [Template](/docs/configuration/templating/#processing-incoming-data) to extract blue color from the state payload value.
        /// </summary>
        public string? BlueTemplate { get; set; }

        /// <summary>
        /// [Template](/docs/configuration/templating/#processing-incoming-data) to extract brightness from the state payload value.
        /// </summary>
        public string? BrightnessTemplate { get; set; }

        /// <summary>
        /// [Template](/docs/configuration/templating/#processing-incoming-data) to extract color temperature from the state payload value.
        /// </summary>
        public string? ColorTempTemplate { get; set; }

        /// <summary>
        /// The [template](/docs/configuration/templating/#processing-incoming-data) for *off* state changes. Available variables: `state` and `transition`.
        /// </summary>
        public string CommandOffTemplate { get; set; }

        /// <summary>
        /// The [template](/docs/configuration/templating/#processing-incoming-data) for *on* state changes. Available variables: `state`, `brightness`, `red`, `green`, `blue`, `white_value`, `flash`, `transition` and `effect`.
        /// </summary>
        public string CommandOnTemplate { get; set; }

        /// <summary>
        /// The MQTT topic to publish commands to change the light’s state.
        /// </summary>
        public string CommandTopic { get; set; }

        /// <summary>
        /// List of possible effects.
        /// </summary>
        public IList<string>? EffectList { get; set; }

        /// <summary>
        /// [Template](/docs/configuration/templating/#processing-incoming-data) to extract effect from the state payload value.
        /// </summary>
        public string? EffectTemplate { get; set; }

        /// <summary>
        /// [Template](/docs/configuration/templating/#processing-incoming-data) to extract green color from the state payload value.
        /// </summary>
        public string? GreenTemplate { get; set; }

        /// <summary>
        /// The maximum color temperature in mireds.
        /// </summary>
        public int? MaxMireds { get; set; }

        /// <summary>
        /// The minimum color temperature in mireds.
        /// </summary>
        public int? MinMireds { get; set; }

        /// <summary>
        /// The name of the light.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Flag that defines if the light works in optimistic mode.
        /// </summary>
        public bool? Optimistic { get; set; }

        /// <summary>
        /// [Template](/docs/configuration/templating/#processing-incoming-data) to extract red color from the state payload value.
        /// </summary>
        public string? RedTemplate { get; set; }

        /// <summary>
        /// The schema to use. Must be `template` to select the template schema.
        /// </summary>
        public string? Schema { get; set; } = "template";

        /// <summary>
        /// [Template](/docs/configuration/templating/#processing-incoming-data) to extract state from the state payload value.
        /// </summary>
        public string? StateTemplate { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive state updates.
        /// </summary>
        public string? StateTopic { get; set; }

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

        public class MqttLightTemplateValidator : MqttSensorDiscoveryBaseValidator<MqttLightTemplate>
        {
            public MqttLightTemplateValidator()
            {
                TopicAndTemplate(s => s.StateTopic, s => s.BlueTemplate);
                TopicAndTemplate(s => s.StateTopic, s => s.BrightnessTemplate);
                TopicAndTemplate(s => s.StateTopic, s => s.ColorTempTemplate);
                TopicAndTemplate(s => s.StateTopic, s => s.CommandOffTemplate);
                TopicAndTemplate(s => s.StateTopic, s => s.CommandOnTemplate);
                TopicAndTemplate(s => s.StateTopic, s => s.EffectTemplate);
                TopicAndTemplate(s => s.StateTopic, s => s.GreenTemplate);
                TopicAndTemplate(s => s.StateTopic, s => s.RedTemplate);
                TopicAndTemplate(s => s.StateTopic, s => s.StateTemplate);
                TopicAndTemplate(s => s.StateTopic, s => s.BlueTemplate);
                TopicAndTemplate(s => s.StateTopic, s => s.BlueTemplate);

                RuleFor(s => s.Schema).Equal("template");

                // TODO: Find defaults
                //MinMax(s => s.MinMireds, s => s.MaxMireds, 1, 100);
            }
        }
    }
}