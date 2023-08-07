#nullable enable
using System;
using System.Collections.Generic;
using FluentValidation;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models;

/// <summary>
/// https://www.home-assistant.io/integrations/light.mqtt/#json-schema
///
/// The mqtt light platform with JSON schema lets you control a MQTT-enabled light that can receive JSON messages.
/// 
/// This schema supports on/off, brightness, RGB colors, XY colors, color temperature, transitions and short/long
/// flashing.The messages sent to/from the lights look similar to this, omitting fields when they aren’t needed.
/// The color_mode will not be present in messages sent to the light. It is optional in messages received from the
/// light, but can be used to disambiguate the current mode in the light. In the example below, color_mode is set
/// to rgb and color_temp, color.c, color.w, color.x, color.y, color.h, color.s will all be ignored by Home Assistant.
/// </summary>
/// <remarks>
/// In an ideal scenario, the MQTT device will have a state topic to publish state changes. If these messages are
/// published with the RETAIN flag, the MQTT light will receive an instant state update after subscription and will
/// start with the correct state. Otherwise, the initial state of the light will be off.
/// 
/// When a state topic is not available, the light will work in optimistic mode.In this mode, the light will
/// immediately change state after every command.Otherwise, the light will wait for state confirmation from the
/// device (message from state_topic).
/// 
/// Optimistic mode can be forced, even if state topic is available.Try enabling it if the light is operating
/// incorrectly.
/// </remarks>
[DeviceType(HassDeviceType.Light)]
[PublicAPI]
public class MqttLightJson : MqttSensorDiscoveryBase<MqttLightJson, MqttLightJson.MqttLightJsonValidator>, IHasUniqueId,
    IHasAvailability, IHasQos, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasRetain, IHasEntityCategory,
    IHasObjectId, IHasEncoding, IHasName
{
    public MqttLightJson(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
    {
    }

    /// <summary>
    /// Flag that defines if the light supports brightness.
    /// </summary>
    public bool? Brightness { get; set; }

    /// <summary>
    /// Defines the maximum brightness value (i.e., 100%) of the MQTT device.
    /// </summary>
    public int? BrightnessScale { get; set; }

    /// <summary>
    /// Flag that defines if the light supports color modes.
    /// </summary>
    public bool? ColorMode { get; set; }

    /// <summary>
    /// The MQTT topic to publish commands to change the light’s state.
    /// </summary>
    public string CommandTopic { get; set; }

    /// <summary>
    /// Flag that defines if the light supports effects.
    /// </summary>
    public bool? Effect { get; set; }

    /// <summary>
    /// The list of effects the light supports.
    /// </summary>
    public IList<string>? EffectList { get; set; }

    /// <summary>
    /// The duration, in seconds, of a “long” flash.
    /// </summary>
    public int? FlashTimeLong { get; set; }

    /// <summary>
    /// The duration, in seconds, of a “short” flash.
    /// </summary>
    public int? FlashTimeShort { get; set; }

    /// <summary>
    /// The maximum color temperature in mireds.
    /// </summary>
    public int? MaxMireds { get; set; }

    /// <summary>
    /// The minimum color temperature in mireds.
    /// </summary>
    public int? MinMireds { get; set; }

    /// <summary>
    /// Flag that defines if the light works in optimistic mode.
    /// </summary>
    public bool? Optimistic { get; set; }

    /// <summary>
    /// The schema to use. Must be `json` to select the JSON schema.
    /// </summary>
    public string? Schema { get; set; } = "json";

    /// <summary>
    /// The MQTT topic subscribed to receive state updates.
    /// </summary>
    public string? StateTopic { get; set; }

    /// <summary>
    /// A list of color modes supported by the list. This is required if color_mode is True.
    /// Possible color modes are:
    /// - onoff
    /// - brightness
    /// - color_temp
    /// - hs
    /// - xy
    /// - rgb
    /// - rgbw
    /// - rgbww
    /// - white
    /// </summary>
    public IList<string>? SupportedColorModes { get; set; }

    /// <summary>
    /// Defines the maximum white level (i.e., 100%) of the MQTT device. This is used when setting the light to white mode.
    /// </summary>
    public byte? WhiteScale { get; set; }

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
    public string? Name { get; set; }

    public class MqttLightJsonValidator : MqttSensorDiscoveryBaseValidator<MqttLightJson>
    {
        public MqttLightJsonValidator()
        {
            // TODO: Make enums
            HashSet<string> validColors = new HashSet<string>(StringComparer.Ordinal)
            {
                "onoff", "brightness", "color_temp", "hs", "xy", "rgb", "rgbw", "rgbww"
            };

            RuleFor(s => s.SupportedColorModes)
                .NotEmpty()
                .ForEach(x =>
                    x.Must(validColors.Contains)
                        .WithMessage("{PropertyName} must be one of " + string.Join(", ", validColors)))
                .When(s => s.ColorMode.GetValueOrDefault(false));

            RuleFor(s => s.FlashTimeShort)
                .GreaterThanOrEqualTo(1);

            RuleFor(s => s.FlashTimeLong)
                .GreaterThanOrEqualTo(1);

            RuleFor(s => s.BrightnessScale)
                .GreaterThanOrEqualTo(1);

            RuleFor(s => s.EffectList)
                .NotEmpty()
                .When(s => s.EffectList != null);

            RuleFor(s => s.Schema).Equal("json");

            // TODO: Find defaults
            //MinMax(s => s.MinMireds, s => s.MaxMireds, 1, 100);
        }
    }
}