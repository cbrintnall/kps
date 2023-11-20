using System;
using Newtonsoft.Json;
using UnityEngine;

class GameObjectConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        Debug.Log($"Type: {objectType.Name}");

        return objectType == typeof(GameObject) || objectType.IsSubclassOf(typeof(MonoBehaviour));
    }

    public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer
    )
    {
        if (reader.Value is string str)
        {
            return Resources.Load(str, objectType);
        }

        return null;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
