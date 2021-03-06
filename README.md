# MBW.HassMQTT [![Generic Build](https://github.com/LordMike/MBW.HassMQTT/actions/workflows/dotnet.yml/badge.svg)](https://github.com/LordMike/MBW.HassMQTT/actions/workflows/dotnet.yml) [![Nuget](https://img.shields.io/nuget/v/MBW.HassMQTT)](https://www.nuget.org/packages/MBW.HassMQTT)

Code to help me build integrations for Home Assistants MQTT integration. Contains models, common service and helper utilities.

There will be little to no support on this.

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
