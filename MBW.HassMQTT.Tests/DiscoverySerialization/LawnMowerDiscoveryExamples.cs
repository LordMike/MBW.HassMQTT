using System;
using MBW.HassMQTT.DiscoveryModels.Models;
using Xunit;

namespace MBW.HassMQTT.Tests.DiscoverySerialization;

public class LawnMowerDiscoveryExamples
{
    public static TheoryData<string, Type, string, bool> HaExamples => new()
    {
        {
            "Configuration",
            typeof(MqttLawnMower),
            """
            {
              "name": "Test Lawn Mower"
            }
            """,
            false
        },
        {
            "Example",
            typeof(MqttLawnMower),
            """
            {
              "name": "Lawn Mower Plus",
              "activity_state_topic": "lawn_mower_plus/state",
              "activity_value_template": "{{ value_json.activity }}",
              "pause_command_topic": "lawn_mower_plus/set",
              "pause_command_template": "{\"activity\": \"{{ value }}\"}",
              "dock_command_topic": "lawn_mower_plus/set",
              "dock_command_template": "{\"activity\": \"{{ value }}\"}",
              "start_mowing_command_topic": "lawn_mower_plus/set",
              "start_mowing_command_template": "{\"activity\": \"{{ value }}\"}"
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
