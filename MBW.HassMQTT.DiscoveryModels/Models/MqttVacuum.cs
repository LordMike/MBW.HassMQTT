#nullable enable
using System.Collections.Generic;
using FluentValidation;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models;

/// <summary>
/// https://www.home-assistant.io/integrations/vacuum.mqtt/#state-configuration
/// 
/// The mqtt vacuum integration allows you to control your MQTT-enabled vacuum. There are two possible message
/// schemas - legacy and state, chosen by setting the schema configuration parameter. New installations should
/// use the state schema as legacy is deprecated and might be removed someday in the future. The state schema
/// is preferred because the vacuum will then be represented as a StateVacuumDevice which is the preferred parent
/// vacuum entity.
///
/// The initial state of the state vacuum entity will set to `unknown` and can be reset by a device by sending
/// a `null` payload as state. The legacy `mqtt` vacuum does not support handling an `unknown` state.
/// </summary>
[DeviceType(HassDeviceType.Vacuum)]
[PublicAPI]
public class MqttVacuum : MqttSensorDiscoveryBase<MqttVacuum, MqttVacuum.MqttVacuumValidator>, IHasUniqueId,
    IHasAvailability, IHasAvailabilityPayloads, IHasQos, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasRetain, IHasEntityCategory,
    IHasDefaultEntityId, IHasName, IHasVisibleByDefault, IHasMessageExpiryInterval, IHasEncoding
{
    public MqttVacuum(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
    {
    }

    /// <summary>
    /// Defines a template used to generate the payload sent to <see cref="CleanSegmentsCommandTopic"/>.
    /// </summary>
    public string? CleanSegmentsCommandTemplate { get; set; }

    /// <summary>The MQTT topic used to publish a JSON list of segment IDs to clean.</summary>
    public string? CleanSegmentsCommandTopic { get; set; }

    /// <summary>
    /// The MQTT topic to publish commands to control the vacuum.
    /// </summary>
    public string? CommandTopic { get; set; }

    /// <summary>
    /// List of possible fan speeds for the vacuum.
    /// </summary>
    public IList<string>? FanSpeedList { get; set; }

    /// <summary>
    /// The payload to send to the `command_topic` to begin a spot cleaning cycle.
    /// </summary>
    public string? PayloadCleanSpot { get; set; }

    /// <summary>
    /// The payload to send to the `command_topic` to locate the vacuum (typically plays a song).
    /// </summary>
    public string? PayloadLocate { get; set; }

    /// <summary>
    /// The payload to send to the `command_topic` to pause the vacuum.
    /// </summary>
    public string? PayloadPause { get; set; }

    /// <summary>
    /// The payload to send to the `command_topic` to tell the vacuum to return to base.
    /// </summary>
    public string? PayloadReturnToBase { get; set; }

    /// <summary>
    /// The payload to send to the `command_topic` to begin the cleaning cycle.
    /// </summary>
    public string? PayloadStart { get; set; }

    /// <summary>
    /// The payload to send to the `command_topic` to stop cleaning.
    /// </summary>
    public string? PayloadStop { get; set; }

    /// <summary>
    /// The MQTT topic to publish custom commands to the vacuum.
    /// </summary>
    public string? SendCommandTopic { get; set; }

    /// <summary>
    /// The MQTT topic to publish commands to control the vacuum's fan speed.
    /// </summary>
    public string? SetFanSpeedTopic { get; set; }

    /// <summary>
    /// The MQTT topic subscribed to receive state messages from the vacuum. Messages received on the `state_topic` must be a valid JSON dictionary, with a mandatory `state` key and optionally `battery_level` and `fan_speed` keys as shown in the [example](#state-mqtt-protocol).
    /// </summary>
    public string? StateTopic { get; set; }

    /// <summary>
    /// List of features that the vacuum supports, possible values are:
    /// - start
    /// - stop
    /// - pause
    /// - return_home
    /// - battery
    /// - status
    /// - locate
    /// - clean_spot
    /// - fan_speed
    /// - send_command
    /// </summary>
    public IList<HassVacuumFeature>? SupportedFeatures { get; set; }

    /// <inheritdoc />
    public string? UniqueId { get; set; }
    /// <inheritdoc />
    public IList<AvailabilityModel>? Availability { get; set; }
    /// <inheritdoc />
    public AvailabilityMode? AvailabilityMode { get; set; }
    /// <inheritdoc />
    public string? AvailabilityTemplate { get; set; }
    /// <inheritdoc />
    public string? AvailabilityTopic { get; set; }
    /// <inheritdoc />
    public string? PayloadAvailable { get; set; }
    /// <inheritdoc />
    public string? PayloadNotAvailable { get; set; }
    /// <inheritdoc />
    public MqttQosLevel? Qos { get; set; }
    /// <inheritdoc />
    public string? JsonAttributesTemplate { get; set; }
    /// <inheritdoc />
    public string? JsonAttributesTopic { get; set; }
    /// <inheritdoc />
    public string? Icon { get; set; }
    /// <inheritdoc />
    public bool? EnabledByDefault { get; set; }
    /// <inheritdoc />
    public bool? Retain { get; set; }
    /// <inheritdoc />
    public EntityCategory? EntityCategory { get; set; }
    /// <inheritdoc />
    public string? DefaultEntityId { get; set; }
    /// <inheritdoc />
    public bool? VisibleByDefault { get; set; }
    /// <inheritdoc />
    public MessageExpiryInterval? MessageExpiryInterval { get; set; }
    /// <inheritdoc />
    public Optional<string?> Name { get; set; }
    /// <inheritdoc />
    public string? Encoding { get; set; }

    public class MqttVacuumValidator : MqttSensorDiscoveryBaseValidator<MqttVacuum>
    {
        public MqttVacuumValidator()
        {
            TopicAndTemplate(s => s.CleanSegmentsCommandTopic, s => s.CleanSegmentsCommandTemplate);

            RuleFor(s => s.FanSpeedList)
                .NotEmpty()
                .When(s => s.FanSpeedList != null);
        }
    }
}
