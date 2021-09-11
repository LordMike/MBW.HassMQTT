using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MBW.HassMQTT.Abstracts.Interfaces;
using MBW.HassMQTT.DiscoveryModels;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.Extensions;
using MBW.HassMQTT.Helpers;
using MBW.HassMQTT.Interfaces;
using MBW.HassMQTT.Internal;
using MBW.HassMQTT.Serialization;
using MBW.HassMQTT.Topics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet.Extensions.ManagedClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MBW.HassMQTT
{
    public class HassMqttManager
    {
        private readonly ManualResetEventSlim _lockObject = new ManualResetEventSlim(true);
        private readonly IServiceProvider _serviceProvider;
        private readonly HassMqttManagerConfiguration _config;
        private readonly IManagedMqttClient _mqttClient;
        private readonly ILogger<HassMqttManager> _logger;
        private readonly ConcurrentDictionary<string, IDiscoveryDocumentBuilder> _discoveryDocuments;
        private readonly ConcurrentDictionary<string, MqttStateValueTopic> _values;
        private readonly ConcurrentDictionary<string, MqttAttributesTopic> _attributes;

        internal HassMqttTopicBuilder TopicBuilder { get; }

        public HassMqttManager(IServiceProvider serviceProvider, IOptions<HassMqttManagerConfiguration> config, IManagedMqttClient mqttClient, HassMqttTopicBuilder topicBuilder, ILogger<HassMqttManager> logger)
        {
            _serviceProvider = serviceProvider;
            _config = config.Value;
            _mqttClient = mqttClient;
            TopicBuilder = topicBuilder;
            _logger = logger;
            _discoveryDocuments = new ConcurrentDictionary<string, IDiscoveryDocumentBuilder>(StringComparer.OrdinalIgnoreCase);
            _values = new ConcurrentDictionary<string, MqttStateValueTopic>(StringComparer.OrdinalIgnoreCase);
            _attributes = new ConcurrentDictionary<string, MqttAttributesTopic>(StringComparer.OrdinalIgnoreCase);
        }

        public IDiscoveryDocumentBuilder<TEntity> ConfigureSensor<TEntity>(string deviceId, string entityId, string uniqueId = null) where TEntity : IHassDiscoveryDocument
        {
            uniqueId ??= $"{deviceId}_{entityId}".ToLower();

            IDiscoveryDocumentBuilder builder = _discoveryDocuments.GetOrAdd(uniqueId, s =>
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
            string uniqueId = $"{deviceId}_{entityId}";

            _discoveryDocuments.TryGetValue(uniqueId, out IDiscoveryDocumentBuilder builder);
            sensor = builder as ISensorContainer;

            return sensor != null;
        }

        public MqttAttributesTopic GetAttributesSender(string topic)
        {
            if (string.IsNullOrWhiteSpace(topic))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(topic));

            return _attributes.GetOrAdd(topic, s => new MqttAttributesTopic(topic));
        }

        public MqttStateValueTopic GetValueSender(string topic)
        {
            if (string.IsNullOrWhiteSpace(topic))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(topic));

            return _values.GetOrAdd(topic, s => new MqttStateValueTopic(topic));
        }

        private async Task SendValue(IMqttValueContainer container, bool resetDirty, CancellationToken token)
        {
            object value = container.GetSerializedValue(false);
            bool log = _logger.IsEnabled(LogLevel.Debug);

            object sentValue;
            if (value is string str)
            {
                sentValue = str;
                await _mqttClient.SendValueAsync(container.PublishTopic, str, token);
            }
            else if (value == null)
            {
                sentValue = "<null>";
                await _mqttClient.SendValueAsync(container.PublishTopic, string.Empty, token);
            }
            else
            {
                sentValue = null;

                JToken converted = JToken.FromObject(value, CustomJsonSerializer.Serializer);
                await _mqttClient.SendJsonAsync(container.PublishTopic, converted, token);

                if (log)
                    sentValue = converted.ToString(Formatting.None);
            }

            // Set to not-dirty _after_ sending the value
            if (resetDirty)
                container.SetDirty(false);

            if (log)
                _logger.LogDebug("Pushed {Value} to {Topic}", sentValue, container.PublishTopic);
        }

        public void MarkAllValuesDirty()
        {
            foreach (MqttStateValueTopic value in _values.Values)
                value.SetDirty();

            foreach (MqttAttributesTopic value in _attributes.Values)
                value.SetDirty();
        }

        public async Task FlushAll(CancellationToken token = default)
        {
            if (!_lockObject.Wait(0))
            {
                _logger.LogDebug("Unable to acquire exclusive flushing lock");
                return;
            }

            try
            {
                int discoveryDocs = 0, values = 0, attributes = 0;

                foreach (IDiscoveryDocumentBuilder value in _discoveryDocuments.Values.Where(s => s.DiscoveryUntyped.Dirty))
                {
                    string uniqueId = (value.DiscoveryUntyped as IHasUniqueId)?.UniqueId ?? value.ToString();

                    if (_config.SendDiscoveryDocuments)
                    {
                        if (_config.ValidateDiscoveryDocuments)
                        {
                            ValidationContext<IHassDiscoveryDocument> validationContext = new ValidationContext<IHassDiscoveryDocument>(value.DiscoveryUntyped);
                            ValidationResult validation = await value.DiscoveryUntyped.Validator.ValidateAsync(validationContext, token);

                            if (!validation.IsValid)
                                throw new ValidationException(validation.Errors);
                        }

                        _logger.LogDebug("Sending discovery document for {identifier}", uniqueId);

                        await SendValue(value.DiscoveryUntyped, true, token);
                        discoveryDocs++;
                    }
                    else
                    {
                        _logger.LogDebug("Not sending discovery for {identifier}, due to configuration", uniqueId);
                        value.DiscoveryUntyped.SetDirty(false);
                    }
                }

                foreach (MqttStateValueTopic value in _values.Values.Where(s => s.Dirty))
                {
                    _logger.LogDebug("Sending state value for {topic}", value.PublishTopic);

                    await SendValue(value, true, token);
                    values++;
                }

                foreach (MqttAttributesTopic value in _attributes.Values.Where(s => s.Dirty))
                {
                    _logger.LogDebug("Sending attributes for {topic}", value.PublishTopic);

                    await SendValue(value, true, token);
                    attributes++;
                }

                if (discoveryDocs > 0 || values > 0 || attributes > 0)
                    _logger.LogInformation("Pushed {discovery} discovery documents, {values} values and {attributes} attribute changes", discoveryDocs, values, attributes);
            }
            finally
            {
                _lockObject.Set();
            }
        }

        internal T GetService<T>()
        {
            return _serviceProvider.GetService<T>();
        }
    }
}