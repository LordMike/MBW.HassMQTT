using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using MBW.HassMQTT.Abstracts.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Device;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
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
        [UsedImplicitly]
        protected virtual void OnPropertyChanged(string propertyName, object before, object after)
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

        /// <summary>
        /// Device details for this entity, usually this is duplicated between multiple entities to let HA link them together.
        /// At least one of identifiers or connections must be present to identify the device.
        /// </summary>
        public MqttDeviceDocument Device { get; }

        public MqttSensorDiscoveryBase(string discoveryTopic, string uniqueId)
        {
            PublishTopic = discoveryTopic;

            if (this is IHasUniqueId asHasUniqueId)
                asHasUniqueId.UniqueId = uniqueId;

#pragma warning disable 618
            if (this is IHasAvailability asAvailabilityTopic)
                asAvailabilityTopic.Availability = new List<AvailabilityModel>();
#pragma warning restore 618

            Device = new MqttDeviceDocument();
            Device.PropertyChanged += (_, _) => SetDirty();
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