using System.Threading;
using System.Threading.Tasks;
using MQTTnet;

namespace MBW.HassMQTT.Interfaces;

public interface IMqttEventReceiver
{
    Task OnConnect(MqttClientConnectedEventArgs args, CancellationToken token);
    Task OnDisconnect(MqttClientDisconnectedEventArgs args, CancellationToken token);
    Task OnStopping(CancellationToken token) => Task.CompletedTask;
}
