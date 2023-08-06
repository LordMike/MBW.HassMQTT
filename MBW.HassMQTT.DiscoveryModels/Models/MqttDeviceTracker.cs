#nullable enable
using System.Collections.Generic;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models;

/// <summary>
/// https://www.home-assistant.io/integrations/device_tracker.mqtt/
///
/// The mqtt device tracker platform allows you to define new device_trackers through manual YAML
/// configuration in configuration.yaml and also to automatically discover device_trackers through
/// a discovery schema using the MQTT Discovery protocol.
/// </summary>
[DeviceType(HassDeviceType.DeviceTracker)]
[PublicAPI]
public class MqttDeviceTracker :
    MqttSensorDiscoveryBase<MqttDeviceTracker, MqttDeviceTracker.MqttDeviceTrackerValidator>, IHasUniqueId,
    IHasAvailability, IHasQos, IHasJsonAttributes, IHasIcon, IHasObjectId
{
    public MqttDeviceTracker(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
    {
    }

    /// <summary>
    /// The name of the MQTT device_tracker.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The payload value that represents the ‘home’ state for the device.
    /// </summary>
    /// <remarks>Default value: 'home'</remarks>
    public string? PayloadHome { get; set; }

    /// <summary>
    /// The payload value that represents the ‘not_home’ state for the device.
    /// </summary>
    /// <remarks>Default value: 'not_home'</remarks>
    public string? PayloadNotHome { get; set; }

    /// <summary>
    /// The payload value that will have the device's location automatically derived from Home Assistant's zones.
    /// </summary>
    /// <remarks>Default value: None</remarks>
    public string? PayloadReset { get; set; }

    /// <summary>
    /// Attribute of a device tracker that affects state when being used to track a person. Valid options are gps, router, bluetooth, or bluetooth_le.
    /// </summary>
    public DeviceTrackerSourceType? SourceType { get; set; }

    /// <summary>
    /// The MQTT topic subscribed to receive device tracker state changes. The states defined in `state_topic` override the location states defined by the `json_attributes_topic`. This state override is turned inactive if the `state_topic` receives a message containing `payload_reset`. The `state_topic` can only be omitted if `json_attributes_topic` is used.
    /// </summary>
    public string? StateTopic { get; set; }

    /// <summary>
    /// Defines a template that returns a device tracker state.
    /// </summary>
    public string? ValueTemplate { get; set; }

    public string? UniqueId { get; set; }
    public IList<AvailabilityModel>? Availability { get; set; }
    public AvailabilityMode? AvailabilityMode { get; set; }
    public MqttQosLevel? Qos { get; set; }
    public string? JsonAttributesTemplate { get; set; }
    public string? JsonAttributesTopic { get; set; }
    public string? Icon { get; set; }
    public string? ObjectId { get; set; }

    public class MqttDeviceTrackerValidator : MqttSensorDiscoveryBaseValidator<MqttDeviceTracker>
    {
        public MqttDeviceTrackerValidator()
        {
            TopicAndTemplate(s => s.StateTopic, s => s.ValueTemplate);
        }
    }
}