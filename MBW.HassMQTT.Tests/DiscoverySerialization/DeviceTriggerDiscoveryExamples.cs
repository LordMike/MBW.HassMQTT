using System;
using MBW.HassMQTT.DiscoveryModels.Models;
using Xunit;

namespace MBW.HassMQTT.Tests.DiscoverySerialization;

public class DeviceTriggerDiscoveryExamples
{
    public static TheoryData<string, Type, string, bool> HaExamples => new()
    {
        {
            "Left arrow click configuration",
            typeof(MqttDeviceTrigger),
            """
            {
              "automation_type": "trigger",
              "type": "action",
              "subtype": "arrow_left_click",
              "payload": "arrow_left_click",
              "topic": "zigbee2mqtt/0x90fd9ffffedf1266/action",
              "device": {
                "identifiers": [
                  "zigbee2mqtt_0x90fd9ffffedf1266"
                ],
                "name": "0x90fd9ffffedf1266",
                "sw_version": "Zigbee2MQTT 1.14.0",
                "model": "TRADFRI remote control (E1524/E1810)",
                "manufacturer": "IKEA"
              }
            }
            """,
            false
        },
        {
            "Right arrow click configuration",
            typeof(MqttDeviceTrigger),
            """
            {
              "automation_type": "trigger",
              "type": "action",
              "subtype": "arrow_right_click",
              "payload": "arrow_right_click",
              "topic": "zigbee2mqtt/0x90fd9ffffedf1266/action",
              "device": {
                "identifiers": [
                  "zigbee2mqtt_0x90fd9ffffedf1266"
                ]
              }
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
