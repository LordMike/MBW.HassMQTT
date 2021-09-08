namespace MBW.HassMQTT.DiscoveryModels.Interfaces
{
    public interface IHasRetain
    {
        /// <summary>
        /// If the published message should have the retain flag on or not.
        /// </summary>
        public bool? Retain { get; set; }
    }
}