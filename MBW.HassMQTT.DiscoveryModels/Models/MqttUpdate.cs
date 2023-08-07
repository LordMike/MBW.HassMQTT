#nullable enable
using System.Collections.Generic;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models;

/// <summary>
/// https://www.home-assistant.io/integrations/update.mqtt/
/// 
/// The `mqtt` Update platform allows you to integrate devices that might expose firmware/software installed and the latest versions through
/// MQTT into Home Assistant as an Update entity. Every time a message under the `topic` in the configuration is received, the entity will
/// be updated in Home Assistant.
/// </summary>
[DeviceType(HassDeviceType.Update)]
[PublicAPI]
public class MqttUpdate : MqttSensorDiscoveryBase<MqttUpdate, MqttUpdate.MqttUpdateValidator>, IHasUniqueId,
    IHasAvailability, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasObjectId, IHasQos, IHasEntityCategory,
    IHasEncoding, IHasRetain, IHasName
{
    public MqttUpdate(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
    {
    }

    /// <summary>
    /// The MQTT topic to publish `payload_install` to start installing process.
    /// </summary>
    public string? CommandTopic { get; set; }

    /// <summary>
    /// The type/class of the update to set the icon in the frontend.
    /// See https://www.home-assistant.io/integrations/update/#device-classes
    /// </summary>
    /// <remarks>Default value: 'None'</remarks>
    public HassUpdateDeviceClass? DeviceClass { get; set; }

    /// <summary>
    /// Picture URL for the entity.
    /// </summary>
    public string? EntityPicture { get; set; }

    /// <summary>
    /// Defines a template to extract the latest version value.
    /// </summary>
    public string? LatestVersionTemplate { get; set; }

    /// <summary>
    /// The MQTT topic subscribed to receive an update of the latest version.
    /// </summary>
    public string? LatestVersionTopic { get; set; }

    /// <summary>
    /// The MQTT payload to start installing process.
    /// </summary>
    public string? PayloadInstall { get; set; }

    /// <summary>
    /// Summary of the release notes or changelog. This is suitable a brief update description of max 255 characters.
    /// </summary>
    public string? ReleaseSummary { get; set; }

    /// <summary>
    /// URL to the full release notes of the latest version available.
    /// </summary>
    public string? ReleaseUrl { get; set; }

    /// <summary>
    /// The MQTT topic subscribed to receive state updates. The state update may be either JSON or a simple string with `installed_version` value. When a JSON payload is detected, the state value of the JSON payload should supply the `installed_version` and can optional supply: `latest_version`, `title`, `release_summary`, `release_url` or an `entity_picture` URL.
    /// </summary>
    public string? StateTopic { get; set; }

    /// <summary>
    /// Title of the software, or firmware update. This helps to differentiate between the device or entity name versus the title of the software installed.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Defines a template to extract the `installed_version` state value or to render to a valid JSON payload on from the payload received on `state_topic`.
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

    public class MqttUpdateValidator : MqttSensorDiscoveryBaseValidator<MqttUpdate>
    {
        public MqttUpdateValidator()
        {
            TopicAndTemplate(x => x.LatestVersionTopic, x => x.LatestVersionTemplate);
            TopicAndTemplate(x => x.StateTopic, x => x.ValueTemplate);
        }
    }
}