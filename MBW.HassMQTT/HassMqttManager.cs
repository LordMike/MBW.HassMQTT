using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.Extensions;
using MBW.HassMQTT.Interfaces;
using MBW.HassMQTT.Internal;
using MBW.HassMQTT.Serialization;
using MBW.HassMQTT.Topics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Exceptions;
using MQTTnet.Protocol;

namespace MBW.HassMQTT;

public class HassMqttManager : IMqttEventReceiver
{
    private readonly SemaphoreSlim _flushLock = new SemaphoreSlim(1, 1);
    private readonly HassMqttManagerConfiguration _config;
    private readonly IHassMqttClient _mqttClient;
    private readonly ILogger<HassMqttManager> _logger;
    private readonly object _entityRegistryLock = new object();
    private readonly ConcurrentDictionary<string, HassMqttEntity> _entities;
    private readonly ConcurrentDictionary<string, MqttStateValueTopic> _values;
    private readonly ConcurrentDictionary<string, MqttAttributesTopic> _attributes;
    private readonly ConcurrentDictionary<string, byte> _removeTopics;

    internal IServiceProvider ServiceProvider { get; }
    internal HassMqttTopicBuilder TopicBuilder { get; }

    public HassMqttManager(
        IServiceProvider serviceProvider,
        IOptions<HassMqttManagerConfiguration> config,
        IHassMqttClient mqttClient,
        HassMqttTopicBuilder topicBuilder,
        ILogger<HassMqttManager> logger)
    {
        ServiceProvider = serviceProvider;
        _config = config.Value;
        _mqttClient = mqttClient;
        TopicBuilder = topicBuilder;
        _logger = logger;
        _entities = new ConcurrentDictionary<string, HassMqttEntity>(StringComparer.Ordinal);
        _values = new ConcurrentDictionary<string, MqttStateValueTopic>(StringComparer.Ordinal);
        _attributes = new ConcurrentDictionary<string, MqttAttributesTopic>(StringComparer.Ordinal);
        _removeTopics = new ConcurrentDictionary<string, byte>(StringComparer.Ordinal);
    }

    /// <summary>Creates an unregistered, reusable entity builder.</summary>
    public IEntityBuilder<TEntity> CreateEntity<TEntity>() where TEntity : IHassDiscoveryDocument =>
        new EntityBuilder<TEntity>(this, _config.AutoConfigureAttributesTopics);

    public void RemoveEntity<TEntity>(string deviceId, string entityId) where TEntity : IHassDiscoveryDocument
    {
        string discoveryTopic = TopicBuilder.GetDiscoveryTopic<TEntity>(deviceId, entityId);
        lock (_entityRegistryLock)
        {
            _entities.TryRemove(discoveryTopic, out _);
            _removeTopics.TryAdd(discoveryTopic, 0);
        }
    }

    public bool TryGetEntity<TEntity>(string deviceId, string entityId, out IHassMqttEntity entity)
        where TEntity : IHassDiscoveryDocument
    {
        bool found = _entities.TryGetValue(
            TopicBuilder.GetDiscoveryTopic<TEntity>(deviceId, entityId),
            out HassMqttEntity builtEntity);
        entity = builtEntity;
        return found;
    }

    public MqttAttributesTopic GetAttributesSender(string topic)
    {
        if (string.IsNullOrWhiteSpace(topic))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(topic));
        return _attributes.GetOrAdd(topic, static value => new MqttAttributesTopic(value));
    }

    public MqttStateValueTopic GetValueSender(string topic)
    {
        if (string.IsNullOrWhiteSpace(topic))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(topic));
        return _values.GetOrAdd(topic, static value => new MqttStateValueTopic(value));
    }

    public void MarkAllValuesDirty()
    {
        foreach (HassMqttEntity entity in _entities.Values)
            entity.MarkDirty();
        foreach (MqttStateValueTopic value in _values.Values)
            value.MarkDirty();
        foreach (MqttAttributesTopic value in _attributes.Values)
            value.MarkDirty();
    }

    public Task<MqttFlushResult> FlushAll(CancellationToken token = default) => FlushAll(false, token);

    private async Task<MqttFlushResult> FlushAll(bool waitForLock, CancellationToken token)
    {
        if (waitForLock)
            await _flushLock.WaitAsync(token);
        else if (!await _flushLock.WaitAsync(0, token))
            return new MqttFlushResult(MqttFlushStatus.Busy);

        int discoveryDocuments = 0, removedTopics = 0, values = 0, attributes = 0;
        try
        {
            if (!_mqttClient.IsConnected)
                return Result(MqttFlushStatus.Disconnected);

            foreach (HassMqttEntity entity in _entities.Values)
            {
                DiscoveryPublishOperation discovery = entity.Discovery;
                if (!discovery.Dirty)
                    continue;

                long revision = discovery.Revision;
                if (!_config.SendDiscoveryDocuments)
                {
                    discovery.MarkPublished(revision);
                    continue;
                }

                _logger.LogDebug("Publishing discovery document for {Identifier}", entity.UniqueId);
                MqttFlushStatus status = await Publish(discovery.Topic, discovery.Payload, token);
                if (status != MqttFlushStatus.Completed)
                    return Result(status);

                discovery.MarkPublished(revision);
                discoveryDocuments++;
            }

            foreach (string topic in _removeTopics.Keys)
            {
                MqttFlushStatus status = await Publish(topic, Array.Empty<byte>(), token);
                if (status != MqttFlushStatus.Completed)
                    return Result(status);
                _removeTopics.TryRemove(topic, out _);
                removedTopics++;
            }

            foreach (HassMqttEntity entity in _entities.Values)
            {
                foreach (CompiledPublishOperation operation in entity.PublishingPlan)
                {
                    if (!operation.TryCapture(out CompiledPublishOperation.CompiledPublishAttempt attempt))
                        continue;

                    MqttFlushStatus status = await Publish(attempt.Topic, attempt.Payload, token);
                    if (status != MqttFlushStatus.Completed)
                        return Result(status);

                    attempt.Acknowledge();
                    if (attempt.Kind == PublishOperationKind.Attributes)
                        attributes++;
                    else
                        values++;
                }
            }

            foreach (MqttStateValueTopic value in _values.Values)
            {
                if (!value.Dirty)
                    continue;

                MqttStateValueTopic.Snapshot snapshot = value.Capture();
                MqttFlushStatus status = await Publish(
                    value.PublishTopic,
                    MqttValueSerializer.SerializeStandalone(snapshot.Value),
                    token);
                if (status != MqttFlushStatus.Completed)
                    return Result(status);
                value.MarkPublished(snapshot.Revision);
                values++;
            }

            foreach (MqttAttributesTopic value in _attributes.Values)
            {
                if (!value.Dirty)
                    continue;

                MqttAttributesTopic.Snapshot snapshot = value.Capture();
                MqttFlushStatus status = await Publish(
                    value.PublishTopic,
                    MqttValueSerializer.SerializeAttributes(snapshot.Values),
                    token);
                if (status != MqttFlushStatus.Completed)
                    return Result(status);
                value.MarkPublished(snapshot.Revision);
                attributes++;
            }

            if (discoveryDocuments + removedTopics + values + attributes > 0)
            {
                _logger.LogInformation(
                    "Published {Discovery} discovery documents, {Removed} removals, {Values} values and {Attributes} attribute changes",
                    discoveryDocuments, removedTopics, values, attributes);
            }

            return Result(MqttFlushStatus.Completed);
        }
        finally
        {
            _flushLock.Release();
        }

        MqttFlushResult Result(MqttFlushStatus status) => new(status, discoveryDocuments, removedTopics, values, attributes);
    }

    private async Task<MqttFlushStatus> Publish(string topic, byte[] payload, CancellationToken token)
    {
        try
        {
            MqttApplicationMessage message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithRetainFlag()
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            MqttClientPublishResult result = await _mqttClient.PublishAsync(message, token);
            if (!result.IsSuccess)
            {
                _logger.LogError("MQTT broker rejected publish to {Topic}: {ReasonCode} {ReasonString}", topic, result.ReasonCode, result.ReasonString);
                return MqttFlushStatus.BrokerRejected;
            }

            _logger.LogDebug("Published retained value to {Topic}", topic);
            return MqttFlushStatus.Completed;
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            throw;
        }
        catch (MqttClientNotConnectedException)
        {
            return MqttFlushStatus.Disconnected;
        }
        catch (MqttCommunicationException exception)
        {
            _logger.LogWarning(exception, "MQTT publish to {Topic} was interrupted; values remain dirty", topic);
            return MqttFlushStatus.Interrupted;
        }
    }

    internal void Register(HassMqttEntity entity)
    {
        lock (_entityRegistryLock)
        {
            if (_removeTopics.ContainsKey(entity.Discovery.Topic))
                throw new InvalidOperationException($"Entity {entity.DeviceId}/{entity.EntityId} has a pending removal");
            if (!_entities.TryAdd(entity.Discovery.Topic, entity))
                throw new InvalidOperationException($"An entity is already registered for {entity.DeviceId}/{entity.EntityId}");
        }
    }

    internal void LogValidationFailure(ValidationFailure failure)
    {
        if (failure.Severity == Severity.Warning)
            _logger.LogWarning("Discovery validation warning for {Property}: {Message}", failure.PropertyName, failure.ErrorMessage);
        else
            _logger.LogInformation("Discovery validation information for {Property}: {Message}", failure.PropertyName, failure.ErrorMessage);
    }

    internal T GetService<T>() => ServiceProvider.GetService(typeof(T)) is T service ? service : default;

    async Task IMqttEventReceiver.OnConnect(MqttClientConnectedEventArgs args, CancellationToken token)
    {
        MarkAllValuesDirty();
        await FlushAll(true, token);
    }

    Task IMqttEventReceiver.OnDisconnect(MqttClientDisconnectedEventArgs args, CancellationToken token) => Task.CompletedTask;
}
