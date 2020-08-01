using System.Threading;
using System.Threading.Tasks;
using MQTTnet;

namespace MBW.HassMQTT.Services
{
    public interface IMqttMessageReceiver
    {
        Task ReceiveAsync(MqttApplicationMessage argApplicationMessage, CancellationToken token = default);
    }
}