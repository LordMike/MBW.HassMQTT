using System.Threading;
using System.Threading.Tasks;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;

namespace MBW.HassMQTT.Interfaces
{
    public interface IMqttEventReceiver
    {
        Task OnConnect(MqttClientConnectedEventArgs args, CancellationToken token);
        Task OnDisconnect(MqttClientDisconnectedEventArgs args, CancellationToken token);
    }
}