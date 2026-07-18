#nullable enable

using FluentValidation;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models;

/// <summary>
/// MQTT discovery schema for a Home Assistant date/time entity.
/// </summary>
[DeviceType(HassDeviceType.DateTime)]
[PublicAPI]
public class MqttDateTime : MqttTemporalDiscoveryBase<MqttDateTime, MqttDateTime.MqttDateTimeValidator>
{
    public MqttDateTime(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
    {
    }

    /// <summary>
    /// The IANA timezone identifier used when state updates do not include timezone information.
    /// </summary>
    public string? Timezone { get; set; }

    public class MqttDateTimeValidator : MqttSensorDiscoveryBaseValidator<MqttDateTime>
    {
        public MqttDateTimeValidator()
        {
            RuleFor(model => model.CommandTopic).NotEmpty();
            TopicAndTemplate(model => model.CommandTopic, model => model.CommandTemplate);
            TopicAndTemplate(model => model.StateTopic, model => model.ValueTemplate);
        }
    }
}
