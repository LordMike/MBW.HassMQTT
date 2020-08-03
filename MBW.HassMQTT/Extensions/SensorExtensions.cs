﻿using System;
using EnumsNET;
using MBW.HassMQTT.DiscoveryModels;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.Interfaces;

namespace MBW.HassMQTT.Extensions
{
    public static class SensorExtensions
    {
        public static IDiscoveryDocumentBuilder<TEntity> ConfigureTopics<TEntity>(this IDiscoveryDocumentBuilder<TEntity> builder, params HassTopicKind[] topicKinds) where TEntity : MqttSensorDiscoveryBase
        {
            foreach (HassTopicKind topicKind in topicKinds)
            {
                string topic = builder.HassMqttManager.TopicBuilder.GetServiceTopic(builder.DeviceId, builder.EntityId, topicKind.AsString(EnumFormat.EnumMemberValue));
                builder.Discovery.SetTopic(topicKind, topic);
            }

            return builder;
        }

        /// <summary>
        /// Get the Json Attributes sender for this device and entity
        /// </summary>
        public static MqttAttributesTopic GetAttributesSender(this ISensorContainer builder)
        {
            if (!(builder.Discovery is IHasAttributesTopic asAttributesTopic))
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
    }
}