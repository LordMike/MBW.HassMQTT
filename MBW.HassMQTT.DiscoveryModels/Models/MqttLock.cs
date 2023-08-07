#nullable enable
using System.Collections.Generic;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models;

/// <summary>
/// https://www.home-assistant.io/integrations/lock.mqtt/
///
/// The mqtt lock platform lets you control your MQTT enabled locks.
/// </summary>
/// <remarks>
/// In an ideal scenario, the MQTT device will have a state_topic to publish state changes. If these messages
/// are published with a RETAIN flag, the MQTT lock will receive an instant state update after subscription and
/// will start with correct state. Otherwise, the initial state of the lock will be false / unlocked.
/// 
/// When a state_topic is not available, the lock will work in optimistic mode.In this mode, the lock will
/// immediately change state after every command.Otherwise, the lock will wait for state confirmation from
/// the device (message from state_topic).
/// 
/// Optimistic mode can be forced, even if state topic is available.Try to enable it, if experiencing incorrect
/// lock operation.
///
/// It's mandatory for locks to support `lock` and `unlock`. A lock may optionally support `open`, (e.g. to open
/// the bolt in addition to the latch), in this case, `payload_open` is required in the configuration. If the lock
/// is in optimistic mode, it will change states to `unlocked` when handling the `open` command.
/// </remarks>
[DeviceType(HassDeviceType.Lock)]
[PublicAPI]
public class MqttLock : MqttSensorDiscoveryBase<MqttLock, MqttLock.MqttLockValidator>, IHasUniqueId, IHasAvailability,
    IHasQos, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasRetain, IHasEntityCategory, IHasObjectId,
    IHasEncoding, IHasName
{
    public MqttLock(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
    {
    }

    /// <summary>
    /// A regular expression to validate a supplied code when it is set during the service call to `open`, `lock` or `unlock` the MQTT lock.
    /// </summary>
    public string CodeFormat { get; set; }

    /// <summary>
    /// Defines a template to generate the payload to send to `command_topic`. The lock command template accepts the parameters `value` and `code`. The `value` parameter will contain the configured value for either `payload_open`, `payload_lock` or `payload_unlock`. The `code` parameter is set during the service call to `open`, `lock` or `unlock` the MQTT lock and will be set `None` if no code was passed.
    /// </summary>
    public string CommandTemplate { get; set; }

    /// <summary>
    /// The MQTT topic to publish commands to change the lock state.
    /// </summary>
    public string CommandTopic { get; set; }

    /// <summary>
    /// Flag that defines if lock works in optimistic mode.
    /// </summary>
    public bool? Optimistic { get; set; }

    /// <summary>
    /// The payload sent to the lock to lock it.
    /// </summary>
    /// <remarks>Default value: 'LOCK'</remarks>
    public string? PayloadLock { get; set; }

    /// <summary>
    /// The payload sent to the lock to unlock it.
    /// </summary>
    /// <remarks>Default value: 'UNLOCK'</remarks>
    public string? PayloadUnlock { get; set; }

    /// <summary>
    /// The payload sent to the lock to open it.
    /// </summary>
    /// <remarks>Default value: 'OPEN'</remarks>
    public string? PayloadOpen { get; set; }

    /// <summary>
    /// The payload sent to `state_topic` by the lock when it's jammed.
    /// </summary>
    /// <remarks>Default value: 'JAMMED'</remarks>
    public string? StateJammed { get; set; }

    /// <summary>
    /// The payload sent to `state_topic` by the lock when it's locked.
    /// </summary>
    /// <remarks>Default value: 'LOCKED'</remarks>
    public string? StateLocked { get; set; }

    /// <summary>
    /// The payload sent to `state_topic` by the lock when it's locking.
    /// </summary>
    /// <remarks>Default value: 'LOCKING'</remarks>
    public string? StateLocking { get; set; }

    /// <summary>
    /// The MQTT topic subscribed to receive state updates. It accepts states configured with `state_jammed`, `state_locked`, `state_unlocked`, `state_locking` or `state_unlocking`.
    /// </summary>
    public string? StateTopic { get; set; }

    /// <summary>
    /// The payload sent to `state_topic` by the lock when it's unlocked.
    /// </summary>
    /// <remarks>Default value: 'UNLOCKED'</remarks>
    public string? StateUnlocked { get; set; }

    /// <summary>
    /// The payload sent to `state_topic` by the lock when it's unlocking.
    /// </summary>
    /// <remarks>Default value: 'UNLOCKING'</remarks>
    public string? StateUnlocking { get; set; }

    /// <summary>
    /// Defines a template to extract a state value from the payload.
    /// </summary>
    public string? ValueTemplate { get; set; }

    public string? UniqueId { get; set; }
    public IList<AvailabilityModel>? Availability { get; set; }
    public AvailabilityMode? AvailabilityMode { get; set; }
    public MqttQosLevel? Qos { get; set; }
    public string? JsonAttributesTemplate { get; set; }
    public string? JsonAttributesTopic { get; set; }
    public string? Icon { get; set; }
    public bool? EnabledByDefault { get; set; }
    public bool? Retain { get; set; }
    public EntityCategory? EntityCategory { get; set; }
    public string? ObjectId { get; set; }
    public string? Encoding { get; set; }
    public string? Name { get; set; }

    public class MqttLockValidator : MqttSensorDiscoveryBaseValidator<MqttLock>
    {
        public MqttLockValidator()
        {
            TopicAndTemplate(s => s.StateTopic, s => s.ValueTemplate);
        }
    }
}