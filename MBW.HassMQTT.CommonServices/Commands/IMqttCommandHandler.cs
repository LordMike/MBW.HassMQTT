using System.Threading;
using System.Threading.Tasks;
using MQTTnet;

namespace MBW.HassMQTT.CommonServices.Commands
{
    public interface IMqttCommandHandler
    {
        string[] GetFilter();

        Task Handle(string[] topicLevels, MqttApplicationMessage message, CancellationToken token = default);
    }
}