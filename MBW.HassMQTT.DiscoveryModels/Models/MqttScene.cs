#nullable enable
using System.Collections.Generic;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models;

/// <summary>
/// https://www.home-assistant.io/integrations/scene.mqtt/
///
/// The mqtt scene platform lets you control your MQTT enabled scenes.
/// </summary>
[DeviceType(HassDeviceType.Scene)]
[PublicAPI]
public class MqttScene : MqttSensorDiscoveryBase<MqttScene, MqttScene.MqttSceneValidator>, IHasUniqueId,
    IHasAvailability, IHasQos, IHasIcon, IHasEnabledByDefault, IHasRetain, IHasEntityCategory, IHasObjectId, IHasName
{
    public MqttScene(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
    {
    }

    /// <summary>
    /// The MQTT topic to publish `payload_on` to activate the scene.
    /// </summary>
    public string? CommandTopic { get; set; }

    /// <summary>
    /// The payload that will be sent to `command_topic` when activating the MQTT scene.
    /// </summary>
    public string? PayloadOn { get; set; }

    public string? UniqueId { get; set; }
    public IList<AvailabilityModel>? Availability { get; set; }
    public AvailabilityMode? AvailabilityMode { get; set; }
    public MqttQosLevel? Qos { get; set; }
    public string? Icon { get; set; }
    public bool? EnabledByDefault { get; set; }
    public bool? Retain { get; set; }
    public EntityCategory? EntityCategory { get; set; }
    public string? ObjectId { get; set; }
    public string? Name { get; set; }

    public class MqttSceneValidator : MqttSensorDiscoveryBaseValidator<MqttScene>
    {
        public MqttSceneValidator()
        {
        }
    }
}