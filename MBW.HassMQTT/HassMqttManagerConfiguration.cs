namespace MBW.HassMQTT
{
    public class HassMqttManagerConfiguration
    {
        public bool AutoConfigureAttributesTopics { get; set; } = false;

        public bool SendDiscoveryDocuments { get; set; } = true;
    }
}