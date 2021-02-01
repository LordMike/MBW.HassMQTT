using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MQTTnet.Extensions.ManagedClient;

namespace MBW.HassMQTT.CommonServices
{
    internal class MqttClientLifetimeService : IHostedService
    {
        private readonly IManagedMqttClient _client;
        private readonly IManagedMqttClientOptions _options;

        public MqttClientLifetimeService(IManagedMqttClient client, IManagedMqttClientOptions options)
        {
            _client = client;
            _options = options;
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