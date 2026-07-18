using System;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.Interfaces;
using MBW.HassMQTT.Topics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Protocol;

namespace MBW.HassMQTT.CommonServices.AliveAndWill;

public static class HassConnectedEntityServiceExtensions
{
    public static IServiceCollection AddHassConnectedEntityService(this IServiceCollection services, string systemName, Action<HassConnectedEntityServiceConfig> configuration = null)
    {
        services
            .AddSingleton<AvailabilityDecoratorService>()
            .AddSingleton<HassConnectedEntityService>()
            .AddSingleton<IMqttEventReceiver>(x => x.GetRequiredService<HassConnectedEntityService>())
            .AddHostedService(x => x.GetRequiredService<HassConnectedEntityService>())
            .Configure<HassConnectedEntityServiceConfig>(x =>
            {
                x.DeviceId = systemName;
                x.DiscoveryDeviceName = systemName;
                x.DiscoveryEntityName = "Status";
            });

        if (configuration != null)
            services.PostConfigure(configuration);

        return services;
    }

    public static MqttClientOptionsBuilder ConfigureHassConnectedEntityServiceLastWill(this MqttClientOptionsBuilder builder, IServiceProvider provider)
    {
        HassMqttTopicBuilder topicBuilder = provider.GetRequiredService<HassMqttTopicBuilder>();
        HassConnectedEntityServiceConfig config = provider.GetRequiredService<IOptions<HassConnectedEntityServiceConfig>>().Value;

        return builder
            .WithWillTopic(topicBuilder.GetServiceTopic(config.DeviceId, config.EntityId, "state"))
            .WithWillPayload(config.ProblemMessage)
            .WithWillRetain()
            .WithWillQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce);
    }

    public static IEntityBuilder<TEntity> ConfigureAliveService<TEntity>(this IEntityBuilder<TEntity> builder) where TEntity : IHassDiscoveryDocument, IHasAvailability
    {
        AvailabilityDecoratorService decorator = builder.HassMqttManager.GetService<AvailabilityDecoratorService>();
        if (decorator == null)
            throw new InvalidOperationException($"Unable to locate the Alive services. Did you forget to configure services.{nameof(AddHassConnectedEntityService)}?");

        return builder.ConfigureDiscovery(discovery => decorator.ApplyAvailabilityInformation(discovery));
    }
}
