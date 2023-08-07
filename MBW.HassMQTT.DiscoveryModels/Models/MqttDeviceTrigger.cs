#nullable enable

using FluentValidation;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models;

/// <summary>
/// https://www.home-assistant.io/integrations/device_trigger.mqtt/
///
/// The mqtt device trigger platform uses an MQTT message payload to generate device trigger events.
/// 
/// An MQTT device trigger is a better option than a binary sensor for buttons, remote controls etc.
/// </summary>
/// <remarks>
/// MQTT device triggers are only supported through MQTT discovery, manual setup through configuration.yaml
/// is not supported. The discovery topic needs to be: &lt;discovery_prefix&gt;/device_automation/[&lt;node_id&gt;/]&lt;object_id&gt;/config.
/// Note that only one trigger may be defined per unique discovery topic. Also note that the combination of type and subtype
/// should be unique for a device.
/// </remarks>
[DeviceType(HassDeviceType.DeviceTrigger)]
[PublicAPI]
public class
    MqttDeviceTrigger : MqttSensorDiscoveryBase<MqttDeviceTrigger, MqttDeviceTrigger.MqttDeviceTriggerValidator>,
        IHasQos
{
    public MqttDeviceTrigger(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
    {
    }

    /// <summary>
    /// The type of automation, must be 'trigger'.
    /// </summary>
    public string AutomationType { get; set; } = "trigger";

    /// <summary>
    /// Optional payload to match the payload being sent over the topic.
    /// </summary>
    public string? Payload { get; set; }

    /// <summary>
    /// The MQTT topic subscribed to receive trigger events.
    /// </summary>
    public string Topic { get; set; }

    /// <summary>
    /// The type of the trigger, e.g. `button_short_press`.
    ///
    /// Entries supported by the frontend:
    /// - button_short_press
    /// - button_short_release
    /// - button_long_press
    /// - button_long_release
    /// - button_double_press
    /// - button_triple_press
    /// - button_quadruple_press
    /// - button_quintuple_press
    ///
    /// If set to an unsupported value, will render as `subtype type`, e.g. `button_1 spammed` with `type` set to `spammed` and `subtype` set to `button_1`
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// The subtype of the trigger, e.g. `button_1`.
    ///
    /// Entries supported by the frontend:
    /// - turn_on
    /// - turn_off
    /// - button_1
    /// - button_2
    /// - button_3
    /// - button_4
    /// - button_5
    /// - button_6
    ///
    /// If set to an unsupported value, will render as `subtype type`, e.g. `left_button pressed` with `type` set to `button_short_press` and `subtype` set to `left_button`
    /// </summary>
    public string Subtype { get; set; }

    /// <summary>
    /// Defines a [template](/docs/configuration/templating/#using-templates-with-the-mqtt-integration) to extract the value.
    /// </summary>
    public string? ValueTemplate { get; set; }

    public MqttQosLevel? Qos { get; set; }

    public class MqttDeviceTriggerValidator : MqttSensorDiscoveryBaseValidator<MqttDeviceTrigger>
    {
        public MqttDeviceTriggerValidator()
        {
            TopicAndTemplate(s => s.Topic, s => s.ValueTemplate);

            RuleFor(s => s.AutomationType).Equal("trigger");
        }
    }
}