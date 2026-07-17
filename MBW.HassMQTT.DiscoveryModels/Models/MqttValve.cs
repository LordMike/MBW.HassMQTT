#nullable enable

using System.Collections.Generic;
using FluentValidation;
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
    public HassValveDeviceClass? DeviceClass { get; set; }
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
    public IList<string>? Group { get; set; }
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
    public bool? Optimistic { get; set; }
    /// <inheritdoc />
    public MqttQosLevel? Qos { get; set; }
    /// <inheritdoc />
    public bool? Retain { get; set; }
    /// <inheritdoc />
    public string? UniqueId { get; set; }
    /// <inheritdoc />
    public bool? VisibleByDefault { get; set; }

    public class MqttValveValidator : MqttSensorDiscoveryBaseValidator<MqttValve>
    {
        public MqttValveValidator()
        {
            TopicAndTemplate(x => x.CommandTopic, x => x.CommandTemplate);
            TopicAndTemplate(x => x.StateTopic, x => x.ValueTemplate);

            When(x => x.ReportsPosition == true, () =>
            {
                RuleFor(x => x.PayloadClose).Null();
                RuleFor(x => x.PayloadOpen).Null();
                RuleFor(x => x.StateClosed).Null();
                RuleFor(x => x.StateOpen).Null();
            });
        }
    }
}
