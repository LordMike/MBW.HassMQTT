using MBW.HassMQTT.Abstracts.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Helpers;
using Newtonsoft.Json.Linq;

namespace MBW.HassMQTT.DiscoveryModels
{
    /// <summary>
    /// All MQTT discovery types are documented here:
    /// https://www.home-assistant.io/docs/mqtt/discovery/
    /// </summary>
    public abstract class MqttSensorDiscoveryBase : IMqttValueContainer
    {
        private readonly JObject _discover;

        public bool Dirty { get; private set; }

        public string PublishTopic { get; }

        public MqttDeviceDocument Device { get; }

        public string UniqueId
        {
            get => _discover.GetOrDefault<string>("unique_id", null);
            set => _discover.SetIfChanged("unique_id", value, SetDirty);
        }

        public MqttSensorDiscoveryBase(string topic, string uniqueId)
        {
            PublishTopic = topic;
            _discover = new JObject();

            JObject deviceDoc = new JObject();
            Device = new MqttDeviceDocument(deviceDoc, SetDirty);

            _discover["device"] = deviceDoc;

            UniqueId = uniqueId;
        }

        public object GetSerializedValue(bool resetDirty)
        {
            if (resetDirty)
                Dirty = false;

            return _discover;
        }

        protected void SetValue<T>(string name, T value)
        {
            _discover.SetIfChanged(name, value, SetDirty);
        }

        protected T GetValue<T>(string name, T @default)
        {
            return _discover.GetOrDefault(name, @default);
        }

        private void SetDirty()
        {
            // To avoid 200x lambdas
            Dirty = true;
        }
    }
}