using System;
using MBW.HassMQTT.DiscoveryModels.Models;
using Xunit;

namespace MBW.HassMQTT.Tests.DiscoverySerialization;

public class SceneDiscoveryExamples
{
    public static TheoryData<string, Type, string, bool> HaExamples => new()
    {
        {
            "Configuration",
            typeof(MqttScene),
            """
            {
              "command_topic": "zigbee2mqtt/living_room_group/set"
            }
            """,
            false
        },
        {
            "Full configuration",
            typeof(MqttScene),
            """
            {
              "unique_id": "living_room_party_scene",
              "name": "Party Scene",
              "command_topic": "home/living_room/party_scene/set",
              "availability": [
                {
                  "topic": "home/living_room/party_scene/available"
                }
              ],
              "payload_on": "ON",
              "qos": 0,
              "retain": true,
              "device": {
                "name": "Living Room",
                "identifiers": "livingroom_lights"
              }
            }
            """,
            false
        },
        {
            "Use with a JSON Payload",
            typeof(MqttScene),
            """
            {
              "name": "Living Room Blue Scene",
              "unique_id": "living_room_blue_scene",
              "command_topic": "home/living_room/set",
              "payload_on": "{\"activate_scene\": \"Blue Scene\"}"
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
