using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MBW.HassMQTT.Interfaces;
using MBW.HassMQTT.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MQTTnet.Extensions.ManagedClient;

namespace MBW.HassMQTT
{
    internal class MqttClientLifetimeService : IHostedService
    {
        private readonly IManagedMqttClient _client;
        private readonly IManagedMqttClientOptions _options;

        public MqttClientLifetimeService(IManagedMqttClient client, IManagedMqttClientOptions options, IServiceProvider serviceProvider)
        {
            _client = client;
            _options = options;

            // Initialize initial receives
            MqttEvents mqttEvents = serviceProvider.GetService<MqttEvents>();

            if (mqttEvents != null)
            {
                IEnumerable<IMqttEventReceiver> receivers = serviceProvider.GetServices<IMqttEventReceiver>();

                foreach (IMqttEventReceiver receiver in receivers)
                {
                    mqttEvents.OnConnect += receiver.OnConnect;
                    mqttEvents.OnDisconnect += receiver.OnDisconnect;
                }
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _client.StartAsync(_options);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.StopAsync();
        }
    }
}