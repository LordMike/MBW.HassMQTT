using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace MBW.HassMQTT.Serialization;

internal static class DiscoverySnapshotSerializer
{
    private static JsonSerializer CreateSerializer()
    {
        SnakeCaseNamingStrategy namingStrategy = new SnakeCaseNamingStrategy();
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            DefaultValueHandling = DefaultValueHandling.Include,
            ObjectCreationHandling = ObjectCreationHandling.Auto,
            ContractResolver = new OptionalAwareContractResolver { NamingStrategy = namingStrategy }
        };

        settings.Converters.Add(new OptionalJsonConverter());
        settings.Converters.Add(new StringEnumConverter(namingStrategy));
        return JsonSerializer.Create(settings);
    }

    public static byte[] Capture(object value)
    {
        using MemoryStream stream = new MemoryStream();
        using (StreamWriter textWriter = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
        using (JsonTextWriter jsonWriter = new JsonTextWriter(textWriter))
        {
            CreateSerializer().Serialize(jsonWriter, value);
            jsonWriter.Flush();
        }

        return stream.ToArray();
    }

    public static void Populate(byte[] snapshot, object target)
    {
        using MemoryStream stream = new MemoryStream(snapshot, false);
        using StreamReader textReader = new StreamReader(stream, Encoding.UTF8);
        using JsonTextReader jsonReader = new JsonTextReader(textReader);
        CreateSerializer().Populate(jsonReader, target);
    }
}
