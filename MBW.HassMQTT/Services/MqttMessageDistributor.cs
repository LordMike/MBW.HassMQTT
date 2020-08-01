using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;

namespace MBW.HassMQTT.Services
{
    internal class MqttMessageDistributor : IHostedService
    {
        private readonly ILogger<MqttMessageDistributor> _logger;
        private readonly IMqttClient _mqttClient;
        private readonly List<IMqttMessageReceiver> _receivers;

        public MqttMessageDistributor(ILogger<MqttMessageDistributor> logger, IMqttClient mqttClient, IEnumerable<IMqttMessageReceiver> receivers)
        {
            _logger = logger;
            _mqttClient = mqttClient;
            _receivers = receivers.ToList();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _mqttClient.UseApplicationMessageReceivedHandler(OnMessage);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task OnMessage(MqttApplicationMessageReceivedEventArgs arg)
        {
            _logger.LogTrace("Handling message on {topic} with {bytes} bytes", arg.ApplicationMessage.Topic, arg.ApplicationMessage.Payload.Length);

            foreach (IMqttMessageReceiver mqttMessageReceiver in _receivers)
            {
                try
                {
                    // TODO: Missing cancellation token, might stall app
                    await mqttMessageReceiver.ReceiveAsync(arg.ApplicationMessage, CancellationToken.None);

                    _logger.LogTrace("Handled message on {topic} with {handler} handler", arg.ApplicationMessage.Topic, mqttMessageReceiver.GetType().Name);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Unable to handle message on {topic} with handler {handler}", arg.ApplicationMessage.Topic, mqttMessageReceiver.GetType().Name);
                }
            }
        }
    }
}