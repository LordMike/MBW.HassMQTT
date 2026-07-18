using System;
using MBW.HassMQTT.DiscoveryModels.Models;
using Xunit;

namespace MBW.HassMQTT.Tests.DiscoverySerialization;

public class SensorDiscoveryExamples
{
    public static TheoryData<string, Type, string, bool> HaExamples => new()
    {
        {
            "Configuration",
            typeof(MqttSensor),
            """
            {
              "name": "Bedroom Temperature",
              "state_topic": "home/bedroom/temperature"
            }
            """,
            false
        },
        {
            "Processing Unix EPOCH timestamps",
            typeof(MqttSensor),
            """
            {
              "name": "turned on",
              "state_topic": "pump/timestamp_on",
              "device_class": "timestamp",
              "value_template": "{{ as_datetime(value) }}",
              "unique_id": "hp_1231232_ts_on",
              "device": {
                "name": "Heat pump",
                "identifiers": [
                  "hp_1231232"
                ]
              }
            }
            """,
            false
        },
        {
            "JSON attributes topic configuration",
            typeof(MqttSensor),
            """
            {
              "name": "RSSI",
              "state_topic": "home/sensor1/infojson",
              "unit_of_measurement": "dBm",
              "value_template": "{{ value_json.RSSI }}",
              "availability": [
                {
                  "topic": "home/sensor1/status"
                }
              ],
              "payload_available": "online",
              "payload_not_available": "offline",
              "json_attributes_topic": "home/sensor1/attributes"
            }
            """,
            false
        },
        {
            "JSON attributes template configuration for Timer 1",
            typeof(MqttSensor),
            """
            {
              "name": "Timer 1",
              "state_topic": "tele/sonoff/sensor",
              "value_template": "{{ value_json.Timer1.Arm }}",
              "json_attributes_topic": "tele/sonoff/sensor",
              "json_attributes_template": "{{ value_json.Timer1 | tojson }}"
            }
            """,
            false
        },
        {
            "JSON attributes template configuration for Timer 2",
            typeof(MqttSensor),
            """
            {
              "name": "Timer 2",
              "state_topic": "tele/sonoff/sensor",
              "value_template": "{{ value_json.Timer2.Arm }}",
              "json_attributes_topic": "tele/sonoff/sensor",
              "json_attributes_template": "{{ value_json.Timer2 | tojson }}"
            }
            """,
            false
        },
        {
            "Usage of `entity_id` in the template",
            typeof(MqttSensor),
            """
            {
              "name": "Temp 1",
              "state_topic": "sensor/temperature",
              "value_template": "{% if states(entity_id) == None %}\n  {{ value | round(2) }}\n{% else %}\n  {{ value | round(2) * 0.9 + states(entity_id) * 0.1 }}\n{% endif %}"
            }
            """,
            false
        },
        {
            "Owntracks battery level sensor",
            typeof(MqttSensor),
            """
            {
              "name": "Battery Tablet",
              "state_topic": "owntracks/tablet/tablet",
              "unit_of_measurement": "%",
              "value_template": "{{ value_json.batt }}"
            }
            """,
            false
        },
        {
            "Temperature sensor",
            typeof(MqttSensor),
            """
            {
              "name": "Temperature",
              "state_topic": "office/sensor1",
              "suggested_display_precision": 1,
              "unit_of_measurement": "°C",
              "value_template": "{{ value_json.temperature }}"
            }
            """,
            false
        },
        {
            "Humidity sensor",
            typeof(MqttSensor),
            """
            {
              "name": "Humidity",
              "state_topic": "office/sensor1",
              "unit_of_measurement": "%",
              "value_template": "{{ value_json.humidity }}"
            }
            """,
            false
        },
        {
            "Get sensor value from a device with ESPEasy",
            typeof(MqttSensor),
            """
            {
              "name": "Brightness",
              "state_topic": "home/bathroom/analog/brightness"
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
