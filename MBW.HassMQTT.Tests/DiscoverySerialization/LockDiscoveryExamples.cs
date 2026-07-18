using System;
using MBW.HassMQTT.DiscoveryModels.Models;
using Xunit;

namespace MBW.HassMQTT.Tests.DiscoverySerialization;

public class LockDiscoveryExamples
{
    public static TheoryData<string, Type, string, bool> HaExamples => new()
    {
        {
            "Configuration",
            typeof(MqttLock),
            """
            {
              "command_topic": "home/frontdoor/set"
            }
            """,
            false
        },
        {
            "Full configuration",
            typeof(MqttLock),
            """
            {
              "name": "Frontdoor",
              "state_topic": "home-assistant/frontdoor/state",
              "code_format": "^\\d{4}$",
              "command_topic": "home-assistant/frontdoor/set",
              "command_template": "{ \"action\": \"{{ value }}\", \"code\":\"{{ code }}\" }",
              "payload_lock": "LOCK",
              "payload_unlock": "UNLOCK",
              "state_locked": "LOCK",
              "state_unlocked": "UNLOCK",
              "state_locking": "LOCKING",
              "state_unlocking": "UNLOCKING",
              "state_jammed": "MOTOR_JAMMED",
              "optimistic": false,
              "qos": 1,
              "retain": true,
              "value_template": "{{ value.x }}"
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
