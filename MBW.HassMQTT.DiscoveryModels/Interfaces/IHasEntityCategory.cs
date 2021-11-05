#nullable enable

namespace MBW.HassMQTT.DiscoveryModels.Interfaces
{
    public interface IHasEntityCategory
    {
        /// <summary>
        /// The category of the entity.
        ///
        /// See https://developers.home-assistant.io/docs/core/entity#generic-properties.
        /// </summary>
        public EntityCategory? EntityCategory { get; set; }
    }
}