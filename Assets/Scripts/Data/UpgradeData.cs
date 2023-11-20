using System;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline;

public enum UpgradeRarity
{
    COMMON,
    UNCOMMON,
    RARE,
    LEGENDARY
}

public enum UpgradeScaleType
{
    NONE,
    LINEAR,
    EXPONENTIAL
}

public enum UpgradeType
{
    LADDER,
    STAT
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
    public UpgradeType Type;

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
