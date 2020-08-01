using System;
using System.Text;
using MBW.HassMQTT.Topics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client.Options;

namespace MBW.HassMQTT.CommonServices.AliveAndWill
{
    public static class HassConnectedEntityServiceExtensions
    {
        public static IServiceCollection AddHassConnectedEntityServiceExtensions(this IServiceCollection services, Action<HassConnectedEntityServiceConfig> configuration = null)
        {
            services
                .AddSingleton<AvailabilityDecoratorService>()
                .AddSingleton<HassConnectedEntityService>()
                .AddHostedService(x => x.GetRequiredService<HassConnectedEntityService>());

            if (configuration != null)
                services.PostConfigure(configuration);

            return services;
        }

        public static MqttClientOptionsBuilder ConfigureHassConnectedEntityServiceLastWill(this MqttClientOptionsBuilder builder, IServiceProvider provider)
        {
            HassMqttTopicBuilder topicBuilder = provider.GetRequiredService<HassMqttTopicBuilder>();
            HassConnectedEntityServiceConfig config = provider.GetRequiredService<IOptions<HassConnectedEntityServiceConfig>>().Value;

            return builder.WithWillMessage(new MqttApplicationMessage
            {
                Topic = topicBuilder.GetServiceTopic(config.DeviceId, config.EntityId),
                Payload = Encoding.UTF8.GetBytes(config.ProblemMessage),
                Retain = true
            });
        }
    }
}