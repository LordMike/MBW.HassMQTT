using System;
using Newtonsoft.Json.Linq;

namespace MBW.HassMQTT.DiscoveryModels.Helpers
{
    internal static class JsonHelper
    {
        public static T GetOrDefault<T>(this JObject obj, string name, T @default)
        {
            if (!obj.TryGetValue(name, out JToken val) || val == null)
                return @default;

            return val.Value<T>();
        }

        public static void SetIfChanged<T>(this JObject obj, string name, T newValue, Action onSet)
        {
            if (Equals(newValue, default(T)))
            {
                // Remove a value
                if (obj.Remove(name))
                    onSet();

                return;
            }

            void Overwrite()
            {
                obj[name] = JToken.FromObject(newValue);
                onSet();
            }

            if (obj.TryGetValue(name, out JToken val) && val != null)
            {
                // Compare
                if (newValue is Array newArray && val is JArray existingArray)
                {
                    if (newArray.Length != existingArray.Count)
                    {
                        Overwrite();
                        return;
                    }

                    // Compare each value
                    for (int i = 0; i < newArray.Length; i++)
                    {
                        // TODO: Other types
                        if (!ComparisonHelper.IsSameValue(newArray.GetValue(i), existingArray.Value<string>(i)))
                        {
                            Overwrite();
                            return;
                        }
                    }

                    // No difference
                }
                else
                {
                    T existing = val.Value<T>();

                    if (!ComparisonHelper.IsSameValue(existing, newValue))
                    {
                        Overwrite();
                        return;
                    }
                }
            }
            else
            {
                // Not previously set
                Overwrite();
                return;
            }
        }
    }
}