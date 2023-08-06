#nullable enable
using System;

namespace MBW.HassMQTT.DiscoveryModels.Models;

/// <summary>
/// Migrate to <see cref="MqttVacuum"/>
/// </summary>
[Obsolete("Migrate to 'MqttVacuum'")]
public class MqttVacuumState : MqttVacuum 
{
    public MqttVacuumState(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
    {
    }
}