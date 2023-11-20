using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class StatFloatConverter : JsonConverter<StatFloat>
{
    public override StatFloat ReadJson(
        JsonReader reader,
        Type objectType,
        StatFloat existingValue,
        bool hasExistingValue,
        JsonSerializer serializer
    )
    {
        float min = 0.0f;
        float max = float.PositiveInfinity;
        float value = 0.0f;

        if (reader.Value != null && reader.Value is double dbl)
        {
            value = Convert.ToSingle(dbl);
            return new StatFloat(value, max, min);
        }

        var obj = JObject.Load(reader);

        if (obj.ContainsKey("base"))
        {
            value = Convert.ToSingle(obj["base"]);
        }

        if (obj.ContainsKey("min"))
        {
            min = Convert.ToSingle(obj["min"]);
        }

        if (obj.ContainsKey("max"))
        {
            max = Convert.ToSingle(obj["max"]);
        }

        var stat = new StatFloat(value, max, min);
        return stat;
    }

    public override void WriteJson(JsonWriter writer, StatFloat value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("min");
        writer.WriteValue(GetProperty(value.Min));
        writer.WritePropertyName("max");
        writer.WriteValue(GetProperty(value.Max));
        writer.WritePropertyName("base");
        writer.WriteValue(GetProperty(value.Base));
        writer.WriteEndObject();
    }

    private double GetProperty(float property)
    {
        if (property == float.PositiveInfinity)
        {
            property = float.MaxValue;
        }

        return Convert.ToDouble(property);
    }
}

public class StatIntConverter : JsonConverter<StatInt>
{
    public override StatInt ReadJson(
        JsonReader reader,
        Type objectType,
        StatInt existingValue,
        bool hasExistingValue,
        JsonSerializer serializer
    )
    {
        int min = 0;
        int max = int.MaxValue;
        int value = 0;

        if (reader.Value != null && reader.Value is long dbl)
        {
            value = Convert.ToInt32(dbl);
            return new StatInt(value, max, min);
        }

        var obj = JObject.Load(reader);

        if (obj.ContainsKey("base"))
        {
            value = Convert.ToInt32(obj["base"]);
        }

        if (obj.ContainsKey("min"))
        {
            min = Convert.ToInt32(obj["min"]);
        }

        if (obj.ContainsKey("max"))
        {
            max = Convert.ToInt32(obj["max"]);
        }

        var stat = new StatInt(value, max, min);
        return stat;
    }

    public override void WriteJson(JsonWriter writer, StatInt value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("min");
        writer.WriteValue(value.Min);
        writer.WritePropertyName("max");
        writer.WriteValue(value.Max);
        writer.WritePropertyName("base");
        writer.WriteValue(value.Base);
        writer.WriteEndObject();
    }
}
