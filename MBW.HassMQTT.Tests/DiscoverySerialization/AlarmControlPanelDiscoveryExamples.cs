using System;
using MBW.HassMQTT.DiscoveryModels.Models;
using Xunit;

namespace MBW.HassMQTT.Tests.DiscoverySerialization;

public class AlarmControlPanelDiscoveryExamples
{
    public static TheoryData<string, Type, string, bool> HaExamples => new()
    {
        {
            "Configuration",
            typeof(MqttAlarmControlPanel),
            """
            {
              "state_topic": "home/alarm",
              "command_topic": "home/alarm/set"
            }
            """,
            false
        },
        {
            "Configuration with partial feature support",
            typeof(MqttAlarmControlPanel),
            """
            {
              "name": "Alarm Panel",
              "supported_features": [
                "arm_home",
                "arm_away"
              ],
              "state_topic": "alarmdecoder/panel",
              "command_topic": "alarmdecoder/panel/set"
            }
            """,
            false
        },
        {
            "Configuration with local code validation",
            typeof(MqttAlarmControlPanel),
            """
            {
              "name": "Alarm Panel With Numeric Keypad",
              "state_topic": "alarmdecoder/panel",
              "value_template": "{{value_json.state}}",
              "command_topic": "alarmdecoder/panel/set",
              "code": "mys3cretc0de"
            }
            """,
            false
        },
        {
            "Remote code validation 1",
            typeof(MqttAlarmControlPanel),
            """
            {
              "name": "Alarm Panel With Text Code Dialog",
              "state_topic": "alarmdecoder/panel",
              "value_template": "{{ value_json.state }}",
              "command_topic": "alarmdecoder/panel/set",
              "code": "REMOTE_CODE_TEXT",
              "command_template": "{ \"action\": \"{{ action }}\", \"code\": \"{{ code }}\" }"
            }
            """,
            false
        },
        {
            "Remote code validation 2",
            typeof(MqttAlarmControlPanel),
            """
            {
              "name": "Alarm Panel With Numeric Keypad",
              "state_topic": "alarmdecoder/panel",
              "value_template": "{{ value_json.state }}",
              "command_topic": "alarmdecoder/panel/set",
              "code": "REMOTE_CODE",
              "command_template": "{ \"action\": \"{{ action }}\", \"code\": \"{{ code }}\" }"
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
