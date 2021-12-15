#nullable enable
using FluentValidation;

namespace MBW.HassMQTT.DiscoveryModels.Availability
{
    public class AvailabilityModel
    {
        public static AvailabilityModelValidator Validator { get; } = new AvailabilityModelValidator();

        /// <summary>
        /// An MQTT topic subscribed to receive availability (online/offline) updates.
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// The payload that represents the available state.
        /// </summary>
        /// <remarks>Default is 'online'</remarks>
        public string? PayloadAvailable { get; set; }

        /// <summary>
        /// The payload that represents the unavailable state.
        /// </summary>
        /// <remarks>Default is 'offline'</remarks>
        public string? PayloadNotAvailable { get; set; }

        /// <summary>
        /// Defines a template to extract device’s availability from the topic. To determine the devices’s availability result of this template will be compared to `payload_available` and `payload_not_available`.
        /// </summary>
        public string? ValueTemplate { get; set; }

        public class AvailabilityModelValidator : AbstractValidator<AvailabilityModel>
        {
            public AvailabilityModelValidator()
            {
                RuleFor(s => s.Topic).NotEmpty();
            }
        }
    }
}