using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Models;
using MBW.HassMQTT.Extensions;
using MBW.HassMQTT.Helpers;
using MBW.HassMQTT.Interfaces;
using MBW.HassMQTT.Services;
using MBW.HassMQTT.Topics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Extensions.ManagedClient;

namespace MBW.HassMQTT.CommonServices.AliveAndWill;

/// <summary>
/// Service that uses the LWT (Last will testament) of MQTT to notify HASS what the status of this application is.
/// This is useful to give end users and idea of the uptime of the service, and to notify will stop receiving updates.
/// </summary>
public class HassConnectedEntityService : BackgroundService, IMqttEventReceiver
{
    private readonly HassConnectedEntityServiceConfig _config;
    private readonly IManagedMqttClient _mqttClient;
    private readonly MqttEvents _mqttEvents;
    private readonly HassMqttManager _hassMqttManager;

    public string OkMessage => _config.OkMessage;
    public string ProblemMessage => _config.ProblemMessage;

    public string StateTopic { get; }

    public HassConnectedEntityService(IOptions<HassConnectedEntityServiceConfig> options,
        IManagedMqttClient mqttClient,
        MqttEvents mqttEvents,
        HassMqttManager hassMqttManager,
        HassMqttTopicBuilder topicBuilder)
    {
        _config = options.Value;
        _mqttClient = mqttClient;
        _mqttEvents = mqttEvents;
        _hassMqttManager = hassMqttManager;

        StateTopic = topicBuilder.GetServiceTopic(_config.DeviceId, _config.EntityId, "state");
    }

    private void CreateSystemEntities()
    {
        _hassMqttManager.ConfigureSensor<MqttBinarySensor>(_config.DeviceId, _config.EntityId)
            .ConfigureTopics(HassTopicKind.State, HassTopicKind.JsonAttributes)
            .ConfigureDevice(device =>
            {
                Assembly entryAssembly = Assembly.GetEntryAssembly();
                if (entryAssembly != null)
                    device.SwVersion = entryAssembly.GetName().Version.ToString(3);

                device.Name = _config.DiscoveryDeviceName;
                device.Identifiers.Add(_config.DeviceId);
            })
            .ConfigureDiscovery(discovery =>
            {
                discovery.Name = _config.DiscoveryEntityName;
                discovery.DeviceClass = HassBinarySensorDeviceClass.Problem;

                discovery.PayloadOn = ProblemMessage;
                discovery.PayloadOff = OkMessage;
            });
    }

    public void SetAttribute(string name, object value)
    {
        ISensorContainer sensor = _hassMqttManager.GetSensor(_config.DeviceId, _config.EntityId);

        sensor.SetAttribute(name, value);
    }

    async Task IMqttEventReceiver.OnConnect(MqttClientConnectedEventArgs args, CancellationToken token)
    {
        // Hook to on connect
        // Connected: Push "ok" message
        // Testament (When disconnected): Leave "problem" message
        if (token.IsCancellationRequested)
            return;

        await _mqttClient.SendValueAsync(StateTopic, OkMessage, token);
    }

    Task IMqttEventReceiver.OnDisconnect(MqttClientDisconnectedEventArgs args, CancellationToken token)
    {
        // Do nothing
        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        CreateSystemEntities();

        // Push starting values
        ISensorContainer sensor = _hassMqttManager.GetSensor(_config.DeviceId, _config.EntityId);
        MqttAttributesTopic attributes = sensor.GetAttributesSender();

        Assembly entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly != null)
            attributes.SetAttribute("version", entryAssembly.GetName().Version.ToString(3));

        attributes.SetAttribute("started", DateTime.UtcNow);
    }
}