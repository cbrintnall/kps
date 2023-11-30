using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum UpgradeRarity
{
    COMMON,
    UNCOMMON,
    RARE,
    LEGENDARY
}

[Serializable]
public class UpgradeData
{
    public string Prefix;
    public UpgradeRarity Rarity = UpgradeRarity.COMMON;
    public UpgradeData Next;
    public UpgradeBehavior Behavior;
    public Dictionary<string, object> Stats;
    public string Name;
    public string Description;
    public Type Class;
    public int Cost;
    public string[] Requires;

    public string Color =>
        SingletonLoader.Get<FlowManager>().GameData.UpgradeColors[Rarity].ToHexString();

    public void Validate()
    {
        if (Requires != null)
        {
            UpgradesManager upgradesManager = SingletonLoader.Get<UpgradesManager>();
            string[] prefixes = upgradesManager.Prefixes;
            foreach (string requirement in Requires)
            {
                Debug.Assert(
                    prefixes.Contains(requirement),
                    $"Requirement {requirement} does not exist for {GenerateId()}"
                );
            }
        }
    }

    public override string ToString()
    {
        return GenerateId();
    }

    public string GenerateId()
    {
        return $"{GenerateBaseId()}.{Rarity}".ToLower();
    }

    public string GenerateBaseId() =>
        (string.IsNullOrEmpty(Prefix) ? Class.Name : Prefix).ToLower();
}
