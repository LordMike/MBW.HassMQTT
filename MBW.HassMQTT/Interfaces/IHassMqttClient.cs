using System.Threading;
using System.Threading.Tasks;
using MQTTnet;

namespace MBW.HassMQTT.Interfaces;

public interface IHassMqttClient
{
    bool IsConnected { get; }

    Task<MqttClientPublishResult> PublishAsync(MqttApplicationMessage message, CancellationToken token = default);
    Task SubscribeAsync(MqttClientSubscribeOptions options, CancellationToken token = default);
    Task UnsubscribeAsync(MqttClientUnsubscribeOptions options, CancellationToken token = default);
}
