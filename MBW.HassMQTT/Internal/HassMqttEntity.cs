using System;
using System.Collections.Generic;
using MBW.HassMQTT.Abstracts.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.Interfaces;

namespace MBW.HassMQTT.Internal;

internal sealed class HassMqttEntity : IHassMqttEntity
{
    private readonly IReadOnlyDictionary<HassTopicKind, MqttStateValueTopic> _valueSenders;
    private readonly IReadOnlyList<IMqttValueContainer> _sources;
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
        IReadOnlyList<IMqttValueContainer> sources,
        IReadOnlyList<CompiledPublishOperation> publishingPlan)
    {
        DeviceId = deviceId;
        EntityId = entityId;
        UniqueId = uniqueId;
        Discovery = discovery;
        _valueSenders = valueSenders;
        _attributesSender = attributesSender;
        _sources = sources;
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
        foreach (IMqttValueContainer source in _sources)
            source.MarkDirty();
    }
}
