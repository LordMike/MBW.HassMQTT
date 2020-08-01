using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MBW.HassMQTT.Abstracts.Interfaces;
using MBW.HassMQTT.DiscoveryModels;
using MBW.HassMQTT.Helpers;
using MBW.HassMQTT.Topics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MBW.HassMQTT
{
    public class HassMqttManager
    {
        private readonly ManualResetEventSlim _lockObject = new ManualResetEventSlim(true);
        private readonly IServiceProvider _serviceProvider;
        private readonly IMqttClient _mqttClient;
        private readonly HassMqttTopicBuilder _topicBuilder;
        private readonly ILogger<HassMqttManager> _logger;
        private readonly Dictionary<string, MqttSensorDiscoveryBase> _discoveryDocuments;
        private readonly Dictionary<string, MqttStateValueTopic> _values;
        private readonly Dictionary<string, MqttAttributesTopic> _attributes;

        public HassMqttManager(IServiceProvider serviceProvider, IMqttClient mqttClient, HassMqttTopicBuilder topicBuilder, ILogger<HassMqttManager> logger)
        {
            _serviceProvider = serviceProvider;
            _mqttClient = mqttClient;
            _topicBuilder = topicBuilder;
            _logger = logger;
            _discoveryDocuments = new Dictionary<string, MqttSensorDiscoveryBase>(StringComparer.OrdinalIgnoreCase);
            _values = new Dictionary<string, MqttStateValueTopic>(StringComparer.OrdinalIgnoreCase);
            _attributes = new Dictionary<string, MqttAttributesTopic>(StringComparer.OrdinalIgnoreCase);
        }

        public TComponent ConfigureDiscovery<TComponent>(string deviceId, string entityId) where TComponent : MqttSensorDiscoveryBase
        {
            string uniqueId = $"{deviceId}_{entityId}";

            if (!_discoveryDocuments.TryGetValue(uniqueId, out MqttSensorDiscoveryBase discoveryDoc))
            {
                string discoveryTopic = _topicBuilder.GetDiscoveryTopic<TComponent>(deviceId, entityId);

                discoveryDoc = ActivatorUtilities.CreateInstance<TComponent>(_serviceProvider, discoveryTopic, uniqueId);
                _discoveryDocuments[uniqueId] = discoveryDoc;
            }

            return (TComponent)discoveryDoc;
        }

        public MqttAttributesTopic GetAttributesValue(string deviceId, string entityId)
        {
            string topic = _topicBuilder.GetAttributesTopic(deviceId, entityId);

            if (_attributes.TryGetValue(topic, out MqttAttributesTopic sensor))
                return sensor;

            return _attributes[topic] = new MqttAttributesTopic(topic);
        }

        public MqttStateValueTopic GetServiceStateValue(string deviceId, string entityId)
        {
            string topic = _topicBuilder.GetServiceTopic(deviceId, entityId);

            if (_values.TryGetValue(topic, out MqttStateValueTopic sensor))
                return sensor;

            return _values[topic] = new MqttStateValueTopic(topic);
        }

        public MqttStateValueTopic GetEntityStateValue(string deviceId, string entityId, string kind)
        {
            string topic = _topicBuilder.GetEntityTopic(deviceId, entityId, kind);

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

                foreach (MqttSensorDiscoveryBase value in _discoveryDocuments.Values.Where(s => s.Dirty))
                {
                    await SendValue(value, true, token);
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