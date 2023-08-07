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
    IHasAvailability, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasObjectId, IHasQos, IHasEntityCategory,
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

    public string? UniqueId { get; set; }
    public IList<AvailabilityModel>? Availability { get; set; }
    public AvailabilityMode? AvailabilityMode { get; set; }
    public string? JsonAttributesTemplate { get; set; }
    public string? JsonAttributesTopic { get; set; }
    public string? Icon { get; set; }
    public bool? EnabledByDefault { get; set; }
    public EntityCategory? EntityCategory { get; set; }
    public string? ObjectId { get; set; }
    public MqttQosLevel? Qos { get; set; }
    public string? Encoding { get; set; }
    public bool? Retain { get; set; }
    public string? Name { get; set; }

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