using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MBW.HassMQTT.Interfaces;
using MBW.HassMQTT.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet.Client;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;

namespace MBW.HassMQTT.CommonServices.MqttReconnect
{
    internal class MqttConnectionService : BackgroundService
    {
        private readonly ILogger<MqttConnectionService> _logger;
        private readonly MqttEvents _mqttEvents;
        private readonly IMqttClient _mqttClient;
        private readonly IMqttClientOptions _mqttConnectOptions;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly List<IMqttEventReceiver> _initialReceivers;
        private readonly MqttReconnectionServiceConfig _mqttConfig;

        public MqttConnectionService(ILogger<MqttConnectionService> logger, MqttEvents mqttEvents, IMqttClient mqttClient, IMqttClientOptions mqttConnectOptions, IOptions<MqttReconnectionServiceConfig> mqttConfig, IHostApplicationLifetime lifetime, IEnumerable<IMqttEventReceiver> initialReceivers)
        {
            _logger = logger;
            _mqttEvents = mqttEvents;
            _mqttClient = mqttClient;
            _mqttConnectOptions = mqttConnectOptions;
            _mqttConfig = mqttConfig.Value;
            _lifetime = lifetime;
            _initialReceivers = initialReceivers.ToList();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            CancellationToken appStopToken = _lifetime.ApplicationStopping;

            _mqttEvents.OnDisconnect += async (args, token) =>
            {
                // Do not reconnect if we're done
                if (appStopToken.IsCancellationRequested)
                    return;

                _logger.LogWarning(args.Exception, "Server disconnected, attempting reconnect in {Time}", _mqttConfig.ReconnectInterval);

                await Task.Delay(_mqttConfig.ReconnectInterval, stoppingToken);

                try
                {
                    await _mqttClient.ConnectAsync(_mqttConnectOptions, stoppingToken);
                }
                catch (Exception exception)
                {
                    _logger.LogWarning(exception, "Reconnect failed");
                }
            };

            // Register disconnect when app closes
            appStopToken.Register(o => ((IMqttClient)o).DisconnectAsync(new MqttClientDisconnectOptions
            {
                ReasonCode = MqttClientDisconnectReason.NormalDisconnection,
                ReasonString = "Shutting down"
            }), _mqttClient);

            // Load initial receivers
            foreach (IMqttEventReceiver receiver in _initialReceivers)
            {
                _mqttEvents.OnConnect += receiver.OnConnect;
                _mqttEvents.OnDisconnect += receiver.OnDisconnect;
            }

            // Allow GC to remove receivers from memory if needed
            _initialReceivers.Clear();

            // Connect
            _mqttClient.ConnectAsync(_mqttConnectOptions, stoppingToken);


            return Task.CompletedTask;
        }
    }
}