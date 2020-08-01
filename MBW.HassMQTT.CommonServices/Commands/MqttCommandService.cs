using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MBW.HassMQTT.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;

namespace MBW.HassMQTT.CommonServices.Commands
{
    internal class MqttCommandService : IHostedService, IMqttMessageReceiver
    {
        private readonly ILogger<MqttCommandService> _logger;
        private readonly IMqttClient _mqttClient;
        private readonly string _topicPrefix;

        private readonly List<(string[] filter, IMqttCommandHandler handler)> _handlers = new List<(string[] filter, IMqttCommandHandler handler)>();

        public MqttCommandService(
            ILogger<MqttCommandService> logger,
            IOptions<HassConfiguration> hassConfig,
            IMqttClient mqttClient,
            IEnumerable<IMqttCommandHandler> handlers)
        {
            _logger = logger;
            _mqttClient = mqttClient;
            _topicPrefix = hassConfig.Value.TopicPrefix.TrimEnd('/');

            foreach (IMqttCommandHandler handler in handlers)
                _handlers.Add((handler.GetFilter(), handler));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Listen to topics we want to handle
            // Take the filters of each command, and built a topic filter from it
            // Null segments mean "+" (placeholder for one segment)

            foreach ((string[] filter, IMqttCommandHandler _) in _handlers)
            {
                string subscription = $"{_topicPrefix}/{string.Join("/", filter.Select(x => x ?? "+"))}";

                _logger.LogDebug("Subscribing to {filter}", subscription);

                await _mqttClient.SubscribeAsync(subscription);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task ReceiveAsync(MqttApplicationMessage argApplicationMessage, CancellationToken token = default)
        {
            // Skip prefix, split topic
            string[] topicLevels = argApplicationMessage.Topic.Substring(_topicPrefix.Length + 1).Split('/');

            _logger.LogDebug("Received {Topic}, value {Value}", argApplicationMessage.Topic, argApplicationMessage.ConvertPayloadToString());

            foreach ((string[] filter, IMqttCommandHandler handler) in _handlers)
            {
                if (filter.Length != topicLevels.Length)
                    continue;

                bool wasMatch = true;
                for (int i = 0; i < filter.Length; i++)
                {
                    if (filter[i] == null || filter[i] == topicLevels[i])
                        continue;

                    wasMatch = false;
                    break;
                }

                if (!wasMatch)
                    continue;

                _logger.LogDebug("Received {Topic} matches {Handler}", argApplicationMessage.Topic, handler.ToString());

                try
                {
                    await handler.Handle(topicLevels, argApplicationMessage, token);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Unable to handle message from {topic} using {handler}", argApplicationMessage.Topic, handler.GetType().FullName);
                }
            }
        }
    }
}