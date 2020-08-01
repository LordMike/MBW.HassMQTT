using System;
using Microsoft.Extensions.DependencyInjection;

namespace MBW.HassMQTT.CommonServices.Commands
{
    public static class MqttCommandServiceExtensions
    {
        public static IServiceCollection AddMqttCommandService(this IServiceCollection services)
        {
            return services
                .AddSingleton<MqttCommandService>()
                .AddHostedService(x => x.GetRequiredService<MqttCommandService>())
                .AddMqttMessageReceiver(x => x.GetRequiredService<MqttCommandService>());
        }

        public static IServiceCollection AddMqttCommandHandler<THandler>(this IServiceCollection services, Func<IServiceProvider, THandler> factory = null) where THandler : class, IMqttCommandHandler
        {
            if (factory != null)
                return services.AddSingleton<IMqttCommandHandler>(factory);

            return services.AddSingleton<IMqttCommandHandler, THandler>();
        }
    }
}