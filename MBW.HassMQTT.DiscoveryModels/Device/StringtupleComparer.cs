using System.Collections.Generic;

namespace MBW.HassMQTT.DiscoveryModels.Device
{
    internal class StringtupleComparer : IEqualityComparer<(string, string)>
    {
        public static StringtupleComparer Instance { get; } = new();

        private StringtupleComparer()
        {
        }

        public bool Equals((string, string) x, (string, string) y)
        {
            return x.Item1 == y.Item1 && x.Item2 == y.Item2;
        }

        public int GetHashCode((string, string) obj)
        {
            unchecked
            {
                return (obj.Item1.GetHashCode() * 397) ^ obj.Item2.GetHashCode();
            }
        }
    }
}