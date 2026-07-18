#nullable enable

using FluentValidation;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models;

/// <summary>
/// MQTT discovery schema for a Home Assistant time entity.
/// </summary>
[DeviceType(HassDeviceType.Time)]
[PublicAPI]
public class MqttTime : MqttTemporalDiscoveryBase<MqttTime, MqttTime.MqttTimeValidator>
{
    public MqttTime(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
    {
    }

    public class MqttTimeValidator : MqttSensorDiscoveryBaseValidator<MqttTime>
    {
        public MqttTimeValidator()
        {
            RuleFor(model => model.CommandTopic).NotEmpty();
            TopicAndTemplate(model => model.CommandTopic, model => model.CommandTemplate);
            TopicAndTemplate(model => model.StateTopic, model => model.ValueTemplate);
        }
    }
}
