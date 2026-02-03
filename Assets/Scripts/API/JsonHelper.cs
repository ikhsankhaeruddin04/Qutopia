using System.Collections.Generic;
using Newtonsoft.Json;

public static class JsonHelper
{
    public static string ToJson<T>(T[] array, bool prettyPrint = false)
    {
        var settings = new JsonSerializerSettings
        {
            Formatting = prettyPrint ? Formatting.Indented : Formatting.None
        };
        return JsonConvert.SerializeObject(array, settings);
    }

    public static List<T> FromJsonArray<T>(string json)
    {
        return JsonConvert.DeserializeObject<List<T>>(json);
    }
}