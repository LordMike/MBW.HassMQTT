using System;
using MBW.HassMQTT.DiscoveryModels.Models;
using Xunit;

namespace MBW.HassMQTT.Tests.DiscoverySerialization;

public class ButtonDiscoveryExamples
{
    public static TheoryData<string, Type, string, bool> HaExamples => new()
    {
        {
            "Configuration",
            typeof(MqttButton),
            """
            {
              "command_topic": "home/bedroom/switch1/reboot"
            }
            """,
            false
        },
        {
            "Full configuration",
            typeof(MqttButton),
            """
            {
              "unique_id": "bedroom_switch_reboot_btn",
              "name": "Restart Bedroom Switch",
              "command_topic": "home/bedroom/switch1/commands",
              "payload_press": "restart",
              "availability": [
                {
                  "topic": "home/bedroom/switch1/available"
                }
              ],
              "qos": 0,
              "retain": false,
              "entity_category": "config",
              "device_class": "restart"
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
