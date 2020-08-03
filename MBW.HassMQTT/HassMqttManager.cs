using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MBW.HassMQTT.Abstracts.Interfaces;
using MBW.HassMQTT.DiscoveryModels;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.Extensions;
using MBW.HassMQTT.Helpers;
using MBW.HassMQTT.Interfaces;
using MBW.HassMQTT.Internal;
using MBW.HassMQTT.Topics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MBW.HassMQTT
{
    public class HassMqttManager
    {
        private readonly ManualResetEventSlim _lockObject = new ManualResetEventSlim(true);
        private readonly IServiceProvider _serviceProvider;
        private readonly HassMqttManagerConfiguration _config;
        private readonly IMqttClient _mqttClient;
        private readonly ILogger<HassMqttManager> _logger;
        private readonly Dictionary<string, IDiscoveryDocumentBuilder> _discoveryDocuments;
        private readonly Dictionary<string, MqttStateValueTopic> _values;
        private readonly Dictionary<string, MqttAttributesTopic> _attributes;

        internal HassMqttTopicBuilder TopicBuilder { get; }

        public HassMqttManager(IServiceProvider serviceProvider, IOptions<HassMqttManagerConfiguration> config, IMqttClient mqttClient, HassMqttTopicBuilder topicBuilder, ILogger<HassMqttManager> logger)
        {
            _serviceProvider = serviceProvider;
            _config = config.Value;
            _mqttClient = mqttClient;
            TopicBuilder = topicBuilder;
            _logger = logger;
            _discoveryDocuments = new Dictionary<string, IDiscoveryDocumentBuilder>(StringComparer.OrdinalIgnoreCase);
            _values = new Dictionary<string, MqttStateValueTopic>(StringComparer.OrdinalIgnoreCase);
            _attributes = new Dictionary<string, MqttAttributesTopic>(StringComparer.OrdinalIgnoreCase);
        }

        public IDiscoveryDocumentBuilder<TEntity> ConfigureSensor<TEntity>(string deviceId, string entityId) where TEntity : MqttSensorDiscoveryBase
        {
            string uniqueId = $"{deviceId}_{entityId}";

            if (_discoveryDocuments.TryGetValue(uniqueId, out IDiscoveryDocumentBuilder builder))
                return (IDiscoveryDocumentBuilder<TEntity>)builder;

            string discoveryTopic = TopicBuilder.GetDiscoveryTopic<TEntity>(deviceId, entityId);

            builder = new DiscoveryDocumentBuilder<TEntity>(this)
            {
                Discovery = ActivatorUtilities.CreateInstance<TEntity>(_serviceProvider, discoveryTopic, uniqueId),
                DeviceId = deviceId,
                EntityId = entityId
            };

            _discoveryDocuments[uniqueId] = builder;

            if (_config.AutoConfigureAttributesTopics && builder.Discovery is IHasAttributesTopic)
                ((IDiscoveryDocumentBuilder<TEntity>)builder).ConfigureTopics(HassTopicKind.JsonAttributes);

            return (IDiscoveryDocumentBuilder<TEntity>)builder;
        }

        public ISensorContainer GetSensor(string deviceId, string entityId)
        {
            string uniqueId = $"{deviceId}_{entityId}";

            if (_discoveryDocuments.TryGetValue(uniqueId, out IDiscoveryDocumentBuilder builder))
                return builder as ISensorContainer;

            throw new InvalidOperationException($"Unable to find sensor {deviceId}/{entityId} - is it configured?");
        }

        public MqttAttributesTopic GetAttributesSender(string topic)
        {
            if (string.IsNullOrWhiteSpace(topic))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(topic));

            if (_attributes.TryGetValue(topic, out MqttAttributesTopic sensor))
                return sensor;

            return _attributes[topic] = new MqttAttributesTopic(topic);
        }

        public MqttStateValueTopic GetValueSender(string topic)
        {
            if (string.IsNullOrWhiteSpace(topic))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(topic));

            if (_values.TryGetValue(topic, out MqttStateValueTopic sensor))
                return sensor;

            return _values[topic] = new MqttStateValueTopic(topic);
        }

        [Obsolete]
        public MqttAttributesTopic GetAttributesValue(string deviceId, string entityId)
        {
            string topic = TopicBuilder.GetAttributesTopic(deviceId, entityId);

            if (_attributes.TryGetValue(topic, out MqttAttributesTopic sensor))
                return sensor;

            return _attributes[topic] = new MqttAttributesTopic(topic);
        }
        
        [Obsolete]
        public MqttStateValueTopic GetEntityStateValue(string deviceId, string entityId, string kind)
        {
            string topic = TopicBuilder.GetEntityTopic(deviceId, entityId, kind);

            if (_values.TryGetValue(topic, out MqttStateValueTopic sensor))
                return sensor;

            return _values[topic] = new MqttStateValueTopic(topic);
        }

        private async Task SendValue(IMqttValueContainer container, bool resetDirty, CancellationToken token)
        {
            object value = container.GetSerializedValue(resetDirty);
            bool log = _logger.IsEnabled(LogLevel.Debug);

            object sentValue = default;
            if (value is string str)
            {
                sentValue = str;
                await _mqttClient.SendValueAsync(container.PublishTopic, str, token);
            }
            else
            {
                JToken converted = JToken.FromObject(value);
                await _mqttClient.SendJsonAsync(container.PublishTopic, converted, token);

                if (log)
                    sentValue = converted.ToString(Formatting.None);
            }

            if (log)
                _logger.LogDebug("Pushed {Value} to {Topic}", sentValue, container.PublishTopic);
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

                foreach (IDiscoveryDocumentBuilder value in _discoveryDocuments.Values.Where(s => s.Discovery.Dirty))
                {
                    await SendValue(value.Discovery, true, token);
                    discoveryDocs++;
                }

                foreach (MqttStateValueTopic value in _values.Values.Where(s => s.Dirty))
                {
                    await SendValue(value, true, token);
                    values++;
                }

                foreach (MqttAttributesTopic value in _attributes.Values.Where(s => s.Dirty))
                {
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
    }
}