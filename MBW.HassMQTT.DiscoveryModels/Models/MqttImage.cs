#nullable enable
using System.Collections.Generic;
using FluentValidation;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;
using MBW.HassMQTT.DiscoveryModels.Validation;

namespace MBW.HassMQTT.DiscoveryModels.Models;

/// <summary>
/// https://www.home-assistant.io/integrations/image.mqtt/
///
/// The mqtt image platform allows you to integrate the content of an image file sent through MQTT into Home Assistant as an image. The image platform is a simplified version of the camera platform that only accepts images. Every time a message under the image_topic in the configuration is received, the image displayed in Home Assistant will also be updated. Messages received on image_topic should contain the full contents of an image file, for example, a JPEG image, without any additional encoding or metadata.
/// </summary>
[DeviceType(HassDeviceType.Image)]
[PublicAPI]
public class MqttImage : MqttSensorDiscoveryBase<MqttImage, MqttImage.MqttImageValidator>, IHasAvailability,
    IHasEnabledByDefault, IHasEncoding, IHasEntityCategory, IHasIcon, IHasJsonAttributes, IHasObjectId, IHasUniqueId
{
    public MqttImage(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
    {
    }

    /// <summary>
    /// The content type of and image data message received on `image_topic`. This option cannot be used with the `from_url_topic` because the content type is derived when downloading the image.
    /// </summary>
    public string? ContentType { get; set; }
    
    /// <summary>
    /// The encoding of the image payloads received. Set to `"b64"` to enable base64 decoding of image payload. If not set, the image payload must be raw binary data.
    /// </summary>
    public string? ImageEncoding { get; set; }
    
    /// <summary>
    /// The MQTT topic to subscribe to receive the image payload of the image to be downloaded. Ensure the `content_type` type option is set to the corresponding content type. This option cannot be used together with the `url_topic` option. But at least one of these option is required.
    /// </summary>
    public string? ImageTopic { get; set; }
    
    /// <summary>
    /// The name of the image.
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Defines a template to extract the image URL from a message received at `url_topic`.
    /// </summary>
    public string? UrlTemplate { get; set; }
    
    /// <summary>
    /// The MQTT topic to subscribe to receive an image URL. A `url_template` option can extract the URL from the message. The `content_type` will be derived from the image when downloaded. This option cannot be used together with the `image_topic` option, but at least one of these options is required.
    /// </summary>
    public string? UrlTopic { get; set; }

    public IList<AvailabilityModel>? Availability { get; set; }
    public AvailabilityMode? AvailabilityMode { get; set; }
    public bool? EnabledByDefault { get; set; }
    public string? Encoding { get; set; }
    public EntityCategory? EntityCategory { get; set; }
    public string? Icon { get; set; }
    public string? JsonAttributesTemplate { get; set; }
    public string? JsonAttributesTopic { get; set; }
    public string? ObjectId { get; set; }
    public string? UniqueId { get; set; }

    public class MqttImageValidator : MqttSensorDiscoveryBaseValidator<MqttImage>
    {
        public MqttImageValidator()
        {
            TopicAndTemplate(s => s.UrlTopic, s => s.UrlTemplate);

            RuleFor(s => s.UrlTopic)
                .IsValidMqttTopic()
                .When(s => !string.IsNullOrEmpty(s.UrlTemplate));
            RuleFor(s => s.ImageTopic)
                .IsValidMqttTopic()
                .When(s => !string.IsNullOrEmpty(s.ImageEncoding));

            RuleFor(s => s.ImageEncoding)
                .NotEmpty()
                .When(s => !string.IsNullOrEmpty(s.ImageTopic));
        }
    }
}