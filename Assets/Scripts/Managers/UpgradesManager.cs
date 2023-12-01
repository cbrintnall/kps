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
using Sirenix.Utilities;

public class UpgradeSettings
{
    public Dictionary<UpgradeRarity, int> Chances;
}

[AttributeUsage(AttributeTargets.Field)]
public class StatUpgradeDirective : Attribute
{
    public int[] Values;
    public string[] Requires;
    public bool IsPercent = true;
    public string Name;
    public string StatName;
    public string Description =
        "{Amount:cond:<0:Decreases|Increases} {StatName} by {IsPercentage:{Amount}%|{Amount}}.";
}

public class UpgradeGroup
{
    public Type Behavior = typeof(UpgradeBehavior);
    protected UpgradeDefinition[] Definitions;

    public UpgradeDefinition[] GetDefinitions =>
        Definitions
            .Select(def =>
            {
                def.Behavior = Behavior;
                return def;
            })
            .ToArray();
}

public interface IUpgrade
{
    void Apply(PlayerEquipmentController controller, UpgradeData data);
}

public class UpgradeDefinition
{
    public string Prefix;
    public string Name;
    public string Description;
    public string[] Requires;
    public Type Class;
    public Type Behavior = typeof(UpgradeBehavior);
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
                    Rarity = tier.Rarity,
                    Stats = tier.Stats,
                    Cost = tier.Cost,
                    Requires = Requires
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

[@Singleton]
public class UpgradesManager : MonoBehaviour, IReloadable
{
    public WeightedList<UpgradeData> RoundData { get; private set; }
    public string[] Prefixes => upgrades.Keys.Select(key => key.Split('.')[0]).ToArray();
    private Dictionary<string, UpgradeData> upgrades = new();
    private UpgradeSettings settings = new();

    [ConsoleMethod("ls", "Print all upgrades")]
    public static void PrintUpgrades()
    {
        Debug.Log(
            string.Join(
                "\n-",
                SingletonLoader.Get<UpgradesManager>().upgrades.Select(upgrade => upgrade.Key)
            )
        );
    }

    [ConsoleMethod("getval", "Outputs the current value of an upgrade")]
    public static void GetValue(string name)
    {
        if (PlayerEquipmentController.Instance.TryGetUpgrade(name, out var data))
        {
            Debug.Log(data);
        }
        else
        {
            Debug.LogWarning($"Player doesn't have upgrade {name}");
        }
    }

    [ConsoleMethod("stat", "Gets the value of the player's stat")]
    public static void GetStat(string name)
    {
        FieldInfo field = typeof(StatBlock).GetField(name);
        if (field != null)
        {
            Debug.Log(field.GetValue(PlayerEquipmentController.Instance.Stats));
        }
        else
        {
            string[] data = name.Split('/');
            if (PlayerEquipmentController.Instance.TryGetUpgrade(data[0], out var upgrade))
            {
                var upgradeField = upgrade.Upgrade.GetType().GetField(data[1]);
                Debug.Log(upgradeField.GetValue(upgrade.Upgrade));
            }
        }
    }

    [ConsoleMethod("gu", "Gives an upgrade")]
    public static void GiveUpgrade(string name, int count = 1)
    {
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

        var data = Resources.Load<TextAsset>("table");
        List<string> upgradePaths = JsonConvert.DeserializeObject<List<string>>(data.text);
        List<UpgradeDefinition> intermediate = new();

        foreach (var path in upgradePaths)
        {
            var upgradeData = Resources.Load<TextAsset>(path);
            Debug.Log($"Loading upgrade data at path \"{path}\": {upgradeData.text}");
            try
            {
                var group = JsonConvert.DeserializeObject<UpgradeGroup>(upgradeData.text);
                intermediate.AddRange(group.GetDefinitions);
            }
            catch (Exception)
            {
                Debug.LogWarning(
                    $"Upgrade at path {path} is in legacy format.. reverting to legacy behavior for file"
                );

                intermediate.AddRange(
                    JsonConvert.DeserializeObject<List<UpgradeDefinition>>(upgradeData.text)
                );
            }
        }

        intermediate.AddRange(IngestStatDirectives());

        upgrades = new();
        foreach (var definition in intermediate)
        {
            try
            {
                var generated = definition.ToData();
                foreach (var upgrade in generated)
                {
                    Assert.IsFalse(
                        upgrades.ContainsKey(upgrade.GenerateId()),
                        "Clash for upgrade key"
                    );
                    upgrades.Add(upgrade.GenerateId(), upgrade);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(
                    $"Skipped ingesting upgrade {definition.Name} due to {e.GetType()} exception."
                );
            }
        }

        Debug.Log(
            $"Loaded {upgrades.Count} upgrades in {(DateTime.Now - time).Milliseconds}ms, ids=[{string.Join("\n-", upgrades.Keys)}]."
        );

        var settingsData = Resources.Load<TextAsset>("Upgrades/settings");
        settings = JsonConvert.DeserializeObject<UpgradeSettings>(settingsData.text);
    }

    List<UpgradeDefinition> IngestStatDirectives()
    {
        return typeof(StatBlock)
            .GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Where(type => type.GetCustomAttribute<StatUpgradeDirective>(true) != null)
            .Select(type => Tuple.Create(type, type.GetCustomAttribute<StatUpgradeDirective>(true)))
            .Select(duo =>
            {
                List<UpgradeRarity> rarities =
                    new()
                    {
                        UpgradeRarity.COMMON,
                        UpgradeRarity.UNCOMMON,
                        UpgradeRarity.RARE,
                        UpgradeRarity.LEGENDARY,
                    };

                var tiers = rarities.SelectEnumerate(
                    (rarity, idx) =>
                    {
                        Dictionary<string, object> stats = new Dictionary<string, object>()
                        {
                            { "FieldName", duo.Item1.Name },
                            { "Amount", duo.Item2.Values[idx] },
                            { "IsPercentage", duo.Item2.IsPercent },
                            { "StatName", duo.Item2.StatName }
                        };

                        return new UpgradeTierDefinition() { Rarity = rarity, Stats = stats };
                    }
                );

                return new UpgradeDefinition()
                {
                    Name = duo.Item2.Name,
                    Description = duo.Item2.Description,
                    Behavior = typeof(StatBehavior),
                    Class = typeof(StatChanger),
                    Prefix = duo.Item1.Name.ToLower(),
                    Tiers = tiers.ToArray(),
                    Requires = duo.Item2.Requires
                };
            })
            .ToList();
    }

    void Awake()
    {
        IngestData();
    }

    void Start()
    {
        Debug.Log("Validating upgrade data");

        foreach (var upgrade in upgrades.Values)
        {
            upgrade.Validate();
        }

        Debug.Log($"Upgrades validated");

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
