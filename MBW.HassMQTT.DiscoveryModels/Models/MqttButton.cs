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
public class MqttButton : MqttSensorDiscoveryBase<MqttButton, MqttButton.MqttButtonValidator>, IHasUniqueId, IHasAvailability, IHasQos, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasRetain, IHasEntityCategory, IHasObjectId, IHasEncoding
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
    public HassButtonDeviceClass? DeviceClass { get; set; }

    /// <summary>
    /// The name to use when displaying this button.
    /// </summary>
    /// <remarks>Default value: 'MQTT Button'</remarks>
    public string? Name { get; set; }

    /// <summary>
    /// The payload to send to trigger the button.
    /// </summary>
    /// <remarks>Default value: 'PRESS'</remarks>
    public string? PayloadPress { get; set; }

    public string? UniqueId { get; set; }
    public IList<AvailabilityModel>? Availability { get; set; }
    public AvailabilityMode? AvailabilityMode { get; set; }
    public MqttQosLevel? Qos { get; set; }
    public string? JsonAttributesTemplate { get; set; }
    public string? JsonAttributesTopic { get; set; }
    public string? Icon { get; set; }
    public bool? EnabledByDefault { get; set; }
    public bool? Retain { get; set; }
    public EntityCategory? EntityCategory { get; set; }
    public string? ObjectId { get; set; }
    public string? Encoding { get; set; }

    public class MqttButtonValidator : MqttSensorDiscoveryBaseValidator<MqttButton>
    {
        public MqttButtonValidator()
        {
            TopicAndTemplate(x => x.CommandTopic, x => x.CommandTemplate);
        }
    }
}