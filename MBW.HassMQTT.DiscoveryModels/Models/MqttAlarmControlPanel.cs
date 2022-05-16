#nullable enable
using System.Collections.Generic;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models;

/// <summary>
/// https://www.home-assistant.io/integrations/alarm_control_panel.mqtt/
///
/// The mqtt alarm panel platform enables the possibility to control MQTT capable alarm panels. The Alarm icon
/// will change state after receiving a new state from state_topic. If these messages are published with RETAIN flag,
/// the MQTT alarm panel will receive an instant state update after subscription and will start with the correct state.
/// Otherwise, the initial state will be unknown.
/// 
/// The integration will accept the following states from your Alarm Panel (in lower case):
/// - disarmed
/// - armed_home
/// - armed_away
/// - armed_night
/// - armed_vacation
/// - armed_custom_bypass
/// - pending
/// - triggered
/// - arming
/// - disarming
/// 
/// The integration can control your Alarm Panel by publishing to the command_topic when a user interacts with the
/// Home Assistant frontend.
/// </summary>
[DeviceType(HassDeviceType.AlarmControlPanel)]
[PublicAPI]
public class MqttAlarmControlPanel : MqttSensorDiscoveryBase<MqttAlarmControlPanel, MqttAlarmControlPanel.MqttAlarmControlPanelValidator>, IHasUniqueId, IHasAvailability, IHasQos, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasRetain, IHasEntityCategory, IHasObjectId, IHasEncoding
{
    public MqttAlarmControlPanel(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
    {
    }

    /// <summary>
    /// If defined, specifies a code to enable or disable the alarm in the frontend. Note that the code is validated locally and blocks sending MQTT messages to the remote device. For remote code validation, the code can be configured to either of the special values `REMOTE_CODE` (numeric code) or `REMOTE_CODE_TEXT` (text code). In this case, local code validation is bypassed but the frontend will still show a numeric or text code dialog. Use `command_template` to send the code to the remote device. Example configurations for remote code validation [can be found here](./#configurations-with-remote-code-validation).
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// If true the code is required to arm the alarm. If false the code is not validated.
    /// </summary>
    public bool? CodeArmRequired { get; set; }

    /// <summary>
    /// If true the code is required to disarm the alarm. If false the code is not validated.
    /// </summary>
    public bool? CodeDisarmRequired { get; set; }

    /// <summary>
    /// If true the code is required to trigger the alarm. If false the code is not validated.
    /// </summary>
    /// <remarks>Default is 'true'</remarks>
    public bool? CodeTriggerRequired { get; set; }

    /// <summary>
    /// The [template](/docs/configuration/templating/#processing-incoming-data) used for the command payload. Available variables: `action` and `code`.
    /// </summary>
    public string? CommandTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to publish commands to change the alarm state.
    /// </summary>
    public string CommandTopic { get; set; }

    /// <summary>
    /// The name of the alarm.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The payload to set armed-away mode on your Alarm Panel.
    /// </summary>
    public string? PayloadArmAway { get; set; }

    /// <summary>
    /// The payload to set armed-home mode on your Alarm Panel.
    /// </summary>
    public string? PayloadArmHome { get; set; }

    /// <summary>
    /// The payload to set armed-night mode on your Alarm Panel.
    /// </summary>
    public string? PayloadArmNight { get; set; }

    /// <summary>
    /// The payload to set armed-vacation mode on your Alarm Panel.
    /// </summary>
    public string? PayloadArmVacation { get; set; }

    /// <summary>
    /// The payload to set armed-custom-bypass mode on your Alarm Panel.
    /// </summary>
    public string? PayloadArmCustomBypass { get; set; }

    /// <summary>
    /// The payload to disarm your Alarm Panel.
    /// </summary>
    public string? PayloadDisarm { get; set; }

    /// <summary>
    /// The MQTT topic subscribed to receive state updates.
    /// </summary>
    public string StateTopic { get; set; }

    /// <summary>
    /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the value.
    /// </summary>
    public string? ValueTemplate { get; set; }

    /// <summary>
    /// The payload to trigger the alarm on your Alarm Panel.
    /// </summary>
    /// <remarks>Default value: 'TRIGGER'</remarks>
    public string? PayloadTrigger { get; set; }

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

    public class MqttAlarmControlPanelValidator : MqttSensorDiscoveryBaseValidator<MqttAlarmControlPanel>
    {
        public MqttAlarmControlPanelValidator()
        {
            TopicAndTemplate(s => s.CommandTopic, s => s.CommandTemplate);
            TopicAndTemplate(s => s.StateTopic, s => s.ValueTemplate);
        }
    }
}