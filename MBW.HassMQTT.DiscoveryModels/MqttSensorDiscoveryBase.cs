using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using MBW.HassMQTT.Abstracts.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Device;
using MBW.HassMQTT.DiscoveryModels.Enum;
using Newtonsoft.Json;

namespace MBW.HassMQTT.DiscoveryModels
{
    /// <summary>
    /// All MQTT discovery types are documented here:
    /// https://www.home-assistant.io/docs/mqtt/discovery/
    /// </summary>
    public abstract class MqttSensorDiscoveryBase : IMqttValueContainer, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (nameof(Dirty) != propertyName)
                Dirty = true;
        }

        /// <inheritdoc />
        [JsonIgnore]
        public bool Dirty { get; private set; }

        /// <inheritdoc />
        [JsonIgnore]
        public string PublishTopic { get; }

        [JsonProperty("device")]
        public MqttDeviceDocument Device { get; set; }

        [JsonProperty("unique_id")]
        public string UniqueId { get; set; }

        public MqttSensorDiscoveryBase(string discoveryTopic, string uniqueId)
        {
            PublishTopic = discoveryTopic;
            UniqueId = uniqueId;

            Device = new MqttDeviceDocument(this);
        }

        public void SetDirty(bool dirty = true)
        {
            Dirty = dirty;
        }

        public object GetSerializedValue(bool resetDirty)
        {
            if (resetDirty)
                Dirty = false;

            return this;
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