using System;
using MBW.HassMQTT.DiscoveryModels.Models;
using Xunit;

namespace MBW.HassMQTT.Tests.DiscoverySerialization;

public class FanDiscoveryExamples
{
    public static TheoryData<string, Type, string, bool> HaExamples => new()
    {
        {
            "Configuration",
            typeof(MqttFan),
            """
            {
              "command_topic": "bedroom_fan/on/set"
            }
            """,
            false
        },
        {
            "Full configuration",
            typeof(MqttFan),
            """
            {
              "name": "Bedroom Fan",
              "state_topic": "bedroom_fan/on/state",
              "command_topic": "bedroom_fan/on/set",
              "direction_state_topic": "bedroom_fan/direction/state",
              "direction_command_topic": "bedroom_fan/direction/set",
              "oscillation_state_topic": "bedroom_fan/oscillation/state",
              "oscillation_command_topic": "bedroom_fan/oscillation/set",
              "percentage_state_topic": "bedroom_fan/speed/percentage_state",
              "percentage_command_topic": "bedroom_fan/speed/percentage",
              "preset_mode_state_topic": "bedroom_fan/preset/preset_mode_state",
              "preset_mode_command_topic": "bedroom_fan/preset/preset_mode",
              "preset_modes": [
                "auto",
                "smart",
                "whoosh",
                "eco",
                "breeze"
              ],
              "qos": 0,
              "payload_on": "true",
              "payload_off": "false",
              "payload_oscillation_on": "true",
              "payload_oscillation_off": "false",
              "speed_range_min": 1,
              "speed_range_max": 10
            }
            """,
            false
        },
        {
            "Configuration using command templates 1",
            typeof(MqttFan),
            """
            {
              "name": "Bedroom Fan",
              "command_topic": "bedroom_fan/on/set",
              "command_template": "{ state: '{{ value }}'}",
              "direction_command_template": "{{ iif(value == 'forward', 'fwd', 'rev') }}",
              "direction_value_template": "{{ iif(value == 'fwd', 'forward', 'reverse') }}",
              "oscillation_command_topic": "bedroom_fan/oscillation/set",
              "oscillation_command_template": "{ oscillation: '{{ value }}'}",
              "percentage_command_topic": "bedroom_fan/speed/percentage",
              "percentage_command_template": "{ percentage: '{{ value }}'}",
              "preset_mode_command_topic": "bedroom_fan/preset/preset_mode",
              "preset_mode_command_template": "{ preset_mode: '{{ value }}'}",
              "preset_modes": [
                "auto",
                "smart",
                "whoosh",
                "eco",
                "breeze"
              ]
            }
            """,
            false
        },
        {
            "Configuration using command templates 2",
            typeof(MqttFan),
            """
            {
              "name": "Bedroom Fan",
              "direction_command_template": "{{ iif(value == 'forward', 'fwd', 'rev') }}",
              "direction_value_template": "{{ iif(value == 'fwd', 'forward', 'reverse') }}"
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
