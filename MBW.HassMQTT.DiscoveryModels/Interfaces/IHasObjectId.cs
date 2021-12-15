namespace MBW.HassMQTT.DiscoveryModels.Interfaces
{
    public interface IHasObjectId
    {
        /// <summary>
        /// Used instead of name for automatic generation of entity_id
        /// </summary>
        public string? ObjectId { get; set; }
    }
}