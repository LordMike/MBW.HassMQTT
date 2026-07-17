using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MBW.HassMQTT.Abstracts.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.Extensions;
using MBW.HassMQTT.Interfaces;
using MBW.HassMQTT.Internal;
using MBW.HassMQTT.Serialization;
using MBW.HassMQTT.Topics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Exceptions;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MBW.HassMQTT;

public class HassMqttManager : IMqttEventReceiver
{
    private readonly SemaphoreSlim _flushLock = new SemaphoreSlim(1, 1);
    private readonly IServiceProvider _serviceProvider;
    private readonly HassMqttManagerConfiguration _config;
    private readonly IHassMqttClient _mqttClient;
    private readonly ILogger<HassMqttManager> _logger;
    private readonly ConcurrentDictionary<string, IDiscoveryDocumentBuilder> _discoveryDocuments;
    private readonly ConcurrentDictionary<string, MqttStateValueTopic> _values;
    private readonly ConcurrentDictionary<string, MqttAttributesTopic> _attributes;
    private readonly ConcurrentDictionary<string, byte> _removeTopics;

    internal HassMqttTopicBuilder TopicBuilder { get; }

    public HassMqttManager(
        IServiceProvider serviceProvider,
        IOptions<HassMqttManagerConfiguration> config,
        IHassMqttClient mqttClient,
        HassMqttTopicBuilder topicBuilder,
        ILogger<HassMqttManager> logger)
    {
        _serviceProvider = serviceProvider;
        _config = config.Value;
        _mqttClient = mqttClient;
        TopicBuilder = topicBuilder;
        _logger = logger;
        _discoveryDocuments = new ConcurrentDictionary<string, IDiscoveryDocumentBuilder>(StringComparer.OrdinalIgnoreCase);
        _values = new ConcurrentDictionary<string, MqttStateValueTopic>(StringComparer.OrdinalIgnoreCase);
        _attributes = new ConcurrentDictionary<string, MqttAttributesTopic>(StringComparer.OrdinalIgnoreCase);
        _removeTopics = new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase);
    }

    public void RemoveSensor<TEntity>(string deviceId, string entityId) where TEntity : IHassDiscoveryDocument =>
        _removeTopics.TryAdd(TopicBuilder.GetDiscoveryTopic<TEntity>(deviceId, entityId), 0);

    public IDiscoveryDocumentBuilder<TEntity> ConfigureSensor<TEntity>(string deviceId, string entityId, string uniqueId = null) where TEntity : IHassDiscoveryDocument
    {
        uniqueId ??= $"{deviceId}_{entityId}".ToLower();

        IDiscoveryDocumentBuilder builder = _discoveryDocuments.GetOrAdd(uniqueId, _ =>
        {
            string discoveryTopic = TopicBuilder.GetDiscoveryTopic<TEntity>(deviceId, entityId);
            DiscoveryDocumentBuilder<TEntity> newBuilder = new DiscoveryDocumentBuilder<TEntity>(this)
            {
                Discovery = ActivatorUtilities.CreateInstance<TEntity>(_serviceProvider, discoveryTopic, uniqueId),
                DeviceId = deviceId,
                EntityId = entityId
            };

            if (_config.AutoConfigureAttributesTopics && newBuilder.Discovery is IHasJsonAttributes)
                newBuilder.ConfigureTopics(HassTopicKind.JsonAttributes);

            return newBuilder;
        });

        return (IDiscoveryDocumentBuilder<TEntity>)builder;
    }

    public bool TryGetSensor(string deviceId, string entityId, out ISensorContainer sensor)
    {
        _discoveryDocuments.TryGetValue($"{deviceId}_{entityId}", out IDiscoveryDocumentBuilder builder);
        sensor = builder as ISensorContainer;
        return sensor != null;
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
        foreach (IDiscoveryDocumentBuilder value in _discoveryDocuments.Values)
            value.DiscoveryUntyped.MarkDirty();
        foreach (MqttStateValueTopic value in _values.Values)
            value.MarkDirty();
        foreach (MqttAttributesTopic value in _attributes.Values)
            value.MarkDirty();
    }

    public async Task<MqttFlushResult> FlushAll(CancellationToken token = default)
    {
        if (!await _flushLock.WaitAsync(0, token))
            return new MqttFlushResult(MqttFlushStatus.Busy);

        int discoveryDocuments = 0, removedTopics = 0, values = 0, attributes = 0;
        try
        {
            if (!_mqttClient.IsConnected)
                return Result(MqttFlushStatus.Disconnected);

            foreach (IDiscoveryDocumentBuilder builder in _discoveryDocuments.Values.Where(value => value.DiscoveryUntyped.Dirty))
            {
                IHassDiscoveryDocument discovery = builder.DiscoveryUntyped;
                string uniqueId = (discovery as IHasUniqueId)?.UniqueId ?? builder.ToString();

                if (!_config.SendDiscoveryDocuments)
                {
                    discovery.MarkPublished(discovery.Revision);
                    continue;
                }

                if (_config.ValidateDiscoveryDocuments)
                {
                    ValidationResult validation = await discovery.Validator.ValidateAsync(new ValidationContext<IHassDiscoveryDocument>(discovery), token);
                    if (!validation.IsValid)
                        throw new ValidationException(validation.Errors);
                }

                _logger.LogDebug("Publishing discovery document for {Identifier}", uniqueId);
                MqttFlushStatus status = await Publish(discovery, token);
                if (status != MqttFlushStatus.Completed)
                    return Result(status);
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

            foreach (MqttStateValueTopic value in _values.Values.Where(value => value.Dirty))
            {
                MqttFlushStatus status = await Publish(value, token);
                if (status != MqttFlushStatus.Completed)
                    return Result(status);
                values++;
            }

            foreach (MqttAttributesTopic value in _attributes.Values.Where(value => value.Dirty))
            {
                MqttFlushStatus status = await Publish(value, token);
                if (status != MqttFlushStatus.Completed)
                    return Result(status);
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

    private async Task<MqttFlushStatus> Publish(IMqttValueContainer container, CancellationToken token)
    {
        long revision = container.Revision;
        byte[] payload = Serialize(container.GetSerializedValue());
        MqttFlushStatus status = await Publish(container.PublishTopic, payload, token);
        if (status == MqttFlushStatus.Completed)
            container.MarkPublished(revision);
        return status;
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

    private static byte[] Serialize(object value)
    {
        if (value is string text)
            return Encoding.UTF8.GetBytes(text);
        if (value == null)
            return Array.Empty<byte>();

        JToken converted = JToken.FromObject(value, CustomJsonSerializer.Serializer);
        return Encoding.UTF8.GetBytes(converted.ToString(Formatting.None));
    }

    async Task IMqttEventReceiver.OnConnect(MqttClientConnectedEventArgs args, CancellationToken token)
    {
        MarkAllValuesDirty();
        await FlushAll(token);
    }

    Task IMqttEventReceiver.OnDisconnect(MqttClientDisconnectedEventArgs args, CancellationToken token) => Task.CompletedTask;

    internal T GetService<T>() => _serviceProvider.GetService<T>();
}
