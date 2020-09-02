using System;
using System.Threading;
using MBW.HassMQTT.CommonServices.AliveAndWill;
using MBW.HassMQTT.CommonServices.MqttReconnect;
using MBW.HassMQTT.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace MBW.HassMQTT.CommonServices
{
    public static class CommonServicesExtensions
    {
        public static IServiceCollection AddAndConfigureMqtt(this IServiceCollection services, string applicationName,
            Action<HassMqttManagerConfiguration> hassMqttManagerConfiguration = null,
            Action<MqttClientOptionsBuilder> mqttOptionsBuilder = null)
        {
            // MQTT setup
            // This client will be configured to use MqttEvents to distribute connect/disconnect events
            services
                .AddMqttClientFactoryWithLogging()
                .AddSingleton(provider =>
                {
                    CommonMqttConfiguration mqttConfig = provider.GetRequiredService<IOptions<CommonMqttConfiguration>>().Value;

                    // Prepare options
                    MqttClientOptionsBuilder optionsBuilder = new MqttClientOptionsBuilder()
                        .WithTcpServer(mqttConfig.Server, mqttConfig.Port)
                        .WithCleanSession(false)
                        .WithClientId(mqttConfig.ClientId);

                    // Configure HASS LWT for alive services
                    optionsBuilder.ConfigureHassConnectedEntityServiceLastWill(provider);

                    if (!string.IsNullOrEmpty(mqttConfig.Username))
                        optionsBuilder.WithCredentials(mqttConfig.Username, mqttConfig.Password);

                    if (mqttConfig.KeepAlivePeriod.HasValue)
                        optionsBuilder.WithKeepAlivePeriod(mqttConfig.KeepAlivePeriod.Value);

                    // Additional config
                    mqttOptionsBuilder?.Invoke(optionsBuilder);

                    return optionsBuilder.Build();
                })
                .AddSingleton(provider =>
                {
                    // TODO: Support TLS & client certs
                    IHostApplicationLifetime appLifetime = provider.GetRequiredService<IHostApplicationLifetime>();
                    CancellationToken stoppingtoken = appLifetime.ApplicationStopping;

                    IMqttFactory factory = provider.GetRequiredService<IMqttFactory>();
                    IMqttClient mqttClient = factory.CreateMqttClient();

                    // Hook up event handlers
                    mqttClient.ConfigureMqttEvents(provider, stoppingtoken);

                    return mqttClient;
                });

            // Add MQTT events manager to help us distribute connected/disconnected events
            services.AddMqttEvents();

            // Add MQTT events manager to help us distribute subscribed messages
            services.AddMqttMessageReceiverService();

            // Add MQTT connection service that will connect to MQTT when we start, and reconnect when we need to
            services.AddMqttConnectionService();

            // Add HASS service that indicates if we're alive or not (uses LWT feature of MQTT)
            services.AddHassConnectedEntityService(applicationName);

            // Add HASS MQTT Manager
            services.AddHassMqttManager(hassMqttManagerConfiguration);

            return services;
        }
    }
}