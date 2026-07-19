#nullable enable
using System.Collections.Generic;
using FluentValidation;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models;

/// <summary>
/// https://www.home-assistant.io/integrations/camera.mqtt/
///
/// The mqtt camera platform allows you to integrate the content of an image file sent through
/// MQTT into Home Assistant as a camera. Every time a message under the topic in the configuration
/// is received, the image displayed in Home Assistant will also be updated. Messages received on
/// `topic` should contain the full contents of an image file, for example, a JPEG image, without
/// any additional encoding or metadata.
/// 
/// This can be used with an application or a service capable of sending images through MQTT.
/// </summary>
[DeviceType(HassDeviceType.Camera)]
[PublicAPI]
public class MqttCamera : MqttSensorDiscoveryBase<MqttCamera, MqttCamera.MqttCameraValidator>, IHasUniqueId,
    IHasAvailability, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasEntityCategory, IHasDefaultEntityId, IHasEntityPicture, IHasVisibleByDefault,
    IHasEncoding, IHasName
{
    public MqttCamera(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
    {
    }

    /// <summary>
    /// The MQTT topic to subscribe to.
    /// </summary>
    public string Topic { get; set; }

    /// <summary>
    /// The encoding of the image payloads received. Set to `"b64"` to enable base64 decoding of image payload. If not set, or if set to `null`, the image payload must be raw binary data.
    /// </summary>
    public string? Encoding { get; set; }

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
    public Optional<string?> Name { get; set; }

    public class MqttCameraValidator : MqttSensorDiscoveryBaseValidator<MqttCamera>
    {
        public MqttCameraValidator()
        {
            RuleFor(s => s.Encoding).Must(x => x == "b64" || x == "null").When(x => x != null);
        }
    }
}
