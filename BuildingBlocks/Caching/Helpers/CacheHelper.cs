using Newtonsoft.Json;

namespace Caching.Helpers;

public static class CacheHelper
{
    internal static string Serialize(object? o)
    {
        return o == null ? string.Empty : JsonConvert.SerializeObject(o, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });
    }

    internal static T Deserialize<T>(string json)
    {
        return (string.IsNullOrWhiteSpace(json)
            ? default
            : JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }))!;
    }
}