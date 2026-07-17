using System;
using MBW.HassMQTT.DiscoveryModels.Models;
using Xunit;

namespace MBW.HassMQTT.Tests.DiscoverySerialization;

public class SwitchDiscoveryExamples
{
    public static TheoryData<string, Type, string, bool> HaExamples => new()
    {
        {
            "Configuration",
            typeof(MqttSwitch),
            """
            {
              "command_topic": "home/bedroom/switch1/set"
            }
            """,
            false
        },
        {
            "Full configuration",
            typeof(MqttSwitch),
            """
            {
              "unique_id": "bedroom_switch",
              "name": "Bedroom Switch",
              "state_topic": "home/bedroom/switch1",
              "command_topic": "home/bedroom/switch1/set",
              "availability": [
                {
                  "topic": "home/bedroom/switch1/available"
                }
              ],
              "payload_on": "ON",
              "payload_off": "OFF",
              "state_on": "ON",
              "state_off": "OFF",
              "optimistic": false,
              "qos": 0,
              "retain": true
            }
            """,
            false
        },
        {
            "Set the state of a device with ESPEasy",
            typeof(MqttSwitch),
            """
            {
              "name": "bathroom",
              "state_topic": "home/bathroom/gpio/13",
              "command_topic": "home/bathroom/gpio/13",
              "payload_on": "1",
              "payload_off": "0"
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
