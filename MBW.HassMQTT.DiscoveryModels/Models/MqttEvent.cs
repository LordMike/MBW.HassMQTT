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
/// https://www.home-assistant.io/integrations/event.mqtt/
///
/// The `mqtt` event platform allows you to process event info from an MQTT message. Events are signals that are emitted when something happens, for example, when a user presses a physical button like a doorbell or when a button on a remote control is pressed. With the event some event attributes can be sent te become available as an attribute on the entity. MQTT events are stateless. For example, a doorbell does not have a state like being "on" or "off" but instead is momentarily pressed.
/// </summary>
[PublicAPI]
[DeviceType(HassDeviceType.Event)]
public class MqttEvent : MqttSensorDiscoveryBase<MqttEvent, MqttEvent.MqttEventValidator>, IHasAvailability,
    IHasEnabledByDefault, IHasEncoding, IHasEntityCategory, IHasIcon, IHasJsonAttributes, IHasName, IHasObjectId,
    IHasQos, IHasUniqueId
{
    public MqttEvent(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
    {
    }

    /// <summary>
    /// Defines a template to generate the payload to send to `command_topic`.
    /// </summary>
    public string? CommandTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to publish commands to trigger the event.
    /// </summary>
    public string? CommandTopic { get; set; }

    /// <summary>
    /// The type/class of the event to set the icon in the frontend. The `device_class` can be `null`.
    /// </summary>
    public HassEventDeviceClass? DeviceClass { get; set; }

    /// <summary>
    /// A list of valid `event_type` strings.
    /// </summary>
    public List<string>? EventTypes { get; set; }

    /// <summary>
    /// The MQTT topic subscribed to receive JSON event payloads. The JSON payload should contain the `event_type` element. The event type should be one of the configured `event_types`.
    /// </summary>
    public string? StateTopic { get; set; }

    /// <summary>
    /// Defines a template to extract the value and render it to a valid JSON event payload. If the template throws an error, the current state will be used instead.
    /// </summary>
    public string? ValueTemplate { get; set; }

    public IList<AvailabilityModel>? Availability { get; set; }
    public AvailabilityMode? AvailabilityMode { get; set; }
    public bool? EnabledByDefault { get; set; }
    public string? Encoding { get; set; }
    public EntityCategory? EntityCategory { get; set; }
    public string? Icon { get; set; }
    public string? JsonAttributesTemplate { get; set; }
    public string? JsonAttributesTopic { get; set; }
    public string? Name { get; set; }
    public string? ObjectId { get; set; }
    public MqttQosLevel? Qos { get; set; }
    public string? UniqueId { get; set; }

    public class MqttEventValidator : MqttSensorDiscoveryBaseValidator<MqttEvent>
    {
        public MqttEventValidator()
        {
            TopicAndTemplate(s => s.StateTopic, s => s.ValueTemplate);
            TopicAndTemplate(s => s.CommandTopic, s => s.CommandTemplate);

            RuleFor(s => s.StateTopic)
                .NotEmpty();

            RuleFor(s => s.EventTypes)
                .NotNull()
                .NotEmpty();
        }
    }
}