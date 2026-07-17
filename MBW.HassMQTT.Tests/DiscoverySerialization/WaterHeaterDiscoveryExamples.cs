using System;
using MBW.HassMQTT.DiscoveryModels.Models;
using Xunit;

namespace MBW.HassMQTT.Tests.DiscoverySerialization;

public class WaterHeaterDiscoveryExamples
{
    public static TheoryData<string, Type, string, bool> HaExamples => new()
    {
        {
            "Configuration",
            typeof(MqttWaterHeater),
            """
            {
              "name": "Boiler",
              "mode_command_topic": "basement/boiler/mode/set"
            }
            """,
            false
        },
        {
            "Using templates",
            typeof(MqttWaterHeater),
            """
            {
              "name": "Boiler",
              "modes": [
                "off",
                "eco",
                "performance"
              ],
              "mode_command_topic": "basement/boiler/mode/set",
              "mode_state_topic": "basement/boiler/mode/state",
              "mode_state_template": "{{ value_json }}"
            }
            """,
            false
        },
        {
            "Example",
            typeof(MqttWaterHeater),
            """
            {
              "name": "Boiler",
              "modes": [
                "off",
                "eco",
                "performance"
              ],
              "mode_state_topic": "basement/boiler/mode",
              "mode_command_topic": "basement/boiler/mode/set",
              "mode_command_template": "{{ value if value==\"off\" else \"on\" }}",
              "temperature_state_topic": "basement/boiler/temperature",
              "temperature_command_topic": "basement/boiler/temperature/set",
              "current_temperature_topic": "basement/boiler/current_temperature",
              "precision": 1.0
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
