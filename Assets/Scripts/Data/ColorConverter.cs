using System;
using Newtonsoft.Json;
using UnityEngine;

public class ColorConverter : JsonConverter<Color>
{
    public override Color ReadJson(
        JsonReader reader,
        Type objectType,
        Color existingValue,
        bool hasExistingValue,
        JsonSerializer serializer
    )
    {
        if (reader.Value is string str)
        {
            if (ColorUtility.TryParseHtmlString(str, out Color clr))
            {
                return clr;
            }
        }

        return Color.magenta;
    }

    public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
