using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet;

namespace MBW.HassMQTT.Services;

internal sealed class MqttMessageDistributor
{
    private readonly ILogger<MqttMessageDistributor> _logger;
    private readonly IServiceProvider _serviceProvider;

    public MqttMessageDistributor(ILogger<MqttMessageDistributor> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task OnMessage(MqttApplicationMessage message, CancellationToken token)
    {
        _logger.LogTrace("Handling message on {Topic} with {Bytes} bytes", message.Topic, message.Payload.Length);

        foreach (IMqttMessageReceiver receiver in _serviceProvider.GetServices<IMqttMessageReceiver>())
        {
            try
            {
                await receiver.ReceiveAsync(message, token);
                _logger.LogTrace("Handled message on {Topic} with {Handler} handler", message.Topic, receiver.GetType().Name);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                return;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to handle message on {Topic} with handler {Handler}", message.Topic, receiver.GetType().Name);
            }
        }
    }
}
