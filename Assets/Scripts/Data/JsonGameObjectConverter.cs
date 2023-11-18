using System;
using Newtonsoft.Json;
using UnityEngine;

class GameObjectConverter : JsonConverter<GameObject>
{
    public override GameObject ReadJson(
        JsonReader reader,
        Type objectType,
        GameObject existingValue,
        bool hasExistingValue,
        JsonSerializer serializer
    )
    {
        if (reader.Value is string str)
        {
            return Resources.Load<GameObject>(str);
        }

        return null;
    }

    public override void WriteJson(JsonWriter writer, GameObject value, JsonSerializer serializer)
    {
        throw new InvalidOperationException("Can't write out gameobject");
    }
}
