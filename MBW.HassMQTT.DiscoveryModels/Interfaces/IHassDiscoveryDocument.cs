using FluentValidation;
using MBW.HassMQTT.Abstracts.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Device;
using MBW.HassMQTT.DiscoveryModels.Enum;

namespace MBW.HassMQTT.DiscoveryModels.Interfaces
{
    public interface IHassDiscoveryDocument : IMqttValueContainer
    {
        /// <summary>
        /// Device details for this entity, usually this is duplicated between multiple entities to let HA link them together.
        /// At least one of identifiers or connections must be present to identify the device.
        /// </summary>
        MqttDeviceDocument Device { get; }

        void SetTopic(HassTopicKind topicKind, string topic);
        string GetTopic(HassTopicKind topicKind);

        IValidator Validator { get; }
    }
}