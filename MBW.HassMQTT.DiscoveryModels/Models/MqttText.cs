#nullable enable
using System.Collections.Generic;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models;

/// <summary>
/// https://www.home-assistant.io/integrations/text.mqtt/
/// 
/// The `mqtt` Text platform allows you to integrate devices that show text that can be set remotely. Optionally the text state can be monitored too using MQTT.
/// </summary>
[DeviceType(HassDeviceType.Text)]
[PublicAPI]
public class MqttText : MqttSensorDiscoveryBase<MqttText, MqttText.MqttTextValidator>, IHasUniqueId,
    IHasAvailability, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasDefaultEntityId, IHasQos, IHasEntityCategory, IHasEntityPicture, IHasVisibleByDefault, IHasMessageExpiryInterval,
    IHasEncoding, IHasRetain, IHasName
{
    public MqttText(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
    {
    }

    /// <summary>
    /// Defines a template to generate the payload to send to `command_topic`.
    /// </summary>
    public string? CommandTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to publish the text value that is set.
    /// </summary>
    public string CommandTopic { get; set; }

    /// <summary>
    /// The maximum size of a text being set or received (maximum is 255).
    /// </summary>
    /// <remarks>The default is '255'</remarks>
    public byte? Max { get; set; }

    /// <summary>
    /// The minimum size of a text being set or received.
    /// </summary>
    /// <remarks>The default is '0'</remarks>
    public byte? Min { get; set; }

    /// <summary>
    /// The mode off the text entity. Must be either `text` or `password`.
    /// </summary>
    /// <remarks>The default is 'text'</remarks>
    public HassTextMode? Mode { get; set; }

    /// <summary>
    /// A valid regular expression the text being set or received must match with.
    /// </summary>
    public string? Pattern { get; set; }

    /// <summary>
    /// The MQTT topic subscribed to receive text state updates. Text state updates should match the `pattern` (if set) and meet the size constraints `min` and `max`.
    /// Can be used with `value_template` to render the incoming payload to a text update.
    /// </summary>
    public string? StateTopic { get; set; }

    /// <summary>
    /// Defines a template to extract the text state value from the payload received on `state_topic`.
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
    public string? JsonAttributesTemplate { get; set; }
    /// <inheritdoc />
    public string? JsonAttributesTopic { get; set; }
    /// <inheritdoc />
    public string? Icon { get; set; }
    /// <inheritdoc />
    public bool? EnabledByDefault { get; set; }
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
    public MqttQosLevel? Qos { get; set; }
    /// <inheritdoc />
    public string? Encoding { get; set; }
    /// <inheritdoc />
    public bool? Retain { get; set; }
    /// <inheritdoc />
    public Optional<string?> Name { get; set; }

    public class MqttTextValidator : MqttSensorDiscoveryBaseValidator<MqttText>
    {
        public MqttTextValidator()
        {
            TopicAndTemplate(x => x.CommandTopic, x => x.CommandTemplate);
            TopicAndTemplate(x => x.StateTopic, x => x.ValueTemplate);
            MinMax(s => s.Min, s => s.Max, 30, 99);
        }
    }
}
