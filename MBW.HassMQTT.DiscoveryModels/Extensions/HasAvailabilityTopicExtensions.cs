using System;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;

namespace MBW.HassMQTT.DiscoveryModels.Extensions
{
    [PublicAPI]
    public static class HasAvailabilityTopicExtensions
    {
        public static IHasAvailabilityTopic RemoveAvailability(this IHasAvailabilityTopic discovery, string topic)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            for (int i = 0; i < discovery.Availability.Count; i++)
            {
                if (discovery.Availability[i].Topic.Equals(topic, StringComparison.Ordinal))
                    discovery.Availability.RemoveAt(i--);
            }
#pragma warning restore CS0618 // Type or member is obsolete

            return discovery;
        }

        public static IHasAvailabilityTopic ConfigureAvailability(this IHasAvailabilityTopic discovery, string topic, string payloadAvailable, string payloadNotAvailable, Action<AvailabilityModel> configure = null)
        {
            discovery.RemoveAvailability(topic);

#pragma warning disable CS0618 // Type or member is obsolete
            discovery.Availability.Add(new AvailabilityModel
            {
                Topic = topic,
                PayloadAvailable = payloadAvailable,
                PayloadNotAvailable = payloadNotAvailable
            });
#pragma warning restore CS0618 // Type or member is obsolete

            return discovery;
        }
    }
}