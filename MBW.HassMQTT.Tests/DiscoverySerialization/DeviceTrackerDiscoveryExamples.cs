using System;
using MBW.HassMQTT.DiscoveryModels.Models;
using Xunit;

namespace MBW.HassMQTT.Tests.DiscoverySerialization;

public class DeviceTrackerDiscoveryExamples
{
    public static TheoryData<string, Type, string, bool> HaExamples => new()
    {
        {
            "Configuration 1",
            typeof(MqttDeviceTracker),
            """
            {
              "name": "annetherese_n4",
              "state_topic": "location/annetherese"
            }
            """,
            false
        },
        {
            "Configuration 2",
            typeof(MqttDeviceTracker),
            """
            {
              "name": "paulus_oneplus",
              "state_topic": "location/paulus"
            }
            """,
            false
        },
        {
            "Using the discovery protocol 1",
            typeof(MqttDeviceTracker),
            """
            {
              "state_topic": "homeassistant/device_tracker/a4567d663eaf/state",
              "name": "My Tracker",
              "payload_home": "home",
              "payload_not_home": "not_home"
            }
            """,
            false
        },
        {
            "Using the discovery protocol 2",
            typeof(MqttDeviceTracker),
            """
            {
              "json_attributes_topic": "homeassistant/device_tracker/a4567d663eaf/attributes",
              "name": "My Tracker"
            }
            """,
            false
        },
        {
            "YAML configuration",
            typeof(MqttDeviceTracker),
            """
            {
              "name": "My Tracker",
              "state_topic": "a4567d663eaf/state",
              "payload_home": "home",
              "payload_not_home": "not_home"
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
