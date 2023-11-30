using System;
using System.Collections.Generic;
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
    public Type[] Requires;

    public string Color =>
        SingletonLoader.Get<FlowManager>().GameData.UpgradeColors[Rarity].ToHexString();

    public override string ToString()
    {
        return GenerateId();
    }

    public string GenerateId()
    {
        return $"{GenerateBaseId()}.{Rarity}".ToLower();
    }

    public string GenerateBaseId() => string.IsNullOrEmpty(Prefix) ? Class.Name : Prefix;
}
