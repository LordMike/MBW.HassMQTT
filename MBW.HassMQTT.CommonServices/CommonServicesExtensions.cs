using System;
using System.Threading;
using MBW.HassMQTT.CommonServices.AliveAndWill;
using MBW.HassMQTT.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Server;

namespace MBW.HassMQTT.CommonServices;

public static class CommonServicesExtensions
{
    public static IServiceCollection AddAndConfigureMqtt(this IServiceCollection services, string applicationName,
        Action<HassMqttManagerConfiguration> hassMqttManagerConfiguration = null,
        Action<ManagedMqttClientOptionsBuilder> mqttOptionsBuilder = null)
    {
        // MQTT setup
        // This client will be configured to use MqttEvents to distribute connect/disconnect events
        services
            .AddMqttClientFactoryWithLogging()
            .AddSingleton<IManagedMqttClientOptions>(provider =>
            {
                CommonMqttConfiguration mqttConfig = provider.GetRequiredService<IOptions<CommonMqttConfiguration>>().Value;

                // Prepare options
                ManagedMqttClientOptionsBuilder optionsBuilder = new ManagedMqttClientOptionsBuilder()
                    .WithClientOptions(builder =>
                    {
                        builder
                            .WithTcpServer(mqttConfig.Server, mqttConfig.Port)
                            .WithCleanSession(false)
                            .WithClientId(mqttConfig.ClientId);

                        // Configure HASS LWT for alive services
                        builder.ConfigureHassConnectedEntityServiceLastWill(provider);

                        if (!string.IsNullOrEmpty(mqttConfig.Username))
                            builder.WithCredentials(mqttConfig.Username, mqttConfig.Password);

                        if (mqttConfig.KeepAlivePeriod.HasValue)
                            builder.WithKeepAlivePeriod(mqttConfig.KeepAlivePeriod.Value);
                    });

                optionsBuilder.WithAutoReconnect()
                    .WithAutoReconnectDelay(mqttConfig.ReconnectInterval)
                    .WithMaxPendingMessages(10000)
                    .WithPendingMessagesOverflowStrategy(MqttPendingMessagesOverflowStrategy.DropNewMessage);

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
                IManagedMqttClient mqttClient = factory.CreateManagedMqttClient();

                // Hook up event handlers
                mqttClient.ConfigureMqttEvents(provider, stoppingtoken);

                return mqttClient;
            });

        // Add MQTT events manager to help us distribute connected/disconnected events
        services.AddMqttEvents();

        // Add MQTT events manager to help us distribute subscribed messages
        services.AddMqttMessageReceiverService();

        // Add HASS service that indicates if we're alive or not (uses LWT feature of MQTT)
        services.AddHassConnectedEntityService(applicationName);

        // Add HASS MQTT Manager
        services.AddHassMqttManager(hassMqttManagerConfiguration);

        // Add connecter
        services.AddMqttLifetimeService();

        return services;
    }
}