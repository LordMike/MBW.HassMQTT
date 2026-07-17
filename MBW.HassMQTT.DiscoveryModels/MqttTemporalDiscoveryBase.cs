#nullable enable

using System.Collections.Generic;
using FluentValidation;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels;

/// <summary>
/// Common discovery schema for MQTT date, date/time, and time entities.
/// </summary>
public abstract class MqttTemporalDiscoveryBase<T, TValidator> : MqttSensorDiscoveryBase<T, TValidator>,
    IHasAvailability, IHasDefaultEntityId, IHasEnabledByDefault, IHasEncoding, IHasEntityCategory,
    IHasEntityPicture, IHasJsonAttributes, IHasMessageExpiryInterval, IHasName, IHasQos, IHasRetain, IHasUniqueId
    where T : IHassDiscoveryDocument
    where TValidator : AbstractValidator<T>, new()
{
    protected MqttTemporalDiscoveryBase(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
    {
    }

    /// <summary>
    /// Defines a template used to generate the payload sent to <see cref="CommandTopic"/>.
    /// </summary>
    public string? CommandTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to which the selected temporal value is published.
    /// </summary>
    public string CommandTopic { get; set; }

    /// <summary>
    /// The MQTT topic subscribed to receive state updates.
    /// </summary>
    public string? StateTopic { get; set; }

    /// <summary>
    /// Defines a template used to extract the state value from messages received on <see cref="StateTopic"/>.
    /// </summary>
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
}
