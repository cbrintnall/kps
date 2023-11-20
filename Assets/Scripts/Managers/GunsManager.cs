using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Singleton]
public class GunManager : MonoBehaviour
{
    private Dictionary<string, GunDefinition> Guns = new();

    void Awake()
    {
        Guns = JsonConvert.DeserializeObject<Dictionary<string, GunDefinition>>(
            Resources.Load<TextAsset>("guns").text
        );
    }

    public bool TryGetGun(string key, out GunDefinition gun)
    {
        var hasGun = Guns.TryGetValue(key, out GunDefinition g);
        gun = g;
        return hasGun;
    }
}
