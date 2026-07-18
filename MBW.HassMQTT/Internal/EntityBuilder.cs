using System;
using System.Collections.Generic;
using EnumsNET;
using FluentValidation;
using FluentValidation.Results;
using MBW.HassMQTT.Abstracts.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Device;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.Extensions;
using MBW.HassMQTT.Interfaces;
using MBW.HassMQTT.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace MBW.HassMQTT.Internal;

internal sealed class EntityBuilder<TEntity> : IEntityBuilder<TEntity> where TEntity : IHassDiscoveryDocument
{
    private readonly HassMqttManager _hassMqttManager;
    private readonly byte[] _discoverySnapshot;
    private readonly HassTopicKind[] _requiredTopics;

    HassMqttManager IEntityBuilder<TEntity>.HassMqttManager => _hassMqttManager;

    internal EntityBuilder(HassMqttManager hassMqttManager, bool autoConfigureAttributesTopic)
    {
        _hassMqttManager = hassMqttManager;

        TEntity discovery = CreateDocument(string.Empty, null);
        _discoverySnapshot = DiscoverySnapshotSerializer.Capture(discovery);
        _requiredTopics = autoConfigureAttributesTopic && typeof(IHasJsonAttributes).IsAssignableFrom(typeof(TEntity))
            ? new[] { HassTopicKind.JsonAttributes }
            : Array.Empty<HassTopicKind>();
    }

    private EntityBuilder(HassMqttManager hassMqttManager, byte[] discoverySnapshot, HassTopicKind[] requiredTopics)
    {
        _hassMqttManager = hassMqttManager;
        _discoverySnapshot = discoverySnapshot;
        _requiredTopics = requiredTopics;
    }

    public IEntityBuilder<TEntity> ConfigureTopics(params HassTopicKind[] topicKinds)
    {
        if (topicKinds == null)
            throw new ArgumentNullException(nameof(topicKinds));

        List<HassTopicKind> requiredTopics = new List<HassTopicKind>(_requiredTopics);
        foreach (HassTopicKind topicKind in topicKinds)
        {
            if (!Enum.IsDefined(typeof(HassTopicKind), topicKind))
                throw new ArgumentOutOfRangeException(nameof(topicKinds), topicKind, "Unknown topic kind");
            if (typeof(TEntity).GetProperty($"{topicKind}Topic") == null)
                throw new NotSupportedException($"Unable to configure topic {topicKind} on {typeof(TEntity).Name}");
            if (!requiredTopics.Contains(topicKind))
                requiredTopics.Add(topicKind);
        }

        return new EntityBuilder<TEntity>(_hassMqttManager, _discoverySnapshot, requiredTopics.ToArray());
    }

    public IEntityBuilder<TEntity> ConfigureDiscovery(Action<TEntity> configure)
    {
        if (configure == null)
            throw new ArgumentNullException(nameof(configure));

        TEntity discovery = Materialize(string.Empty, null);
        configure(discovery);
        return WithDiscovery(discovery);
    }

    public IEntityBuilder<TEntity> ConfigureDevice(Action<MqttDeviceDocument> configure)
    {
        if (configure == null)
            throw new ArgumentNullException(nameof(configure));

        TEntity discovery = Materialize(string.Empty, null);
        configure(discovery.Device);
        return WithDiscovery(discovery);
    }

    public IHassMqttEntity Build(string deviceId, string entityId, string uniqueId = null)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(deviceId));
        if (string.IsNullOrWhiteSpace(entityId))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(entityId));
        if (uniqueId != null && string.IsNullOrWhiteSpace(uniqueId))
            throw new ArgumentException("Value cannot be empty or whitespace.", nameof(uniqueId));

        string discoveryTopic = _hassMqttManager.TopicBuilder.GetDiscoveryTopic<TEntity>(deviceId, entityId);
        TEntity discovery = Materialize(discoveryTopic, null);

        string defaultUniqueId = $"{deviceId}_{entityId}".ToLowerInvariant();
        string configuredUniqueId = (discovery as IHasUniqueId)?.UniqueId;
        string resolvedUniqueId = uniqueId ?? (string.IsNullOrWhiteSpace(configuredUniqueId) ? defaultUniqueId : configuredUniqueId);
        if (discovery is IHasUniqueId hasUniqueId)
            hasUniqueId.UniqueId = resolvedUniqueId;

        foreach (HassTopicKind topicKind in _requiredTopics)
        {
            if (string.IsNullOrWhiteSpace(discovery.GetTopic(topicKind)))
            {
                string topicName = topicKind.AsString(EnumFormat.EnumMemberValue);
                discovery.SetTopic(topicKind, _hassMqttManager.TopicBuilder.GetServiceTopic(deviceId, entityId, topicName));
            }
        }

        Validate(discovery);

        Dictionary<HassTopicKind, MqttStateValueTopic> valueSenders = new Dictionary<HassTopicKind, MqttStateValueTopic>();
        Dictionary<string, MqttStateValueTopic> valueSendersByTopic = new Dictionary<string, MqttStateValueTopic>(StringComparer.OrdinalIgnoreCase);
        List<IMqttValueContainer> sources = new List<IMqttValueContainer>();
        List<CompiledPublishOperation> publishingPlan = new List<CompiledPublishOperation>();
        MqttAttributesTopic attributesSender = null;

        foreach (HassTopicKind topicKind in _requiredTopics)
        {
            string topic = discovery.GetTopic(topicKind);
            if (topicKind == HassTopicKind.JsonAttributes)
            {
                attributesSender ??= new MqttAttributesTopic(topic);
                if (!sources.Contains(attributesSender))
                {
                    sources.Add(attributesSender);
                    publishingPlan.Add(CompiledPublishOperation.CreateTransform<object, object>(
                        topic,
                        attributesSender,
                        value => value,
                        PublishOperationKind.Attributes));
                }
                continue;
            }

            if (!valueSendersByTopic.TryGetValue(topic, out MqttStateValueTopic sender))
            {
                sender = new MqttStateValueTopic(topic);
                valueSendersByTopic.Add(topic, sender);
                sources.Add(sender);
                publishingPlan.Add(CompiledPublishOperation.CreateTransform<object, object>(
                    topic,
                    sender,
                    value => value,
                    PublishOperationKind.Value));
            }

            valueSenders.Add(topicKind, sender);
        }

        byte[] discoveryPayload = PayloadSerializer.Serialize(discovery);
        HassMqttEntity entity = new HassMqttEntity(
            deviceId,
            entityId,
            resolvedUniqueId,
            new DiscoveryPublishOperation(discoveryTopic, discoveryPayload),
            valueSenders,
            attributesSender,
            sources,
            publishingPlan);

        _hassMqttManager.Register(entity);
        return entity;
    }

    private EntityBuilder<TEntity> WithDiscovery(TEntity discovery) =>
        new EntityBuilder<TEntity>(_hassMqttManager, DiscoverySnapshotSerializer.Capture(discovery), _requiredTopics);

    private TEntity Materialize(string discoveryTopic, string uniqueId)
    {
        TEntity discovery = CreateDocument(discoveryTopic, uniqueId);
        DiscoverySnapshotSerializer.Populate(_discoverySnapshot, discovery);
        return discovery;
    }

    private TEntity CreateDocument(string discoveryTopic, string uniqueId)
    {
        TEntity discovery = ActivatorUtilities.CreateInstance<TEntity>(
            _hassMqttManager.ServiceProvider,
            discoveryTopic ?? string.Empty,
            uniqueId ?? string.Empty);

        if (uniqueId == null && discovery is IHasUniqueId hasUniqueId)
            hasUniqueId.UniqueId = null;

        return discovery;
    }

    private void Validate(TEntity discovery)
    {
        ValidationResult validation = discovery.Validator.Validate(new ValidationContext<IHassDiscoveryDocument>(discovery));
        List<ValidationFailure> errors = new List<ValidationFailure>();
        foreach (ValidationFailure failure in validation.Errors)
        {
            if (failure.Severity == Severity.Error)
                errors.Add(failure);
            else
                _hassMqttManager.LogValidationFailure(failure);
        }

        if (errors.Count > 0)
            throw new ValidationException(errors);
    }
}
