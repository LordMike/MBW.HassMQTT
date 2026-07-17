using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MBW.HassMQTT.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet;

namespace MBW.HassMQTT.Services;

public class MqttEvents
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MqttEvents> _logger;

    public event Func<MqttClientConnectedEventArgs, CancellationToken, Task> OnConnect;
    public event Func<MqttClientDisconnectedEventArgs, CancellationToken, Task> OnDisconnect;
    public event Func<CancellationToken, Task> OnStopping;

    public MqttEvents(IServiceProvider serviceProvider, ILogger<MqttEvents> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task InvokeConnectHandler(MqttClientConnectedEventArgs args, CancellationToken token = default)
    {
        foreach (IMqttEventReceiver receiver in _serviceProvider.GetServices<IMqttEventReceiver>())
            await Invoke(() => receiver.OnConnect(args, token), receiver.GetType());

        if (OnConnect != null)
        {
            foreach (Func<MqttClientConnectedEventArgs, CancellationToken, Task> handler in OnConnect.GetInvocationList())
                await Invoke(() => handler(args, token), handler.Target?.GetType());
        }
    }

    public async Task InvokeDisconnectHandler(MqttClientDisconnectedEventArgs args, CancellationToken token = default)
    {
        foreach (IMqttEventReceiver receiver in _serviceProvider.GetServices<IMqttEventReceiver>())
            await Invoke(() => receiver.OnDisconnect(args, token), receiver.GetType());

        if (OnDisconnect != null)
        {
            foreach (Func<MqttClientDisconnectedEventArgs, CancellationToken, Task> handler in OnDisconnect.GetInvocationList())
                await Invoke(() => handler(args, token), handler.Target?.GetType());
        }
    }

    public async Task InvokeStoppingHandler(CancellationToken token = default)
    {
        foreach (IMqttEventReceiver receiver in _serviceProvider.GetServices<IMqttEventReceiver>())
            await Invoke(() => receiver.OnStopping(token), receiver.GetType());

        if (OnStopping != null)
        {
            foreach (Func<CancellationToken, Task> handler in OnStopping.GetInvocationList())
                await Invoke(() => handler(token), handler.Target?.GetType());
        }
    }

    private async Task Invoke(Func<Task> action, Type receiverType)
    {
        try
        {
            await action();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "MQTT lifecycle receiver {Receiver} failed", receiverType?.FullName ?? "unknown");
        }
    }
}
