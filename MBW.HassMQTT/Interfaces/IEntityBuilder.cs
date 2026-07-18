using System;
using MBW.HassMQTT.DiscoveryModels.Device;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;

namespace MBW.HassMQTT.Interfaces;

/// <summary>
/// An immutable, reusable discovery configuration that can build one or more MQTT entities.
/// </summary>
/// <typeparam name="TEntity">The discovery document type.</typeparam>
public interface IEntityBuilder<TEntity> where TEntity : IHassDiscoveryDocument
{
    internal HassMqttManager HassMqttManager { get; }

    /// <summary>
    /// Requires publish topics for the supplied kinds. Missing topic values are generated during <see cref="Build" />.
    /// </summary>
    IEntityBuilder<TEntity> ConfigureTopics(params HassTopicKind[] topicKinds);

    /// <summary>Returns a builder containing the configured discovery snapshot.</summary>
    IEntityBuilder<TEntity> ConfigureDiscovery(Action<TEntity> configure);

    /// <summary>Returns a builder containing the configured device snapshot.</summary>
    IEntityBuilder<TEntity> ConfigureDevice(Action<MqttDeviceDocument> configure);

    /// <summary>
    /// Validates and compiles this configuration, then atomically registers the completed runtime entity.
    /// </summary>
    IHassMqttEntity Build(string deviceId, string entityId, string uniqueId = null);
}
