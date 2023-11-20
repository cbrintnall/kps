using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public static class ConverterLoader
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void Setup()
    {
        JsonConvert.DefaultSettings = () =>
            new JsonSerializerSettings()
            {
                Converters = new List<JsonConverter>()
                {
                    new TypeConverter(),
                    new GameObjectConverter(),
                    new ColorConverter(),
                    new Vector3Converter(),
                    new StatFloatConverter(),
                    new StatIntConverter()
                }
            };
    }
}
