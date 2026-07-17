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

    /// <summary>A template used to render the value published to <see cref="CommandTopic" />.</summary>
    public string? CommandTemplate { get; set; }
    /// <summary>The MQTT topic to which valve commands are published.</summary>
    public string? CommandTopic { get; set; }
    /// <summary>The Home Assistant valve device class, which changes the displayed state and icon.</summary>
    public HassValveDeviceClass? DeviceClass { get; set; }
    /// <summary>The command payload that closes the valve when <see cref="ReportsPosition" /> is not <see langword="true" />. The documented default is <c>CLOSE</c>.</summary>
    public string? PayloadClose { get; set; }
    /// <summary>The command payload that opens the valve when <see cref="ReportsPosition" /> is not <see langword="true" />. The documented default is <c>OPEN</c>.</summary>
    public string? PayloadOpen { get; set; }
    /// <summary>The command payload that stops the valve.</summary>
    public string? PayloadStop { get; set; }
    /// <summary>The device value representing the closed position. The documented default is <c>0</c>.</summary>
    public int? PositionClosed { get; set; }
    /// <summary>The device value representing the open position. The documented default is <c>100</c>.</summary>
    public int? PositionOpen { get; set; }
    /// <summary>Whether the valve reports or accepts numeric positions. When enabled, Home Assistant publishes positions instead of open, close, or stop payloads.</summary>
    public bool? ReportsPosition { get; set; }
    /// <summary>The payload representing a closed valve when <see cref="ReportsPosition" /> is not <see langword="true" />. The documented default is <c>closed</c>.</summary>
    public string? StateClosed { get; set; }
    /// <summary>The payload representing a closing valve. The documented default is <c>closing</c>.</summary>
    public string? StateClosing { get; set; }
    /// <summary>The payload representing an open valve when <see cref="ReportsPosition" /> is not <see langword="true" />. The documented default is <c>open</c>.</summary>
    public string? StateOpen { get; set; }
    /// <summary>The payload representing an opening valve. The documented default is <c>opening</c>.</summary>
    public string? StateOpening { get; set; }
    /// <summary>The MQTT topic subscribed to receive valve state or position updates.</summary>
    public string? StateTopic { get; set; }
    /// <summary>A template used to extract the valve state or position from messages received on <see cref="StateTopic" />.</summary>
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
