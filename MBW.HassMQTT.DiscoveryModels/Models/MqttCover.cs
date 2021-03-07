using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/cover.mqtt/
    /// </summary>
    [DeviceType(HassDeviceType.Cover)]
    [PublicAPI]
    public class MqttCover : MqttEntitySensorDiscoveryBase
    {
        public MqttCover(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// The MQTT topic to publish commands to control the cover.
        /// </summary>
        public string CommandTopic { get; set; }

        /// <summary>
        /// Sets the [class of the device](/integrations/cover/), changing the device state and icon that is displayed on the frontend.
        /// </summary>
        public string DeviceClass { get; set; }

        /// <summary>
        /// The name of the cover.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Flag that defines if switch works in optimistic mode.
        /// </summary>
        public string Optimistic { get; set; }

        /// <summary>
        /// The command payload that closes the cover.
        /// </summary>
        public string PayloadClose { get; set; }

        /// <summary>
        /// The command payload that opens the cover.
        /// </summary>
        public string PayloadOpen { get; set; }

        /// <summary>
        /// The command payload that stops the cover.
        /// </summary>
        public string PayloadStop { get; set; }

        /// <summary>
        /// Number which represents closed position.
        /// </summary>
        public int PositionClosed { get; set; }

        /// <summary>
        /// Number which represents open position.
        /// </summary>
        public int PositionOpen { get; set; }
        
        /// <summary>
        /// Defines a template that can be used to extract the payload for the `position_topic` topic.
        /// </summary>
        public string PositionTemplate { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive cover position messages.
        /// </summary>
        public string PositionTopic { get; set; }

        /// <summary>
        /// The maximum QoS level to be used when receiving and publishing messages.
        /// </summary>
        public MqttQosLevel Qos { get; set; }

        /// <summary>
        /// Defines if published messages should have the retain flag set.
        /// </summary>
        public bool Retain { get; set; }

        /// <summary>
        /// Defines a [template](/topics/templating/) to define the position to be sent to the `set_position_topic` topic. Incoming position value is available for use in the template `{{position}}`. If no template is defined, the position (0-100) will be calculated according to `position_open` and `position_closed` values.
        /// </summary>
        public string SetPositionTemplate { get; set; }

        /// <summary>
        /// The MQTT topic to publish position commands to. You need to set position_topic as well if you want to use position topic. Use template if position topic wants different values than within range `position_closed` - `position_open`. If template is not defined and `position_closed != 100` and `position_open != 0` then proper position value is calculated from percentage position.
        /// </summary>
        public string SetpositionTopic { get; set; }

        /// <summary>
        /// The payload that represents the closed state.
        /// </summary>
        public string StateClosed { get; set; }

        /// <summary>
        /// The payload that represents the closing state.
        /// </summary>
        public string StateClosing { get; set; }

        /// <summary>
        /// The payload that represents the open state.
        /// </summary>
        public string StateOpen { get; set; }

        /// <summary>
        /// The payload that represents the opening state.
        /// </summary>
        public string StateOpening { get; set; }
        
        /// <summary>
        /// The payload that represents the stopped state (for covers that do not report `open`/`closed` state).
        /// </summary>
        public string StateStopped { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive cover state messages. State topic can only read (`open`, `opening`, `closed`, `closing` or `stopped`) state.
        /// </summary>
        public string StateTopic { get; set; }

        /// <summary>
        /// The value that will be sent on a `close_cover_tilt` command.
        /// </summary>
        public int TiltClosedValue { get; set; }
        
        /// <summary>
        /// Defines a [template](/topics/templating/) that can be used to extract the payload for the `tilt_command_topic` topic.
        /// </summary>
        public string TiltCommandTemplate { get; set; }

        /// <summary>
        /// The MQTT topic to publish commands to control the cover tilt.
        /// </summary>
        public string TiltCommandTopic { get; set; }

        /// <summary>
        /// The maximum tilt value
        /// </summary>
        public int TiltMax { get; set; }

        /// <summary>
        /// The minimum tilt value.
        /// </summary>
        public int TiltMin { get; set; }

        /// <summary>
        /// The value that will be sent on an `open_cover_tilt` command.
        /// </summary>
        public int TiltOpenedValue { get; set; }

        /// <summary>
        /// Flag that determines if tilt works in optimistic mode.
        /// </summary>
        public bool TiltOptimistic { get; set; }

        /// <summary>
        /// Defines a [template](/topics/templating/) that can be used to extract the payload for the `tilt_status_topic` topic. 
        /// </summary>
        public string TiltStatusTemplate { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive tilt status update values.
        /// </summary>
        public string TiltStatusTopic { get; set; }

        /// <summary>
        /// Defines a template that can be used to extract the payload for the `state_topic` topic.
        /// </summary>
        public string ValueTemplate { get; set; }
    }
}