using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Models;
using MBW.HassMQTT.Helpers;
using MBW.HassMQTT.Services;
using MBW.HassMQTT.Topics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MQTTnet.Client;

namespace MBW.HassMQTT.CommonServices.AliveAndWill
{
    /// <summary>
    /// Service that uses the LWT (Last will testament) of MQTT to notify HASS what the status of this application is.
    /// This is useful to give end users and idea of the uptime of the service, and to notify will stop receiving updates.
    /// </summary>
    public class HassConnectedEntityService : BackgroundService
    {
        private readonly HassConnectedEntityServiceConfig _config;
        private readonly IMqttClient _mqttClient;
        private readonly MqttEvents _mqttEvents;
        private readonly HassMqttManager _hassMqttManager;

        public string OkMessage => _config.OkMessage;
        public string ProblemMessage => _config.ProblemMessage;

        public string StateTopic { get; }
        private readonly string _attributesTopic;

        public HassConnectedEntityService(IOptions<HassConnectedEntityServiceConfig> options,
            IMqttClient mqttClient,
            MqttEvents mqttEvents,
            HassMqttManager hassMqttManager,
            HassMqttTopicBuilder topicBuilder)
        {
            _config = options.Value;
            _mqttClient = mqttClient;
            _mqttEvents = mqttEvents;
            _hassMqttManager = hassMqttManager;

            StateTopic = topicBuilder.GetServiceTopic(_config.DeviceId, _config.EntityId);
            _attributesTopic = topicBuilder.GetAttributesTopic(_config.DeviceId, _config.EntityId);
        }

        private void CreateSystemEntities()
        {
            MqttBinarySensor sensor = _hassMqttManager.ConfigureDiscovery<MqttBinarySensor>(_config.DeviceId, _config.EntityId);

            sensor.Device.Name = _config.DiscoveryDeviceName;
            sensor.Device.Identifiers = new[] { _config.DeviceId };

            Assembly entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
                sensor.Device.SwVersion = entryAssembly.GetName().Version.ToString(3);

            sensor.Name = _config.DiscoveryEntityName;
            sensor.DeviceClass = HassDeviceClass.Problem;

            sensor.PayloadOn = ProblemMessage;
            sensor.PayloadOff = OkMessage;

            sensor.StateTopic = StateTopic;
            sensor.JsonAttributesTopic = _attributesTopic;
        }

        public void SetAttribute(string name, object value)
        {
            MqttAttributesTopic attributes = _hassMqttManager.GetAttributesValue(_config.DeviceId, _config.EntityId);

            attributes.SetAttribute(name, value);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            CreateSystemEntities();

            // Push starting values
            MqttAttributesTopic attributes = _hassMqttManager.GetAttributesValue(_config.DeviceId, _config.EntityId);

            Assembly entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
                attributes.SetAttribute("version", entryAssembly.GetName().Version.ToString(3));

            attributes.SetAttribute("started", DateTime.UtcNow);

            // Hook to on connect
            // Connected: Push "ok" message
            // Testament (When disconnected): Leave "problem" message

            _mqttEvents.OnConnect += async (args, token) =>
            {
                if (stoppingToken.IsCancellationRequested)
                    return;

                await _mqttClient.SendValueAsync(StateTopic, OkMessage, token);
            };

            // Send initial Ok message
            await _mqttClient.SendValueAsync(StateTopic, OkMessage, stoppingToken);
        }
    }
}