#nullable enable

using FluentValidation;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models;

/// <summary>
/// MQTT discovery schema for a Home Assistant date entity.
/// </summary>
[DeviceType(HassDeviceType.Date)]
[PublicAPI]
public class MqttDate : MqttTemporalDiscoveryBase<MqttDate, MqttDate.MqttDateValidator>
{
    public MqttDate(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
    {
    }

    public class MqttDateValidator : MqttSensorDiscoveryBaseValidator<MqttDate>
    {
        public MqttDateValidator()
        {
            RuleFor(model => model.CommandTopic).NotEmpty();
            TopicAndTemplate(model => model.CommandTopic, model => model.CommandTemplate);
            TopicAndTemplate(model => model.StateTopic, model => model.ValueTemplate);
        }
    }
}
