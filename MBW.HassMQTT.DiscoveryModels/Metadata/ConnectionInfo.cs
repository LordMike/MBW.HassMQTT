namespace MBW.HassMQTT.DiscoveryModels.Metadata
{
    public class ConnectionInfo
    {
        public string Type { get; set; }

        public string Value { get; set; }

        public static implicit operator ConnectionInfo((string, string) val)
        {
            return new ConnectionInfo
            {
                Type = val.Item1,
                Value = val.Item2
            };
        }
    }
}