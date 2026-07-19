#nullable enable

using System.Collections.Generic;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models;

/// <summary>
/// https://www.home-assistant.io/integrations/button.mqtt
///
/// The mqtt button platform lets you send an MQTT message when the button is pressed in the frontend
/// or the button press service is called. This can be used to expose some service of a remote device,
/// for example reboot.
/// </summary>
[DeviceType(HassDeviceType.Button)]
[PublicAPI]
public class MqttButton : MqttSensorDiscoveryBase<MqttButton, MqttButton.MqttButtonValidator>, IHasUniqueId,
    IHasAvailability, IHasAvailabilityPayloads, IHasQos, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasRetain, IHasEntityCategory,
    IHasDefaultEntityId, IHasEncoding, IHasName, IHasEntityPicture, IHasVisibleByDefault, IHasMessageExpiryInterval
{
    public MqttButton(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
    {
    }

    /// <summary>
    /// The MQTT topic to publish commands to trigger the button.
    /// </summary>
    public string? CommandTopic { get; set; }

    /// <summary>
    /// Defines a [template](/docs/configuration/templating/#using-templates-with-the-mqtt-integration) to generate the payload to send to `command_topic`.
    /// </summary>
    public string? CommandTemplate { get; set; }

    /// <summary>
    /// The type/class of the button to set the icon in the frontend.
    /// See https://www.home-assistant.io/integrations/button/#device-class
    /// </summary>
    /// <remarks>Default value: 'None'</remarks>
    public Optional<HassButtonDeviceClass?> DeviceClass { get; set; }

    /// <summary>
    /// The payload to send to trigger the button.
    /// </summary>
    /// <remarks>Default value: 'PRESS'</remarks>
    public string? PayloadPress { get; set; }

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
    public string? PayloadAvailable { get; set; }
    /// <inheritdoc />
    public string? PayloadNotAvailable { get; set; }
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

    public class MqttButtonValidator : MqttSensorDiscoveryBaseValidator<MqttButton>
    {
        public MqttButtonValidator()
        {
            TopicAndTemplate(x => x.CommandTopic, x => x.CommandTemplate);
        }
    }
}
