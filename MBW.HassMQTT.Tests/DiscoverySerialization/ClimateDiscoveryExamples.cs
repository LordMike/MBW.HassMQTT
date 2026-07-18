using System;
using MBW.HassMQTT.DiscoveryModels.Models;
using Xunit;

namespace MBW.HassMQTT.Tests.DiscoverySerialization;

public class ClimateDiscoveryExamples
{
    public static TheoryData<string, Type, string, bool> HaExamples => new()
    {
        {
            "Configuration",
            typeof(MqttClimate),
            """
            {
              "name": "Study",
              "mode_command_topic": "study/ac/mode/set"
            }
            """,
            false
        },
        {
            "Using templates",
            typeof(MqttClimate),
            """
            {
              "name": "Study",
              "modes": [
                "off",
                "heat",
                "auto"
              ],
              "mode_command_topic": "study/ac/mode/set",
              "mode_state_topic": "study/ac/mode/state",
              "mode_state_template": "{{ value_json }}"
            }
            """,
            false
        },
        {
            "Example",
            typeof(MqttClimate),
            """
            {
              "name": "Study",
              "modes": [
                "off",
                "cool",
                "fan_only"
              ],
              "swing_horizontal_modes": [
                "on",
                "off"
              ],
              "swing_modes": [
                "on",
                "off"
              ],
              "fan_modes": [
                "high",
                "medium",
                "low"
              ],
              "preset_modes": [
                "eco",
                "sleep",
                "activity"
              ],
              "power_command_topic": "study/ac/power/set",
              "preset_mode_command_topic": "study/ac/preset_mode/set",
              "mode_command_topic": "study/ac/mode/set",
              "mode_command_template": "{{ value if value==\"off\" else \"on\" }}",
              "temperature_command_topic": "study/ac/temperature/set",
              "fan_mode_command_topic": "study/ac/fan/set",
              "swing_horizontal_mode_command_topic": "study/ac/swingH/set",
              "swing_mode_command_topic": "study/ac/swing/set",
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
