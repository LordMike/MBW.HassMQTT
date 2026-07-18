# MBW.HassMQTT [![Generic Build](https://github.com/LordMike/MBW.HassMQTT/actions/workflows/dotnet.yml/badge.svg)](https://github.com/LordMike/MBW.HassMQTT/actions/workflows/dotnet.yml) [![Nuget](https://img.shields.io/nuget/v/MBW.HassMQTT)](https://www.nuget.org/packages/MBW.HassMQTT)

Code to help me build integrations for Home Assistants MQTT integration. Contains models, common service and helper utilities.

There will be little to no support on this.

# Version 4

Version 4 targets .NET 8 and .NET 10 and uses MQTTnet 5. The MQTT connection is
owned by a supervised hosted service which reconnects with bounded backoff,
restores subscriptions, and republishes the latest dirty state after reconnecting.

Applications upgrading from version 3 should note these API changes:

* MQTTnet's managed client is no longer exposed or used. Resolve
  `IHassMqttClient` when direct publish or subscription access is required.
* `HassMqttManager.FlushAll` returns `MqttFlushResult`. Existing awaited calls may
  discard the result; callers that need delivery status can inspect it.
* `MqttValueTopic.SetDirty()` is now `MarkDirty()`.
* Dirty state is revision-based. Multiple updates while offline are coalesced and
  the latest value is published after reconnection.

No package is provided for .NET versions older than .NET 8.

# Entity builders (next major)

Entity discovery is configured through an immutable builder and is registered only
when `Build` succeeds. Device and entity identifiers are supplied at build time, so
one configured template can be reused safely:

```csharp
var temperature = manager
    .CreateEntity<MqttSensor>()
    .ConfigureTopics(HassTopicKind.State, HassTopicKind.JsonAttributes)
    .ConfigureDevice(device =>
    {
        device.Name = "Weather station";
        device.Identifiers.Add("weather");
    })
    .ConfigureDiscovery(discovery =>
    {
        discovery.Name = "Outside temperature";
        discovery.UnitOfMeasurement = "°C";
    });

IHassMqttEntity outside = temperature.Build("weather", "outside");
outside.SetValue(HassTopicKind.State, 21.4);
outside.SetAttribute("quality", "good");
```

`CreateEntity` does not modify the manager. `Build` resolves missing requested
topics, validates and snapshots discovery, compiles publishing operations, and then
registers the finished entity atomically. The built entity never exposes the mutable
discovery draft. Building the same device/entity identity twice is rejected. This
replaces the previous `ConfigureSensor` flow, and discovery validation can no longer
be deferred to `FlushAll` or disabled through manager configuration.

# Features

* Complete models for each of the Home Assistant MQTT entities
* Managers to control sending only updates to HASS through MQTT
* Standardized MQTT topics layout
* Common services to help creating status and operational entities, to notify when the service is down

# Nuget packages

| Name | Nuget | Note |
|---|---|---|
| MBW.HassMQTT | [![Nuget](https://img.shields.io/nuget/v/MBW.HassMQTT)](https://www.nuget.org/packages/MBW.HassMQTT/) | Core services such as value managers and senders |
| MBW.HassMQTT.Abstracts | [![Nuget](https://img.shields.io/nuget/v/MBW.HassMQTT.Abstracts)](https://www.nuget.org/packages/MBW.HassMQTT.Abstracts/) | Common interfaces |
| MBW.HassMQTT.CommonServices | [![Nuget](https://img.shields.io/nuget/v/MBW.HassMQTT.CommonServices)](https://www.nuget.org/packages/MBW.HassMQTT.CommonServices/) | Addon service such as notifying HASS of downtime with MQTT LWT |
| MBW.HassMQTT.DiscoveryModels | [![Nuget](https://img.shields.io/nuget/v/MBW.HassMQTT.DiscoveryModels)](https://www.nuget.org/packages/MBW.HassMQTT.DiscoveryModels/) | HASS MQTT discovery models |
