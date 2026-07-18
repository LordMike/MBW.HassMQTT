using System;
using MBW.HassMQTT.DiscoveryModels.Models;
using Xunit;

namespace MBW.HassMQTT.Tests.DiscoverySerialization;

public class CoverDiscoveryExamples
{
    public static TheoryData<string, Type, string, bool> HaExamples => new()
    {
        {
            "Configuration",
            typeof(MqttCover),
            """
            {
              "command_topic": "living-room-cover/set"
            }
            """,
            false
        },
        {
            "Full configuration state topic without tilt",
            typeof(MqttCover),
            """
            {
              "name": "MQTT Cover",
              "command_topic": "living-room-cover/set",
              "state_topic": "living-room-cover/state",
              "availability": [
                {
                  "topic": "living-room-cover/availability"
                }
              ],
              "qos": 0,
              "retain": true,
              "payload_open": "OPEN",
              "payload_close": "CLOSE",
              "payload_stop": "STOP",
              "state_open": "open",
              "state_opening": "opening",
              "state_closed": "closed",
              "state_closing": "closing",
              "payload_available": "online",
              "payload_not_available": "offline",
              "optimistic": false,
              "value_template": "{{ value.x }}"
            }
            """,
            false
        },
        {
            "Full configuration position topic without tilt",
            typeof(MqttCover),
            """
            {
              "name": "MQTT Cover",
              "command_topic": "living-room-cover/set",
              "position_topic": "living-room-cover/position",
              "availability": [
                {
                  "topic": "living-room-cover/availability"
                }
              ],
              "set_position_topic": "living-room-cover/set_position",
              "qos": 0,
              "retain": true,
              "payload_open": "OPEN",
              "payload_close": "CLOSE",
              "payload_stop": "STOP",
              "position_open": 100,
              "position_closed": 0,
              "payload_available": "online",
              "payload_not_available": "offline",
              "optimistic": false,
              "value_template": "{{ value.x }}"
            }
            """,
            false
        },
        {
            "Full configuration for position, state and tilt",
            typeof(MqttCover),
            """
            {
              "name": "MQTT Cover",
              "command_topic": "living-room-cover/set",
              "state_topic": "living-room-cover/state",
              "position_topic": "living-room-cover/position",
              "availability": [
                {
                  "topic": "living-room-cover/availability"
                }
              ],
              "qos": 0,
              "retain": true,
              "payload_open": "OPEN",
              "payload_close": "CLOSE",
              "payload_stop": "STOP",
              "state_open": "open",
              "state_opening": "opening",
              "state_closed": "closed",
              "state_closing": "closing",
              "payload_available": "online",
              "payload_not_available": "offline",
              "optimistic": false,
              "value_template": "{{ value.x }}",
              "position_template": "{{ value.y }}",
              "tilt_command_topic": "living-room-cover/tilt",
              "tilt_status_topic": "living-room-cover/tilt-state",
              "tilt_status_template": "{{ value_json[\"PWM\"][\"PWM1\"] }}",
              "tilt_min": 0,
              "tilt_max": 180,
              "tilt_closed_value": 70,
              "tilt_opened_value": 180
            }
            """,
            false
        },
        {
            "Full configuration using stopped state",
            typeof(MqttCover),
            """
            {
              "name": "MQTT Cover",
              "command_topic": "living-room-cover/set",
              "state_topic": "living-room-cover/state",
              "position_topic": "living-room-cover/position",
              "availability": [
                {
                  "topic": "living-room-cover/availability"
                }
              ],
              "qos": 0,
              "retain": true,
              "payload_open": "OPEN",
              "payload_close": "CLOSE",
              "payload_stop": "STOP",
              "state_opening": "opening",
              "state_closed": "closed",
              "state_stopped": "stopped",
              "payload_available": "online",
              "payload_not_available": "offline",
              "optimistic": false,
              "value_template": "{{ value.x }}",
              "position_template": "{{ value.y }}"
            }
            """,
            false
        },
        {
            "Configuration for disabling cover commands 1",
            typeof(MqttCover),
            """
            {
              "payload_open": "on",
              "payload_close": null,
              "payload_stop": "on"
            }
            """,
            true
        },
        {
            "Configuration for disabling cover commands 2",
            typeof(MqttCover),
            """
            {
              "payload_open": "on",
              "payload_close": null,
              "payload_stop": "on"
            }
            """,
            true
        },
        {
            "Full configuration using `entity_id`- variable in the template",
            typeof(MqttCover),
            """
            {
              "name": "MQTT Cover",
              "command_topic": "living-room-cover/set",
              "state_topic": "living-room-cover/state",
              "position_topic": "living-room-cover/position",
              "set_position_topic": "living-room-cover/position/set",
              "payload_open": "open",
              "payload_close": "close",
              "payload_stop": "stop",
              "state_opening": "open",
              "state_closing": "close",
              "state_stopped": "stop",
              "optimistic": false,
              "position_template": "{% if not state_attr(entity_id, \"current_position\") %}\n  {{ value }}\n{% elif state_attr(entity_id, \"current_position\") < (value | int) %}\n  {{ (value | int + 1) }}\n{% elif state_attr(entity_id, \"current_position\") > (value | int) %}\n  {{ (value | int - 1) }}\n{% else %}\n  {{ value }}\n{% endif %}"
            }
            """,
            false
        },
        {
            "Full configuration using advanced templating (duplicate payload keys use the final documented values)",
            typeof(MqttCover),
            """
            {
              "name": "MQTT Cover",
              "command_topic": "living-room-cover/set",
              "state_topic": "living-room-cover/state",
              "position_topic": "living-room-cover/position",
              "set_position_topic": "living-room-cover/position/set",
              "tilt_command_topic": "living-room-cover/position/set",
              "qos": 1,
              "retain": false,
              "state_opening": "open",
              "state_closing": "close",
              "state_stopped": "stop",
              "position_open": 100,
              "position_closed": 0,
              "tilt_min": 0,
              "tilt_max": 6,
              "tilt_opened_value": 3,
              "tilt_closed_value": 0,
              "optimistic": false,
              "position_template": "{% if not state_attr(entity_id, \"current_position\") %}\n  {\n    \"position\" : {{ value }},\n    \"tilt_position\" : 0\n  }\n{% else %}\n  {% set old_position = state_attr(entity_id, \"current_position\") %}\n  {% set old_tilt_percent = (state_attr(entity_id, \"current_tilt_position\")) %}\n\n  {% set movement = value | int - old_position %}\n  {% set old_tilt_position = (old_tilt_percent / 100 * (tilt_max - tilt_min)) %}\n  {% set new_tilt_position = min(max((old_tilt_position + movement), tilt_min), tilt_max) %}\n\n  {\n    \"position\": {{ value }},\n    \"tilt_position\": {{ new_tilt_position }}\n  }\n{% endif %}",
              "tilt_command_template": "{% set position = state_attr(entity_id, \"current_position\") %} {% set tilt = state_attr(entity_id, \"current_tilt_position\") %} {% set movement = (tilt_position - tilt) / 100 * tilt_max %} {{ position + movement }}",
              "payload_open": "on",
              "payload_close": null,
              "payload_stop": "on"
            }
            """,
            true
        },
    };

    [Theory]
    [MemberData(nameof(HaExamples))]
    public void DocumentationExamplesRoundTrip(string name, Type modelType, string json, bool normalizeExplicitNulls)
    {
        DiscoveryJsonRoundTrip.Assert(name, modelType, json, normalizeExplicitNulls);
    }
}
