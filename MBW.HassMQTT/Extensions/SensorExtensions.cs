using System;
using EnumsNET;
using MBW.HassMQTT.DiscoveryModels.Device;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.Interfaces;

namespace MBW.HassMQTT.Extensions
{
    public static class SensorExtensions
    {
        public delegate void DiscoveryConfigure<TEntity>(TEntity discovery) where TEntity : IHassDiscoveryDocument;

        public delegate void DeviceConfigure(MqttDeviceDocument device);

        public static IDiscoveryDocumentBuilder<TEntity> ConfigureTopics<TEntity>(this IDiscoveryDocumentBuilder<TEntity> builder, params HassTopicKind[] topicKinds) where TEntity : IHassDiscoveryDocument
        {
            foreach (HassTopicKind topicKind in topicKinds)
            {
                string topic = builder.HassMqttManager.TopicBuilder.GetServiceTopic(builder.DeviceId, builder.EntityId, topicKind.AsString(EnumFormat.EnumMemberValue));
                builder.Discovery.SetTopic(topicKind, topic);
            }

            return builder;
        }

        public static IDiscoveryDocumentBuilder<TEntity> ConfigureDiscovery<TEntity>(this IDiscoveryDocumentBuilder<TEntity> builder, DiscoveryConfigure<TEntity> configure) where TEntity : IHassDiscoveryDocument
        {
            configure(builder.Discovery);
            return builder;
        }

        public static IDiscoveryDocumentBuilder<TEntity> ConfigureDevice<TEntity>(this IDiscoveryDocumentBuilder<TEntity> builder, DeviceConfigure configure) where TEntity : IHassDiscoveryDocument
        {
            configure(builder.Discovery.Device);
            return builder;
        }

        /// <summary>
        /// Get the Json Attributes sender for this device and entity
        /// </summary>
        public static MqttAttributesTopic GetAttributesSender(this ISensorContainer builder)
        {
            if (!(builder.Discovery is IHasJsonAttributes asAttributesTopic))
                throw new InvalidOperationException($"Attempted to get attributes sender for an invalid type, {builder.Discovery.GetType().Name}");

            string topic = asAttributesTopic.JsonAttributesTopic;
            return builder.HassMqttManager.GetAttributesSender(topic);
        }

        /// <summary>
        /// Get a well-known value sender for this device and entity
        /// </summary>
        public static MqttStateValueTopic GetValueSender(this ISensorContainer builder, HassTopicKind topicKind)
        {
            string topic = builder.Discovery.GetTopic(topicKind);
            return builder.HassMqttManager.GetValueSender(topic);
        }

        /// <summary>
        /// Get a custom value sender for this device and entity
        /// </summary>
        public static MqttStateValueTopic GetValueSender(this ISensorContainer builder, string valueName)
        {
            string topic = builder.HassMqttManager.TopicBuilder.GetServiceTopic(builder.DeviceId, builder.EntityId, valueName);
            return builder.HassMqttManager.GetValueSender(topic);
        }

        public static ISensorContainer SetAttribute(this ISensorContainer sensor, string name, object value)
        {
            sensor.GetAttributesSender().SetAttribute(name, value);
            return sensor;
        }

        public static ISensorContainer SetValue(this ISensorContainer sensor, HassTopicKind topicKind, object value)
        {
            sensor.GetValueSender(topicKind).Value = value;
            return sensor;
        }

        public static ISensorContainer GetSensor(this HassMqttManager manager, string deviceId, string entityId)
        {
            if (manager.TryGetSensor(deviceId, entityId, out var sensor))
                return sensor;

            throw new InvalidOperationException($"Unable to find sensor {deviceId}/{entityId} - is it configured?");
        }

        public static ISensorContainer GetSensor<TEntity>(this IDiscoveryDocumentBuilder<TEntity> builder) where TEntity : IHassDiscoveryDocument => builder.HassMqttManager.GetSensor(builder.DeviceId, builder.EntityId);
    }
}