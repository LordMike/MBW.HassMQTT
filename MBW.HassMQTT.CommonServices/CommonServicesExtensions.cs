using System;
using MBW.HassMQTT.CommonServices.AliveAndWill;
using MBW.HassMQTT.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace MBW.HassMQTT.CommonServices;

public static class CommonServicesExtensions
{
    public static IServiceCollection AddAndConfigureMqtt(
        this IServiceCollection services,
        string applicationName,
        Action<HassMqttManagerConfiguration> hassMqttManagerConfiguration = null,
        Action<MqttClientOptionsBuilder> mqttOptionsBuilder = null)
    {
        services.AddMqttClientFactoryWithLogging();

        services.AddSingleton(provider =>
        {
            CommonMqttConfiguration config = provider.GetRequiredService<IOptions<CommonMqttConfiguration>>().Value;
            Validate(config);

            MqttClientOptionsBuilder builder = new MqttClientOptionsBuilder()
                .WithTcpServer(config.Server, config.Port)
                .WithCleanSession(false)
                .WithClientId(config.ClientId)
                .WithTimeout(config.ConnectTimeout);

            builder.ConfigureHassConnectedEntityServiceLastWill(provider);

            if (!string.IsNullOrEmpty(config.Username))
                builder.WithCredentials(config.Username, config.Password);

            if (config.KeepAlivePeriod.HasValue)
                builder.WithKeepAlivePeriod(config.KeepAlivePeriod.Value);

            if (config.EnableTls)
            {
                builder.WithTlsOptions(tls => tls
                    .WithSslProtocols(config.TlsProtocols)
                    .WithAllowUntrustedCertificates(config.TlsAllowUntrustedCertificates)
                    .WithIgnoreCertificateChainErrors(config.TlsIgnoreCertificateChainErrors)
                    .WithIgnoreCertificateRevocationErrors(config.TlsIgnoreCertificateRevocationErrors));
            }

            mqttOptionsBuilder?.Invoke(builder);

            return new MqttClientConnectionOptions
            {
                ClientOptions = builder.Build(),
                ConnectTimeout = config.ConnectTimeout,
                ReconnectInterval = config.ReconnectInterval,
                MaximumReconnectInterval = config.MaximumReconnectInterval,
                ReconnectWarningInterval = config.ReconnectWarningInterval
            };
        });

        services.AddSingleton<IMqttClient>(provider => provider.GetRequiredService<MqttClientFactory>().CreateMqttClient());
        services.AddMqttEvents();
        services.AddMqttMessageReceiverService();
        services.AddHassConnectedEntityService(applicationName);
        services.AddHassMqttManager(hassMqttManagerConfiguration);
        services.AddMqttLifetimeService();

        return services;
    }

    private static void Validate(CommonMqttConfiguration config)
    {
        if (string.IsNullOrWhiteSpace(config.Server))
            throw new InvalidOperationException("MQTT server must be configured.");
        if (config.Port is < 1 or > 65535)
            throw new InvalidOperationException("MQTT port must be between 1 and 65535.");
        if (string.IsNullOrWhiteSpace(config.ClientId) || config.ClientId == "HASSMQTT-unset")
            throw new InvalidOperationException("A unique MQTT client ID must be configured.");
        if (config.ConnectTimeout <= TimeSpan.Zero || config.ReconnectInterval <= TimeSpan.Zero)
            throw new InvalidOperationException("MQTT connection timeouts must be positive.");
        if (config.MaximumReconnectInterval < config.ReconnectInterval)
            throw new InvalidOperationException("MaximumReconnectInterval must be greater than or equal to ReconnectInterval.");
        if (config.ReconnectWarningInterval <= TimeSpan.Zero)
            throw new InvalidOperationException("ReconnectWarningInterval must be positive.");
    }
}
