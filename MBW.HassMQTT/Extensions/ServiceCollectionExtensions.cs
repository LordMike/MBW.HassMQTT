using System;
using MBW.HassMQTT.Interfaces;
using MBW.HassMQTT.Logging;
using MBW.HassMQTT.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet;

namespace MBW.HassMQTT.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMqttClientFactoryWithLogging(this IServiceCollection services, Action<MqttClientFactory> configure = null, string loggingSource = "MqttNet")
    {
        return services.AddSingleton(provider =>
        {
            ILoggerFactory loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            ExtensionsLoggingMqttLogger logger = new ExtensionsLoggingMqttLogger(loggerFactory, loggingSource);
            MqttClientFactory factory = new MqttClientFactory(logger);
            configure?.Invoke(factory);
            return factory;
        });
    }

    public static IServiceCollection AddMqttLifetimeService(this IServiceCollection services)
    {
        services.AddSingleton<MqttClientLifetimeService>();
        services.AddSingleton<IHassMqttClient>(provider => provider.GetRequiredService<MqttClientLifetimeService>());
        services.AddHostedService(provider => provider.GetRequiredService<MqttClientLifetimeService>());
        return services;
    }

    public static IServiceCollection AddMqttEvents(this IServiceCollection services) => services.AddSingleton<MqttEvents>();

    public static IServiceCollection AddMqttMessageReceiverService(this IServiceCollection services) => services.AddSingleton<MqttMessageDistributor>();

    public static IServiceCollection AddMqttMessageReceiver<TReceiver>(this IServiceCollection services, Func<IServiceProvider, TReceiver> factory = null) where TReceiver : class, IMqttMessageReceiver
    {
        if (factory != null)
            return services.AddSingleton<IMqttMessageReceiver>(factory);

        return services.AddSingleton<IMqttMessageReceiver>(provider => provider.GetRequiredService<TReceiver>());
    }

    public static IServiceCollection AddHassMqttManager(this IServiceCollection services, Action<HassMqttManagerConfiguration> configure = null)
    {
        services.AddSingleton<HassMqttManager>();
        services.AddSingleton<IMqttEventReceiver>(provider => provider.GetRequiredService<HassMqttManager>());

        if (configure != null)
            services.Configure(configure);

        return services;
    }
}
