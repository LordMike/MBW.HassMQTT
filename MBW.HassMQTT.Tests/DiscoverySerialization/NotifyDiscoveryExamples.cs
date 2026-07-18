using System;
using MBW.HassMQTT.DiscoveryModels.Models;
using Xunit;

namespace MBW.HassMQTT.Tests.DiscoverySerialization;

public class NotifyDiscoveryExamples
{
    public static TheoryData<string, Type, string, bool> HaExamples => new()
    {
        {
            "Configuration",
            typeof(MqttNotify),
            """
            {
              "command_topic": "home/living_room/status_screen/notifications"
            }
            """,
            false
        },
        {
            "Full configuration",
            typeof(MqttNotify),
            """
            {
              "unique_id": "living_room_stat_scr01",
              "name": "Living room status screen",
              "command_topic": "home/living_room/status_screen/notifications",
              "availability": [
                {
                  "topic": "home/living_room/status_screen/available"
                }
              ],
              "qos": 0,
              "retain": false
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
