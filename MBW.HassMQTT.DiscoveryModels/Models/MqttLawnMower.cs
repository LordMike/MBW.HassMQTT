#nullable enable

using System.Collections.Generic;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models;

/// <summary>
/// Defines an MQTT lawn mower entity.
/// See https://www.home-assistant.io/integrations/lawn_mower.mqtt
/// </summary>
[DeviceType(HassDeviceType.LawnMower)]
[PublicAPI]
public class MqttLawnMower : MqttSensorDiscoveryBase<MqttLawnMower, MqttLawnMower.MqttLawnMowerValidator>,
    IHasAvailability, IHasDefaultEntityId, IHasEnabledByDefault, IHasEncoding, IHasEntityCategory,
    IHasEntityPicture, IHasIcon, IHasJsonAttributes, IHasMessageExpiryInterval, IHasName, IHasQos,
    IHasOptimistic, IHasRetain, IHasUniqueId, IHasVisibleByDefault
{
    public MqttLawnMower(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
    {
    }

    /// <summary>The MQTT topic subscribed to receive activity-state updates.</summary>
    public string? ActivityStateTopic { get; set; }
    /// <summary>A template used to extract the activity state from messages received on <see cref="ActivityStateTopic" />.</summary>
    public string? ActivityValueTemplate { get; set; }
    /// <summary>A template used to render the dock command published to <see cref="DockCommandTopic" />.</summary>
    public string? DockCommandTemplate { get; set; }
    /// <summary>The MQTT topic to which dock commands are published.</summary>
    public string? DockCommandTopic { get; set; }
    /// <inheritdoc />
    public bool? Optimistic { get; set; }
    /// <summary>A template used to render the pause command published to <see cref="PauseCommandTopic" />.</summary>
    public string? PauseCommandTemplate { get; set; }
    /// <summary>The MQTT topic to which pause commands are published.</summary>
    public string? PauseCommandTopic { get; set; }
    /// <summary>A template used to render the start-mowing command published to <see cref="StartMowingCommandTopic" />.</summary>
    public string? StartMowingTemplate { get; set; }
    /// <summary>The MQTT topic to which start-mowing commands are published.</summary>
    public string? StartMowingCommandTopic { get; set; }

    /// <inheritdoc />
    public IList<AvailabilityModel>? Availability { get; set; }
    /// <inheritdoc />
    public AvailabilityMode? AvailabilityMode { get; set; }
    /// <inheritdoc />
    public string? AvailabilityTemplate { get; set; }
    /// <inheritdoc />
    public string? AvailabilityTopic { get; set; }
    /// <inheritdoc />
    public string? DefaultEntityId { get; set; }
    /// <inheritdoc />
    public bool? EnabledByDefault { get; set; }
    /// <inheritdoc />
    public string? Encoding { get; set; }
    /// <inheritdoc />
    public EntityCategory? EntityCategory { get; set; }
    /// <inheritdoc />
    public string? EntityPicture { get; set; }
    /// <inheritdoc />
    public string? Icon { get; set; }
    /// <inheritdoc />
    public string? JsonAttributesTemplate { get; set; }
    /// <inheritdoc />
    public string? JsonAttributesTopic { get; set; }
    /// <inheritdoc />
    public MessageExpiryInterval? MessageExpiryInterval { get; set; }
    /// <inheritdoc />
    public string? Name { get; set; }
    /// <inheritdoc />
    public MqttQosLevel? Qos { get; set; }
    /// <inheritdoc />
    public bool? Retain { get; set; }
    /// <inheritdoc />
    public string? UniqueId { get; set; }
    /// <inheritdoc />
    public bool? VisibleByDefault { get; set; }

    public class MqttLawnMowerValidator : MqttSensorDiscoveryBaseValidator<MqttLawnMower>
    {
        public MqttLawnMowerValidator()
        {
            TopicAndTemplate(x => x.ActivityStateTopic, x => x.ActivityValueTemplate);
            TopicAndTemplate(x => x.DockCommandTopic, x => x.DockCommandTemplate);
            TopicAndTemplate(x => x.PauseCommandTopic, x => x.PauseCommandTemplate);
            TopicAndTemplate(x => x.StartMowingCommandTopic, x => x.StartMowingTemplate);
        }
    }
}
