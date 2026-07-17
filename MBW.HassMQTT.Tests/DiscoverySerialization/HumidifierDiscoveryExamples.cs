using System;
using MBW.HassMQTT.DiscoveryModels.Models;
using Xunit;

namespace MBW.HassMQTT.Tests.DiscoverySerialization;

public class HumidifierDiscoveryExamples
{
    public static TheoryData<string, Type, string, bool> HaExamples => new()
    {
        {
            "Configuration",
            typeof(MqttHumidifier),
            """
            {
              "command_topic": "bedroom_humidifier/on/set",
              "target_humidity_command_topic": "bedroom_humidifier/humidity/set"
            }
            """,
            false
        },
        {
            "Full configuration",
            typeof(MqttHumidifier),
            """
            {
              "name": "Bedroom humidifier",
              "device_class": "humidifier",
              "state_topic": "bedroom_humidifier/on/state",
              "action_topic": "bedroom_humidifier/action",
              "command_topic": "bedroom_humidifier/on/set",
              "current_humidity_topic": "bedroom_humidifier/humidity/current",
              "target_humidity_command_topic": "bedroom_humidifier/humidity/set",
              "target_humidity_state_topic": "bedroom_humidifier/humidity/state",
              "mode_state_topic": "bedroom_humidifier/mode/state",
              "mode_command_topic": "bedroom_humidifier/preset/preset_mode",
              "modes": [
                "normal",
                "eco",
                "away",
                "boost",
                "comfort",
                "home",
                "sleep",
                "auto",
                "baby"
              ],
              "qos": 0,
              "payload_on": "true",
              "payload_off": "false",
              "min_humidity": 30,
              "max_humidity": 80
            }
            """,
            false
        },
    };

    [Theory]
    [MemberData(nameof(HaExamples))]
    public void DocumentationExamplesRoundTrip(string name, Type modelType, string json, bool normalizeExplicitNulls)
    {
        DiscoveryJsonRoundTrip.Assert(name, modelType, json, normalizeExplicitNulls);
    }
}
