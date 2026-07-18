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

# Example usage

This example configures the library with Microsoft.Extensions.DependencyInjection,
connects to an MQTT broker, and publishes a sensor:

```shell
dotnet add package MBW.HassMQTT.CommonServices
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package Microsoft.Extensions.Logging.Console
```

```csharp
using MBW.HassMQTT;
using MBW.HassMQTT.CommonServices;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Models;
using MBW.HassMQTT.Extensions;
using MBW.HassMQTT.Topics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

await using ServiceProvider provider = new ServiceCollection()
    .AddLogging(logging => logging.AddConsole())
    .Configure<HassConfiguration>(configuration =>
    {
        configuration.DiscoveryPrefix = "homeassistant";
        configuration.TopicPrefix = "weather";
    })
    .Configure<CommonMqttConfiguration>(configuration =>
    {
        configuration.Server = "localhost";
        configuration.Port = 1883;
        configuration.ClientId = "weather-sample";
    })
    .AddSingleton(provider => new HassMqttTopicBuilder(
        provider.GetRequiredService<IOptions<HassConfiguration>>().Value))
    .AddAndConfigureMqtt("WeatherSample")
    .BuildServiceProvider();

// An IHost starts its IHostedService registrations automatically. This standalone
// example starts the MQTT client service explicitly.
var mqttClientService = provider.GetRequiredService<IHostedService>();
await mqttClientService.StartAsync(default);

var manager = provider.GetRequiredService<HassMqttManager>();
var outsideTemperature = manager
    .CreateEntity<MqttSensor>()
    .ConfigureTopics(HassTopicKind.State, HassTopicKind.JsonAttributes)
    .ConfigureDevice(device =>
    {
        device.Name = "Weather station";
        device.Identifiers.Add("weather-station");
    })
    .ConfigureDiscovery(discovery =>
    {
        discovery.Name = "Outside temperature";
        discovery.DeviceClass = HassSensorDeviceClass.Temperature;
        discovery.UnitOfMeasurement = "°C";
        discovery.StateClass = HassStateClass.Measurement;
    })
    .Build("weather-station", "outside-temperature");

outsideTemperature.SetValue(HassTopicKind.State, 21.4);
outsideTemperature.SetAttribute("quality", "good");

// FlushAll attempts delivery immediately. Pending data remains queued while
// disconnected, so keep the application running to allow reconnect delivery.
await manager.FlushAll();
```

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
