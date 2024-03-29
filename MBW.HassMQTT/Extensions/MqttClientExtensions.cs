﻿using System;
using System.Threading;
using MBW.HassMQTT.Services;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet.Extensions.ManagedClient;

namespace MBW.HassMQTT.Extensions;

public static class MqttClientExtensions
{
    public static IManagedMqttClient ConfigureMqttEvents(this IManagedMqttClient mqttClient, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        MqttEvents mqttEvents = serviceProvider.GetService<MqttEvents>();

        if (mqttEvents == null)
            throw new InvalidOperationException($"Unable to find {nameof(MqttEvents)} service. Did you forget to call {nameof(ServiceCollectionExtensions.AddMqttEvents)}()?");

        mqttClient.UseDisconnectedHandler(async args =>
        {
            await mqttEvents.InvokeDisconnectHandler(args, cancellationToken);
        });
        mqttClient.UseConnectedHandler(async args =>
        {
            await mqttEvents.InvokeConnectHandler(args, cancellationToken);
        });

        return mqttClient;
    }

}