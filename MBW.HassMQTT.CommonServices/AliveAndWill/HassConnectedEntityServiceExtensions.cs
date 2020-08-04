using System;
using System.Text;
using MBW.HassMQTT.DiscoveryModels;
using MBW.HassMQTT.Extensions;
using MBW.HassMQTT.Interfaces;
using MBW.HassMQTT.Topics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client.Options;

namespace MBW.HassMQTT.CommonServices.AliveAndWill
{
    public static class HassConnectedEntityServiceExtensions
    {
        public static IServiceCollection AddHassConnectedEntityService(this IServiceCollection services, string systemName, Action<HassConnectedEntityServiceConfig> configuration = null)
        {
            services
                .AddSingleton<AvailabilityDecoratorService>()
                .AddSingleton<HassConnectedEntityService>()
                .AddHostedService(x => x.GetRequiredService<HassConnectedEntityService>())
                .Configure<HassConnectedEntityServiceConfig>(x =>
                {
                    x.DeviceId = systemName;
                    x.DiscoveryDeviceName = systemName;
                    x.DiscoveryEntityName = $"{systemName} Status";
                });

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
                Topic = topicBuilder.GetServiceTopic(config.DeviceId, config.EntityId, "state"),
                Payload = Encoding.UTF8.GetBytes(config.ProblemMessage),
                Retain = true
            });
        }

        public static IDiscoveryDocumentBuilder<TEntity> ConfigureAliveService<TEntity>(this IDiscoveryDocumentBuilder<TEntity> builder) where TEntity : MqttEntitySensorDiscoveryBase
        {
            AvailabilityDecoratorService decorator = builder.HassMqttManager.GetService<AvailabilityDecoratorService>();

            if (decorator == null)
                throw new InvalidOperationException($"Unable to locate the Alive services. Did you forget to configure services.{nameof(AddHassConnectedEntityService)}?");

            return builder.ConfigureDiscovery(decorator.ApplyAvailabilityInformation);
        }
    }
}