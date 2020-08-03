namespace MBW.HassMQTT
{
    public class HassConfiguration
    {
        public string DiscoveryPrefix { get; set; } = "homeassistant";

        public string TopicPrefix { get; set; }
    }
}