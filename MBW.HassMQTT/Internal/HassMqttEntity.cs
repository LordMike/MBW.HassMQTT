using System;
using System.Collections.Generic;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.Interfaces;

namespace MBW.HassMQTT.Internal;

internal sealed class HassMqttEntity : IHassMqttEntity
{
    private readonly IReadOnlyDictionary<HassTopicKind, MqttStateValueTopic> _valueSenders;
    private readonly IReadOnlyList<MqttStateValueTopic> _valueSources;
    private readonly MqttAttributesTopic _attributesSender;

    public string DeviceId { get; }
    public string EntityId { get; }
    public string UniqueId { get; }

    internal DiscoveryPublishOperation Discovery { get; }
    internal IReadOnlyList<CompiledPublishOperation> PublishingPlan { get; }

    public HassMqttEntity(
        string deviceId,
        string entityId,
        string uniqueId,
        DiscoveryPublishOperation discovery,
        IReadOnlyDictionary<HassTopicKind, MqttStateValueTopic> valueSenders,
        MqttAttributesTopic attributesSender,
        IReadOnlyList<MqttStateValueTopic> valueSources,
        IReadOnlyList<CompiledPublishOperation> publishingPlan)
    {
        DeviceId = deviceId;
        EntityId = entityId;
        UniqueId = uniqueId;
        Discovery = discovery;
        _valueSenders = valueSenders;
        _attributesSender = attributesSender;
        _valueSources = valueSources;
        PublishingPlan = publishingPlan;
    }

    public MqttStateValueTopic GetValueSender(HassTopicKind topicKind)
    {
        if (_valueSenders.TryGetValue(topicKind, out MqttStateValueTopic sender))
            return sender;

        throw new InvalidOperationException($"Entity {DeviceId}/{EntityId} was not configured with a {topicKind} value topic");
    }

    public MqttAttributesTopic GetAttributesSender()
    {
        if (_attributesSender != null)
            return _attributesSender;

        throw new InvalidOperationException($"Entity {DeviceId}/{EntityId} was not configured with a JSON attributes topic");
    }

    internal void MarkDirty()
    {
        Discovery.MarkDirty();
        foreach (MqttStateValueTopic source in _valueSources)
            source.MarkDirty();
        _attributesSender?.MarkDirty();
    }
}
