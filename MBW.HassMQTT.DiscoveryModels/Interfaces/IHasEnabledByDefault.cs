namespace MBW.HassMQTT.DiscoveryModels.Interfaces
{
    public interface IHasEnabledByDefault
    {
        /// <summary>
        /// Flag which defines if the entity should be enabled when first added.
        /// </summary>
        public bool? EnabledByDefault { get; set; }
    }
}