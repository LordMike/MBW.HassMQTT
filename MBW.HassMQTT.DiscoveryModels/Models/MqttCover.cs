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
/// https://www.home-assistant.io/integrations/cover.mqtt/
///
/// The mqtt cover platform allows you to control an MQTT cover (such as blinds, a roller shutter or a garage door).
/// </summary>
/// <remarks>
/// A cover entity can be in states (open, opening, closed or closing).
/// 
/// If a state_topic is configured, the entity’s state will be updated only after an MQTT message is received
/// on state_topic matching state_open, state_opening, state_closed or state_closing.For covers that only report
/// 3 states(opening, closing, stopped), a state_stopped state can be configured to indicate that the device is
/// not moving.When this payload is received on the state_topic, and a position_topic is not configured, the cover
/// will be set to state closed if its state was closing and to state open otherwise.If a position_topic is set,
/// the cover’s position will be used to set the state to either open or closed state.
/// 
/// If the cover reports its position, a position_topic can be configured for receiving the position.If no state_topic
/// is configured, the cover’s state will be set to either open or closed when a position is received.
/// 
/// If the cover reports its tilt position, a tilt_status_topic can be configured for receiving the tilt position.
/// If position topic and state topic are both defined, the device state (open, opening, closed or closing) will be
/// set by the state topic and the cover position will be set by the position topic.
/// 
/// If neither a state topic nor a position topic are defined, the cover will work in optimistic mode. In this mode,
/// the cover will immediately change state (open or closed) after every command sent by Home Assistant.If a state
/// topic/position topic is defined, the cover will wait for a message on state_topic or position_topic.
/// 
/// Optimistic mode can be forced, even if a state_topic / position_topic is defined.Try to enable it if experiencing
/// incorrect cover operation (Google Assistant gauge may need optimistic mode as it often send request to your Home
/// Assistant immediately after send set_cover_position in which case MQTT could be too slow).
/// 
/// The mqtt cover platform optionally supports a list of availability topics to receive online and offline
/// messages (birth and LWT messages) from the MQTT cover device.During normal operation, if the MQTT cover device
/// goes offline(i.e., publishes a matching payload_not_available to any availability topic), Home Assistant will
/// display the cover as “unavailable”. If these messages are published with the retain flag set, the cover will
/// receive an instant update after subscription and Home Assistant will display correct availability state of the
/// cover when Home Assistant starts up.If the retain flag is not set, Home Assistant will display the cover as
/// "unavailable" when Home Assistant starts up.
/// </remarks>
[DeviceType(HassDeviceType.Cover)]
[PublicAPI]
public class MqttCover : MqttSensorDiscoveryBase<MqttCover, MqttCover.MqttCoverValidator>, IHasUniqueId, IHasAvailability, IHasQos, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasRetain, IHasEntityCategory, IHasObjectId, IHasEncoding
{
    public MqttCover(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
    {
    }

    /// <summary>
    /// The MQTT topic to publish commands to control the cover.
    /// </summary>
    public string? CommandTopic { get; set; }

    /// <summary>
    /// Sets the [class of the device](/integrations/cover/), changing the device state and icon that is displayed on the frontend.
    /// </summary>
    public string? DeviceClass { get; set; }

    /// <summary>
    /// The name of the cover.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Flag that defines if switch works in optimistic mode.
    /// </summary>
    public bool? Optimistic { get; set; }

    /// <summary>
    /// The command payload that closes the cover.
    /// </summary>
    public string? PayloadClose { get; set; }

    /// <summary>
    /// The command payload that opens the cover.
    /// </summary>
    public string? PayloadOpen { get; set; }

    /// <summary>
    /// The command payload that stops the cover.
    /// </summary>
    public string? PayloadStop { get; set; }

    /// <summary>
    /// Number which represents closed position.
    /// </summary>
    public int? PositionClosed { get; set; }

    /// <summary>
    /// Number which represents open position.
    /// </summary>
    public int? PositionOpen { get; set; }

    /// <summary>
    /// Defines a template that can be used to extract the payload for the `position_topic` topic.
    /// </summary>
    public string? PositionTemplate { get; set; }

    /// <summary>
    /// The MQTT topic subscribed to receive cover position messages.
    /// </summary>
    public string? PositionTopic { get; set; }

    /// <summary>
    /// Defines a [template](/topics/templating/) to define the position to be sent to the `set_position_topic` topic. Incoming position value is available for use in the template `{{position}}`. If no template is defined, the position (0-100) will be calculated according to `position_open` and `position_closed` values.
    /// </summary>
    public string? SetPositionTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to publish position commands to. You need to set position_topic as well if you want to use position topic. Use template if position topic wants different values than within range `position_closed` - `position_open`. If template is not defined and `position_closed != 100` and `position_open != 0` then proper position value is calculated from percentage position.
    /// </summary>
    public string? SetpositionTopic { get; set; }

    /// <summary>
    /// The payload that represents the closed state.
    /// </summary>
    public string? StateClosed { get; set; }

    /// <summary>
    /// The payload that represents the closing state.
    /// </summary>
    public string? StateClosing { get; set; }

    /// <summary>
    /// The payload that represents the open state.
    /// </summary>
    public string? StateOpen { get; set; }

    /// <summary>
    /// The payload that represents the opening state.
    /// </summary>
    public string? StateOpening { get; set; }

    /// <summary>
    /// The payload that represents the stopped state (for covers that do not report `open`/`closed` state).
    /// </summary>
    public string? StateStopped { get; set; }

    /// <summary>
    /// The MQTT topic subscribed to receive cover state messages. State topic can only read (`open`, `opening`, `closed`, `closing` or `stopped`) state.
    /// </summary>
    public string? StateTopic { get; set; }

    /// <summary>
    /// The value that will be sent on a `close_cover_tilt` command.
    /// </summary>
    public int? TiltClosedValue { get; set; }

    /// <summary>
    /// Defines a [template](/topics/templating/) that can be used to extract the payload for the `tilt_command_topic` topic.
    /// </summary>
    public string? TiltCommandTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to publish commands to control the cover tilt.
    /// </summary>
    public string? TiltCommandTopic { get; set; }

    /// <summary>
    /// The maximum tilt value
    /// </summary>
    public int? TiltMax { get; set; }

    /// <summary>
    /// The minimum tilt value.
    /// </summary>
    public int? TiltMin { get; set; }

    /// <summary>
    /// The value that will be sent on an `open_cover_tilt` command.
    /// </summary>
    public int? TiltOpenedValue { get; set; }

    /// <summary>
    /// Flag that determines if tilt works in optimistic mode.
    /// </summary>
    public bool? TiltOptimistic { get; set; }

    /// <summary>
    /// Defines a [template](/topics/templating/) that can be used to extract the payload for the `tilt_status_topic` topic. 
    /// </summary>
    public string? TiltStatusTemplate { get; set; }

    /// <summary>
    /// The MQTT topic subscribed to receive tilt status update values.
    /// </summary>
    public string? TiltStatusTopic { get; set; }

    /// <summary>
    /// Defines a template that can be used to extract the payload for the `state_topic` topic.
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

    public class MqttCoverValidator : MqttSensorDiscoveryBaseValidator<MqttCover>
    {
        public MqttCoverValidator()
        {
            TopicAndTemplate(s => s.PositionTopic, s => s.PositionTemplate);
            TopicAndTemplate(s => s.SetpositionTopic, s => s.SetPositionTemplate);
            TopicAndTemplate(s => s.StateTopic, s => s.ValueTemplate);
            TopicAndTemplate(s => s.TiltCommandTopic, s => s.TiltCommandTemplate);
            TopicAndTemplate(s => s.TiltStatusTopic, s => s.TiltStatusTemplate);

            RuleFor(s => s.SetpositionTopic).NotNull().Unless(s => s.PositionTopic == null);
            RuleFor(s => s.PositionTopic).NotNull().Unless(s => s.SetpositionTopic == null);

            MinMax(s => s.TiltMin, s => s.TiltMax, 0, 100,
                (s => s.TiltOpenedValue, 0),
                (s => s.TiltClosedValue, 100));
        }
    }
}