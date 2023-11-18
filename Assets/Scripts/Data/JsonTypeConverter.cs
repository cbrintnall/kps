using System;
using System.Reflection;
using Newtonsoft.Json;

class TypeConverter : JsonConverter<Type>
{
    public override Type ReadJson(
        JsonReader reader,
        Type objectType,
        Type existingValue,
        bool hasExistingValue,
        JsonSerializer serializer
    )
    {
        if (reader.Value is string str)
        {
            return Assembly.GetExecutingAssembly().GetType(str);
        }

        return null;
    }

    public override void WriteJson(JsonWriter writer, Type value, JsonSerializer serializer)
    {
        writer.WriteValue(value.Name);
    }
}
