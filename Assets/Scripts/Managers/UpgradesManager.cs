using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IngameDebugConsole;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Localization.SmartFormat;
using KaimiraGames;

public class UpgradeSettings
{
    public Dictionary<UpgradeRarity, int> Chances;
}

public class UpgradeDefinition
{
    public string Prefix;
    public string Name;
    public string Description;
    public Type Class;
    public Type Behavior = typeof(UpgradeBehavior);
    public UpgradeType Type;
    public UpgradeTierDefinition[] Tiers;

    public UpgradeData[] ToData()
    {
        var tiers = Tiers
            .Select(tier =>
            {
                var data = new UpgradeData()
                {
                    Prefix = Prefix,
                    Name = Name,
                    Description = Smart.Format(Description, tier.Stats),
                    Class = Class,
                    Type = Type,
                    Rarity = tier.Rarity,
                    Stats = tier.Stats,
                    Cost = tier.Cost,
                };

                data.Behavior = Activator.CreateInstance(Behavior, data) as UpgradeBehavior;

                return data;
            })
            .ToArray();

        for (int i = 0; i < tiers.Length - 1; i++)
        {
            tiers[i].Next = tiers[i + 1];
        }

        return tiers;
    }
}

public class UpgradeTierDefinition
{
    public UpgradeRarity Rarity = UpgradeRarity.COMMON;
    public Dictionary<string, object> Stats;
    public int Cost;
}

[Singleton]
public class UpgradesManager : MonoBehaviour, IReloadable
{
    public WeightedList<UpgradeData> RoundData { get; private set; }
    private Dictionary<string, UpgradeData> upgrades = new();
    private UpgradeSettings settings = new();

    [ConsoleMethod("ls", "Print all upgrades")]
    public static void PrintUpgrades()
    {
        Debug.Log(
            string.Join(
                ',',
                SingletonLoader.Get<UpgradesManager>().upgrades.Select(upgrade => upgrade.Key)
            )
        );
    }

    [ConsoleMethod("gu", "Gives an upgrade")]
    public static void GiveUpgrade(string name, int count = 1)
    {
        Type type = Assembly.GetExecutingAssembly().GetType(name);
        UpgradesManager upgradesManager = SingletonLoader.Get<UpgradesManager>();

        for (int i = 0; i < count; i++)
        {
            if (upgradesManager.TryGetUpgrade(name, out var data))
            {
                PlayerEquipmentController.Instance.AddUpgrade(data);
            }
            else
            {
                Debug.LogWarning($"Couldn't give upgrade {name}");
            }
        }
    }

    [ConsoleMethod("reset", "Re-ingests the upgrades file")]
    public static void Reset()
    {
        SingletonLoader.Get<UpgradesManager>().IngestData();
    }

    public bool TryGetUpgrade(string name, out UpgradeData data)
    {
        data = null;

        if (upgrades.TryGetValue(name, out var value))
        {
            data = value;
            return true;
        }

        return false;
    }

    public void IngestData()
    {
        var time = DateTime.Now;

        var data = Resources.Load<TextAsset>("Upgrades/definitions");
        var intermediate = JsonConvert.DeserializeObject<UpgradeDefinition[]>(data.text);

        Debug.Log($"Loading upgrade data: {data.text}");

        upgrades = new();
        foreach (var definition in intermediate)
        {
            var generated = definition.ToData();
            Assert.AreEqual(generated.Length, 4, "Should be 4 entries, one for each tier.");
            foreach (var upgrade in generated)
            {
                Assert.IsFalse(upgrades.ContainsKey(upgrade.GenerateId()), "Clash for upgrade key");
                upgrades.Add(upgrade.GenerateId(), upgrade);
            }
        }

        Debug.Log(
            $"Loaded {upgrades.Count} upgrades in {(DateTime.Now - time).Milliseconds}ms, ids=[{string.Join("\n-", upgrades.Keys)}]."
        );

        var settingsData = Resources.Load<TextAsset>("Upgrades/settings");
        settings = JsonConvert.DeserializeObject<UpgradeSettings>(settingsData.text);
    }

    void Awake()
    {
        IngestData();
        UpdateUpgradeList();
    }

    public UpgradeData RequestUpgradeType()
    {
        return upgrades.Values.Random();
    }

    /// <summary>
    /// Provides a filtered list of all valid upgrades
    /// </summary>
    /// <returns>The list of upgrades</returns>
    public List<UpgradeData> RetrieveCandidates()
    {
        return upgrades.Values.Where(value => value.Behavior.CanBeSold()).ToList();
    }

    /// <summary>
    /// Gives an upgrade if there are any left in the current pool, this will pretty
    /// much always return true, but there are valid cases where it can be false.
    /// </summary>
    /// <param name="upgrade">If possible, the upgrade it retrieved</param>
    /// <returns>true if an upgrade was found, false otherwise</returns>
    public bool TryGetViableUpgrade(out UpgradeData upgrade)
    {
        upgrade = RoundData?.Next();

        if (upgrade != null)
        {
            RoundData.Remove(upgrade);
        }

        return upgrade != null;
    }

    public void UpdateUpgradeList()
    {
        var candidates = RetrieveCandidates()
            .Select(
                upgrade =>
                    new WeightedListItem<UpgradeData>(upgrade, settings.Chances[upgrade.Rarity])
            )
            .ToList();

        RoundData = new() { candidates };
        Debug.Log($"Shuffled weighted list, upgrades #{RoundData.Count}");
    }

    public void Reload()
    {
        IngestData();
    }
}
