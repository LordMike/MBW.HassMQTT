#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models;

/// <summary>
/// https://www.home-assistant.io/integrations/select.mqtt/
///
/// The mqtt Select platform allows you to integrate devices that might expose configuration options through MQTT
/// into Home Assistant as a Select. Every time a message under the topic in the configuration is received,
/// the select entity will be updated in Home Assistant and vice-versa, keeping the device and Home Assistant in sync.
/// </summary>
[DeviceType(HassDeviceType.Select)]
[PublicAPI]
public class MqttSelect : MqttSensorDiscoveryBase<MqttSelect, MqttSelect.MqttSelectValidator>, IHasUniqueId,
    IHasAvailability, IHasQos, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasRetain, IHasEntityCategory,
    IHasDefaultEntityId, IHasEncoding, IHasName, IHasOptimistic, IHasEntityPicture, IHasVisibleByDefault, IHasMessageExpiryInterval
{
    public MqttSelect(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
    {
    }

    protected override void OnPropertyChanged(string propertyName, object? before, object? after)
    {
        if (propertyName == nameof(Options))
        {
            // Extra check as arrays are special
            if (before is List<string> valBefore && after is List<string> valAfter &&
                valBefore.Count == valAfter.Count &&
                valBefore.SequenceEqual(valAfter, StringComparer.Ordinal))
            {
                // These arrays are identical
                return;
            }
        }

        base.OnPropertyChanged(propertyName, before, after);
    }

    /// <summary>
    /// Defines a `template` to generate the payload to send to `command_topic`.
    /// </summary>
    public string? CommandTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to publish commands to change the selected option.
    /// </summary>
    public string CommandTopic { get; set; }

    /// <summary>
    /// List of options that can be selected. An empty list or a list with a single item is allowed.
    /// </summary>
    public IList<string> Options { get; set; } = new List<string>();

    /// <summary>
    /// The MQTT topic subscribed to receive update of the selected option.
    /// </summary>
    public string? StateTopic { get; set; }

    /// <summary>
    /// Defines a template to extract the value.
    /// </summary>
    public string? ValueTemplate { get; set; }

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
    public string? EntityPicture { get; set; }
    /// <inheritdoc />
    public bool? VisibleByDefault { get; set; }
    /// <inheritdoc />
    public MessageExpiryInterval? MessageExpiryInterval { get; set; }
    /// <inheritdoc />
    public string? Encoding { get; set; }
    /// <inheritdoc />
    public Optional<string?> Name { get; set; }
    /// <inheritdoc />
    public bool? Optimistic { get; set; }

    public class MqttSelectValidator : MqttSensorDiscoveryBaseValidator<MqttSelect>
    {
        public MqttSelectValidator()
        {
            TopicAndTemplate(s => s.StateTopic, s => s.ValueTemplate);
            TopicAndTemplate(s => s.CommandTopic, s => s.CommandTemplate);
        }
    }
}
