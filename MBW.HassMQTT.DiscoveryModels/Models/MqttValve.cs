#nullable enable

using System.Collections.Generic;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models;

/// <summary>Defines an MQTT valve entity. See https://www.home-assistant.io/integrations/valve.mqtt</summary>
[DeviceType(HassDeviceType.Valve)]
[PublicAPI]
public class MqttValve : MqttSensorDiscoveryBase<MqttValve, MqttValve.MqttValveValidator>,
    IHasAvailability, IHasAvailabilityPayloads, IHasDefaultEntityId, IHasEnabledByDefault, IHasEncoding,
    IHasEntityCategory, IHasEntityPicture, IHasGroup, IHasIcon, IHasJsonAttributes, IHasMessageExpiryInterval,
    IHasName, IHasOptimistic, IHasQos, IHasRetain, IHasUniqueId, IHasVisibleByDefault
{
    public MqttValve(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId) { }

    public string? CommandTemplate { get; set; }
    public string? CommandTopic { get; set; }
    /// <summary>The valve device class used by Home Assistant.</summary>
    public string? DeviceClass { get; set; }
    public string? PayloadClose { get; set; }
    public string? PayloadOpen { get; set; }
    public string? PayloadStop { get; set; }
    public int? PositionClosed { get; set; }
    public int? PositionOpen { get; set; }
    /// <summary>Whether this valve reports a numeric position.</summary>
    public bool? ReportsPosition { get; set; }
    public string? StateClosed { get; set; }
    public string? StateClosing { get; set; }
    public string? StateOpen { get; set; }
    public string? StateOpening { get; set; }
    public string? StateTopic { get; set; }
    public string? ValueTemplate { get; set; }

    public IList<AvailabilityModel>? Availability { get; set; }
    public AvailabilityMode? AvailabilityMode { get; set; }
    public string? AvailabilityTemplate { get; set; }
    public string? AvailabilityTopic { get; set; }
    public string? PayloadAvailable { get; set; }
    public string? PayloadNotAvailable { get; set; }
    public string? DefaultEntityId { get; set; }
    public bool? EnabledByDefault { get; set; }
    public string? Encoding { get; set; }
    public EntityCategory? EntityCategory { get; set; }
    public string? EntityPicture { get; set; }
    public IList<string>? Group { get; set; }
    public string? Icon { get; set; }
    public string? JsonAttributesTemplate { get; set; }
    public string? JsonAttributesTopic { get; set; }
    public MessageExpiryInterval? MessageExpiryInterval { get; set; }
    public string? Name { get; set; }
    public bool? Optimistic { get; set; }
    public MqttQosLevel? Qos { get; set; }
    public bool? Retain { get; set; }
    public string? UniqueId { get; set; }
    public bool? VisibleByDefault { get; set; }

    public class MqttValveValidator : MqttSensorDiscoveryBaseValidator<MqttValve>
    {
        public MqttValveValidator()
        {
            TopicAndTemplate(x => x.CommandTopic, x => x.CommandTemplate);
            TopicAndTemplate(x => x.StateTopic, x => x.ValueTemplate);
        }
    }
}
