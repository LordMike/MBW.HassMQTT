using System;
using System.Reflection;
using MBW.HassMQTT.Abstracts.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Enum;
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
            set => _discover.SetIfChanged("unique_id", value, () => SetDirty());
        }

        public MqttSensorDiscoveryBase(string discoveryTopic, string uniqueId)
        {
            PublishTopic = discoveryTopic;
            _discover = new JObject();

            JObject deviceDoc = new JObject();
            Device = new MqttDeviceDocument(deviceDoc, () => SetDirty());

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
            _discover.SetIfChanged(name, value, () => SetDirty());
        }

        protected T GetValue<T>(string name, T @default)
        {
            return _discover.GetOrDefault(name, @default);
        }

        public void SetDirty(bool dirty = true)
        {
            Dirty = dirty;
        }

        public void SetTopic(HassTopicKind topicKind, string topic)
        {
            Type type = GetType();
            PropertyInfo prop = type.GetProperty($"{topicKind}Topic");

            if (prop == null)
                throw new NotSupportedException($"Unable to set topic {topicKind} on {type.Name}");

            prop.SetValue(this, topic);
        }

        public string GetTopic(HassTopicKind topicKind)
        {
            Type type = GetType();
            PropertyInfo prop = type.GetProperty($"{topicKind}Topic");

            if (prop == null)
                throw new NotSupportedException($"Unable to get the topic {topicKind} on {type.Name}");

            return prop.GetValue(this) as string;
        }
    }
}