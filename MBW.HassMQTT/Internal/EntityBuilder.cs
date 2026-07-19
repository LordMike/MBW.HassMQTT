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
    private const string CombinedStateTemplate = "{{ value_json.state }}";
    private const string CombinedAttributesTemplate = "{{ value_json.attributes | tojson }}";

    private readonly HassMqttManager _hassMqttManager;
    private readonly byte[] _discoverySnapshot;
    private readonly HassTopicKind[] _requiredTopics;
    private readonly bool _publishStateAndAttributesTogether;

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

    private EntityBuilder(
        HassMqttManager hassMqttManager,
        byte[] discoverySnapshot,
        HassTopicKind[] requiredTopics,
        bool publishStateAndAttributesTogether)
    {
        _hassMqttManager = hassMqttManager;
        _discoverySnapshot = discoverySnapshot;
        _requiredTopics = requiredTopics;
        _publishStateAndAttributesTogether = publishStateAndAttributesTogether;
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

        return new EntityBuilder<TEntity>(
            _hassMqttManager,
            _discoverySnapshot,
            requiredTopics.ToArray(),
            _publishStateAndAttributesTogether);
    }

    internal IEntityBuilder<TEntity> PublishStateAndAttributesTogether()
    {
        if (_publishStateAndAttributesTogether)
            return this;

        List<HassTopicKind> requiredTopics = new List<HassTopicKind>(_requiredTopics);
        if (!requiredTopics.Contains(HassTopicKind.State))
            requiredTopics.Add(HassTopicKind.State);
        if (!requiredTopics.Contains(HassTopicKind.JsonAttributes))
            requiredTopics.Add(HassTopicKind.JsonAttributes);

        return new EntityBuilder<TEntity>(
            _hassMqttManager,
            _discoverySnapshot,
            requiredTopics.ToArray(),
            true);
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

        StateAndAttributesPublishingCapability combinedCapability = null;
        if (_publishStateAndAttributesTogether)
        {
            if (!StateAndAttributesPublishingCapability.TryCreate(typeof(TEntity), out combinedCapability))
                throw new NotSupportedException(
                    $"{typeof(TEntity).Name} does not expose one templatable State topic and JSON attributes");

            ConfigureCombinedDiscovery(discovery, deviceId, entityId, combinedCapability);
        }

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
                    sources.Add(attributesSender);
                continue;
            }

            if (!valueSendersByTopic.TryGetValue(topic, out MqttStateValueTopic sender))
            {
                sender = new MqttStateValueTopic(topic);
                valueSendersByTopic.Add(topic, sender);
                sources.Add(sender);
            }

            valueSenders.Add(topicKind, sender);
        }

        MqttStateValueTopic combinedStateSender = _publishStateAndAttributesTogether
            ? valueSenders[HassTopicKind.State]
            : null;
        HashSet<MqttStateValueTopic> compiledValueSenders = new HashSet<MqttStateValueTopic>();
        bool combinedOperationAdded = false;

        foreach (HassTopicKind topicKind in _requiredTopics)
        {
            if (topicKind == HassTopicKind.JsonAttributes)
            {
                if (!_publishStateAndAttributesTogether)
                {
                    publishingPlan.Add(CompiledPublishOperation.CreateTransform<object, object>(
                        attributesSender.PublishTopic,
                        attributesSender,
                        value => value,
                        PublishOperationKind.Attributes));
                }
                continue;
            }

            MqttStateValueTopic sender = valueSenders[topicKind];
            if (_publishStateAndAttributesTogether && ReferenceEquals(sender, combinedStateSender))
            {
                if (!combinedOperationAdded)
                {
                    publishingPlan.Add(CompiledPublishOperation.CreateComposition<object, Dictionary<string, object>, StateAndAttributesPayload>(
                        combinedStateSender.PublishTopic,
                        combinedStateSender,
                        attributesSender,
                        (state, attributes) => new StateAndAttributesPayload(state, attributes),
                        PublishOperationKind.Value,
                        () => combinedStateSender.Initialized));
                    combinedOperationAdded = true;
                }

                compiledValueSenders.Add(sender);
                continue;
            }

            if (compiledValueSenders.Add(sender))
            {
                publishingPlan.Add(CompiledPublishOperation.CreateTransform<object, object>(
                    sender.PublishTopic,
                    sender,
                    value => value,
                    PublishOperationKind.Value));
            }
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
        new EntityBuilder<TEntity>(
            _hassMqttManager,
            DiscoverySnapshotSerializer.Capture(discovery),
            _requiredTopics,
            _publishStateAndAttributesTogether);

    private void ConfigureCombinedDiscovery(
        TEntity discovery,
        string deviceId,
        string entityId,
        StateAndAttributesPublishingCapability capability)
    {
        IHasJsonAttributes attributes = (IHasJsonAttributes)discovery;
        string stateTopic = capability.StateTopicProperty.GetValue(discovery) as string;
        string attributesTopic = attributes.JsonAttributesTopic;
        bool stateTopicMissing = string.IsNullOrWhiteSpace(stateTopic);
        bool attributesTopicMissing = string.IsNullOrWhiteSpace(attributesTopic);
        List<ValidationFailure> errors = new List<ValidationFailure>();

        if (stateTopicMissing && attributesTopicMissing)
        {
            stateTopic = _hassMqttManager.TopicBuilder.GetServiceTopic(deviceId, entityId, "state");
        }
        else if (stateTopicMissing || attributesTopicMissing ||
                 !string.Equals(stateTopic, attributesTopic, StringComparison.Ordinal))
        {
            errors.Add(new ValidationFailure(
                capability.StateTopicProperty.Name,
                $"{capability.StateTopicProperty.Name} and {nameof(IHasJsonAttributes.JsonAttributesTopic)} must be identical when state and attributes are published together"));
        }

        string stateTemplate = capability.StateTemplateProperty.GetValue(discovery) as string;
        if (stateTemplate != null && !string.Equals(stateTemplate, CombinedStateTemplate, StringComparison.Ordinal))
        {
            errors.Add(new ValidationFailure(
                capability.StateTemplateProperty.Name,
                $"Custom {capability.StateTemplateProperty.Name} values are not supported when state and attributes are published together"));
        }

        if (attributes.JsonAttributesTemplate != null &&
            !string.Equals(attributes.JsonAttributesTemplate, CombinedAttributesTemplate, StringComparison.Ordinal))
        {
            errors.Add(new ValidationFailure(
                nameof(IHasJsonAttributes.JsonAttributesTemplate),
                "Custom JsonAttributesTemplate values are not supported when state and attributes are published together"));
        }

        foreach (System.Reflection.PropertyInfo property in capability.AdditionalStateTemplateProperties)
        {
            if (property.GetValue(discovery) != null)
            {
                errors.Add(new ValidationFailure(
                    property.Name,
                    $"{property.Name} consumes the combined state payload and is not supported when state and attributes are published together"));
            }
        }

        if (errors.Count > 0)
            throw new ValidationException(errors);

        capability.StateTopicProperty.SetValue(discovery, stateTopic);
        attributes.JsonAttributesTopic = stateTopic;
        capability.StateTemplateProperty.SetValue(discovery, CombinedStateTemplate);
        attributes.JsonAttributesTemplate = CombinedAttributesTemplate;
    }

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
