using System;
using System.Collections.Generic;

namespace MBW.HassMQTT.DiscoveryModels.Availability
{
    public interface IHasAvailabilityTopic
    {
        /// <summary>
        /// A list of MQTT topics subscribed to receive availability (online/offline) updates. Must not be used together with availability_topic.
        /// </summary>
        [Obsolete("Use ConfigureAvailability extension method instead of this directly - direct use can lead to bugs")]
        IList<AvailabilityModel> Availability { get; set; }

        /// <summary>
        /// When `availability` is configured, this controls the conditions needed to set the entity to `available`.
        /// Valid entries are `all`, `any`, and `latest`. If set to `all`, `payload_available` must be received on all
        /// configured availability topics before the entity is marked as online. If set to `any`, `payload_available`
        /// must be received on at least one configured availability topic before the entity is marked as online.
        ///
        /// If set to `latest`, the last `payload_available` or `payload_not_available` received on any configured availability
        /// topic controls the availability.
        /// </summary>
        /// <remarks>Default is Latest</remarks>
        AvailabilityMode? AvailabilityMode { get; set; }
    }
}
