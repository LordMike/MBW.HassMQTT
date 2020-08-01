using System.Linq;
using System.Text;

namespace MBW.HassMQTT.Topics
{
    public class HassMqttTopicBuilder
    {
        private const char Seperator = '/';
        private readonly string _discoveryPrefix;
        private readonly string _servicePrefix;

        public HassMqttTopicBuilder(HassConfiguration hassOptions)
        {
            _discoveryPrefix = hassOptions.HomeassistantDiscoveryPrefix.TrimEnd('/');
            _servicePrefix = hassOptions.TopicPrefix.TrimEnd('/');
        }

        private string Combine(string prefix, string[] segments)
        {
            StringBuilder sb = new StringBuilder(prefix.Length + segments.Length + segments.Sum(s => s.Length));

            sb.Append(prefix);

            foreach (string segment in segments)
            {
                sb.Append(Seperator);
                sb.Append(segment);
            }

            return sb.ToString();
        }

        public string GetDiscoveryTopic(params string[] segments)
        {
            return Combine(_discoveryPrefix, segments);
        }

        public string GetServiceTopic(params string[] segments)
        {
            return Combine(_servicePrefix, segments);
        }
    }
}