using System;
using MBW.HassMQTT.DiscoveryModels.Models;
using Xunit;

namespace MBW.HassMQTT.Tests.DiscoverySerialization;

public class VacuumDiscoveryExamples
{
    public static TheoryData<string, Type, string, bool> HaExamples => new()
    {
        {
            "Configuration",
            typeof(MqttVacuum),
            """
            {
              "state_topic": "state-topic",
              "command_topic": "command-topic"
            }
            """,
            false
        },
        {
            "Configuration example",
            typeof(MqttVacuum),
            """
            {
              "name": "MQTT Vacuum",
              "supported_features": [
                "start",
                "pause",
                "stop",
                "return_home",
                "status",
                "locate",
                "clean_spot",
                "fan_speed",
                "send_command"
              ],
              "command_topic": "vacuum/command",
              "clean_segments_command_topic": "vacuum/clean_segments",
              "set_fan_speed_topic": "vacuum/set_fan_speed",
              "fan_speed_list": [
                "min",
                "medium",
                "high",
                "max"
              ],
              "send_command_topic": "vacuum/send_command"
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
