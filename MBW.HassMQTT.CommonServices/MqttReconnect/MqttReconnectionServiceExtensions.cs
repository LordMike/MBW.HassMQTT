using System;
using Microsoft.Extensions.DependencyInjection;

namespace MBW.HassMQTT.CommonServices.MqttReconnect
{
    public static class MqttReconnectionServiceExtensions
    {
        public static IServiceCollection AddMqttReconnectService(this IServiceCollection services, Action<MqttReconnectionServiceConfig> configuration = null)
        {
            services.AddHostedService<MqttReconnectionService>();

            if (configuration != null)
                services.PostConfigure(configuration);

            return services;
        }
    }
}