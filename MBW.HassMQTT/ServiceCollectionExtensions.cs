using System;
using MBW.HassMQTT.Logging;
using MBW.HassMQTT.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet;

namespace MBW.HassMQTT
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMqttClientFactoryWithLogging(this IServiceCollection services, Action<IMqttFactory> configure = null, string loggingSource = "MqttNet")
        {
            return services.AddSingleton<IMqttFactory>(provider =>
            {
                ILoggerFactory loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                ExtensionsLoggingMqttLogger logger = new ExtensionsLoggingMqttLogger(loggerFactory, loggingSource);

                MqttFactory factory = new MqttFactory(logger);

                configure?.Invoke(factory);

                return factory;
            });
        }

        public static IServiceCollection AddMqttEvents(this IServiceCollection services)
        {
            return services.AddSingleton<MqttEvents>();
        }

        public static IServiceCollection AddMqttMessageReceiverService(this IServiceCollection services)
        {
            return services.AddSingleton<MqttMessageDistributor>()
                   .AddHostedService(x => x.GetRequiredService<MqttMessageDistributor>());
        }

        public static IServiceCollection AddMqttMessageReceiver<TReceiver>(this IServiceCollection services, Func<IServiceProvider, TReceiver> factory = null) where TReceiver : class, IMqttMessageReceiver
        {
            if (factory != null)
                return services.AddSingleton<IMqttMessageReceiver, TReceiver>();

            return services.AddSingleton<IMqttMessageReceiver>(x => x.GetRequiredService<TReceiver>());
        }
    }
}