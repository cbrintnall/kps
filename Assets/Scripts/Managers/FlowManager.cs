using System.Collections.Generic;
using IngameDebugConsole;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PowerupStats
{
    public int HealAmount;
    public float ExplosionSize;
    public int ExplosionDamage;
    public float SpeedTime;
    public float SpeedMultiplier;
}

public class GameData
{
    public Dictionary<PowerupType, int> PowerupWeights;
    public PowerupStats PowerupStats;
    public Dictionary<UpgradeRarity, Color> UpgradeColors;
}

[Singleton]
public class FlowManager : MonoBehaviour
{
    public GameData GameData;

    [ConsoleMethod("reload", "Reloads the round")]
    public static void Restart()
    {
        SceneManager.LoadScene("ObsidianLibrary");
    }

    void Awake()
    {
        GameData = JsonConvert.DeserializeObject<GameData>(
            Resources.Load<TextAsset>("settings").text
        );
    }
}
