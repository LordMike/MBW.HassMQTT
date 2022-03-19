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
    /// https://www.home-assistant.io/integrations/light.mqtt/#default-schema
    ///
    /// The mqtt light platform with default schema lets you control your MQTT enabled lights.
    /// It supports setting brightness, color temperature, effects, on/off, RGB colors, XY colors and white.
    /// </summary>
    /// <remarks>
    /// In an ideal scenario, the MQTT device will have a state topic to publish state changes. If these messages
    /// are published with a RETAIN flag, the MQTT light will receive an instant state update after subscription
    /// and will start with the correct state. Otherwise, the initial state of the switch will be `unknown`.
    /// A MQTT device can reset the current state to `unknown` using a `None` payload.
    /// 
    /// When a state topic is not available, the light will work in optimistic mode.In this mode, the light will
    /// immediately change state after every command.Otherwise, the light will wait for state confirmation from
    /// the device (message from state_topic). The initial state is set to `False` / `off` in optimistic mode.
    /// 
    /// Optimistic mode can be forced, even if the state_topic is available.Try to enable it, if experiencing incorrect
    /// light operation.
    /// 
    /// Home Assistant internally assumes that a light’s state corresponds to a defined color_mode.The state of MQTT lights
    /// with default schema and support for both color and color temperature will set the color_mode according to the
    /// last received valid color or color temperature.Optionally, a color_mode_state_topic can be configured for explicit
    /// control of the color_mode
    /// </remarks>
    [DeviceType(HassDeviceType.Light)]
    [PublicAPI]
    public class MqttLightDefault : MqttSensorDiscoveryBase<MqttLightDefault, MqttLightDefault.MqttLightDefaultValidator>, IHasUniqueId, IHasAvailability, IHasQos, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasRetain, IHasEntityCategory, IHasObjectId, IHasEncoding
    {
        public MqttLightDefault(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the light’s brightness.
        /// </summary>
        public string? BrightnessCommandTopic { get; set; }

        /// <summary>
        /// Defines a template to compose message which will be sent to `brightness_command_topic`. Available variables: `value`.
        /// </summary>
        public string? BrightnessCommandTemplate { get; set; }

        /// <summary>
        /// Defines the maximum brightness value (i.e., 100%) of the MQTT device.
        /// </summary>
        public int? BrightnessScale { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive brightness state updates.
        /// </summary>
        public string? BrightnessStateTopic { get; set; }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the brightness value.
        /// </summary>
        public string? BrightnessValueTemplate { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive color mode updates. If this is not configured, color_mode will be automatically
        /// set according to the last received valid color or color temperature
        /// </summary>
        public string? ColorModeStateTopic { get; set; }

        /// <summary>
        /// Defines a template to extract the color mode.
        /// </summary>
        public string? ColorModeValueTemplate { get; set; }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/) to compose message which will be sent to `color_temp_command_topic`. Available variables: `value`.
        /// </summary>
        public string? ColorTempCommandTemplate { get; set; }

        /// <summary>
        /// The MQTT topic to publish commands to change the light’s color temperature state. The color temperature command slider has a range of 153 to 500 mireds (micro reciprocal degrees).
        /// </summary>
        public string? ColorTempCommandTopic { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive color temperature state updates.
        /// </summary>
        public string? ColorTempStateTopic { get; set; }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the color temperature value.
        /// </summary>
        public string? ColorTempValueTemplate { get; set; }

        /// <summary>
        /// The MQTT topic to publish commands to change the switch state.
        /// </summary>
        public string CommandTopic { get; set; }

        /// <summary>
        /// The MQTT topic to publish commands to change the light's effect state.
        /// </summary>
        public string? EffectCommandTopic { get; set; }

        /// <summary>
        /// Defines a template to compose message which will be sent to `effect_command_topic`. Available variables: `value`.
        /// </summary>
        public string? EffectCommandTemplate { get; set; }

        /// <summary>
        /// The list of effects the light supports.
        /// </summary>
        public IList<string>? EffectList { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive effect state updates.
        /// </summary>
        public string? EffectStateTopic { get; set; }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the effect value.
        /// </summary>
        public string? EffectValueTemplate { get; set; }

        /// <summary>
        /// The MQTT topic to publish commands to change the light's color state in HS format (Hue Saturation). Brightness is sent separately in the `brightness_command_topic`. Range for Hue: 0° .. 360°, Range of Saturation: 0..100.
        /// </summary>
        public string? HsCommandTopic { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive color state updates in HS format. Brightness is received separately in the `brightness_state_topic`.
        /// </summary>
        public string? HsStateTopic { get; set; }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the HS value.
        /// </summary>
        public string? HsValueTemplate { get; set; }

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
        /// Defines when on the payload_on is sent. Using `last` (the default) will send any style (brightness, color, etc) topics first and then a `payload_on` to the `command_topic`. Using `first` will send the `payload_on` and then any style topics. Using `brightness` will only send brightness commands instead of the `payload_on` to turn the light on.
        /// </summary>
        public string? OnCommandType { get; set; }

        /// <summary>
        /// Flag that defines if switch works in optimistic mode.
        /// </summary>
        public bool? Optimistic { get; set; }

        /// <summary>
        /// The payload that represents disabled state.
        /// </summary>
        public string? PayloadOff { get; set; }

        /// <summary>
        /// The payload that represents enabled state.
        /// </summary>
        public string? PayloadOn { get; set; }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/) to compose message which will be sent to `rgb_command_topic`. Available variables: `red`, `green` and `blue`.
        /// </summary>
        public string? RgbCommandTemplate { get; set; }

        /// <summary>
        /// The MQTT topic to publish commands to change the light's RGB state. Please note that the color value sent by Home Assistant is normalized to full brightness if `brightness_command_topic` is set. Brightness information is in this case sent separately in the `brightness_command_topic`. This will cause a light that expects an absolute color value (including brightness) to flicker.
        /// </summary>
        public string? RgbCommandTopic { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive RGB state updates. The expected payload is the RGB values separated by commas, for example, `255,0,127`. Please note that the color value received by Home Assistant is normalized to full brightness. Brightness information is received separately in the `brightness_state_topic`.
        /// </summary>
        public string? RgbStateTopic { get; set; }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the RGB value.
        /// </summary>
        public string? RgbValueTemplate { get; set; }

        /// <summary>
        /// The schema to use. Must be `default` or omitted to select the default schema.
        /// </summary>
        public string Schema { get; set; } = "default";

        /// <summary>
        /// The MQTT topic subscribed to receive state updates.
        /// </summary>
        public string? StateTopic { get; set; }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the state value. The template should match the payload `on` and `off` values, so if your light uses `power on` to turn on, your `state_value_template` string should return `power on` when the switch is on. For example if the message is just `on`, your `state_value_template` should be `power {{ value }}`.
        /// </summary>
        public string? StateValueTemplate { get; set; }

        /// <summary>
        /// The MQTT topic to publish commands to change the light's white value.
        /// </summary>
        public string? WhiteCommandTopic { get; set; }

        /// <summary>
        /// Defines the maximum white value (i.e., 100%) of the MQTT device.
        /// </summary>
        public int? WhiteScale { get; set; }

        /// <summary>
        /// The MQTT topic to publish commands to change the light's XY state.
        /// </summary>
        public string? XyCommandTopic { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive XY state updates.
        /// </summary>
        public string? XyStateTopic { get; set; }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the XY value.
        /// </summary>
        public string? XyValueTemplate { get; set; }

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

        public class MqttLightDefaultValidator : MqttSensorDiscoveryBaseValidator<MqttLightDefault>
        {
            public MqttLightDefaultValidator()
            {
                TopicAndTemplate(s => s.BrightnessStateTopic, s => s.BrightnessValueTemplate);
                TopicAndTemplate(s => s.ColorModeStateTopic, s => s.ColorModeValueTemplate);
                TopicAndTemplate(s => s.ColorTempCommandTopic, s => s.ColorTempCommandTemplate);
                TopicAndTemplate(s => s.ColorTempStateTopic, s => s.ColorTempValueTemplate);
                TopicAndTemplate(s => s.EffectStateTopic, s => s.EffectValueTemplate);
                TopicAndTemplate(s => s.HsStateTopic, s => s.HsValueTemplate);
                TopicAndTemplate(s => s.RgbCommandTopic, s => s.RgbCommandTemplate);
                TopicAndTemplate(s => s.RgbStateTopic, s => s.RgbValueTemplate);
                TopicAndTemplate(s => s.StateTopic, s => s.StateValueTemplate);
                TopicAndTemplate(s => s.XyStateTopic, s => s.XyValueTemplate);
                TopicAndTemplate(s => s.EffectCommandTopic, s => s.EffectCommandTemplate);
                TopicAndTemplate(s => s.BrightnessCommandTopic, s => s.BrightnessCommandTemplate);

                RuleFor(s => s.Schema).Equal("default");
            }
        }
    }
}