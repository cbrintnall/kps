using System;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// A large class holding all the stats relevant to a given player
/// </summary>
[Serializable]
public class StatBlock
{
    [
        Header("Base Damage"),
        StatUpgradeDirective(IsPercent = false, Values = new int[] { 2, 5, 10, 20 })
    ]
    public StatInt RocketDamage = 3;

    [StatUpgradeDirective(
        IsPercent = false,
        Values = new int[] { 3, 7, 15, 30 },
        Name = "On Hit Damage"
    )]
    public StatInt HitScanDamage = 1;
    public StatInt GrenadeExplosiveDamage = 2;
    public StatInt GrenadeHitDamage = 1;

    [StatUpgradeDirective(
        IsPercent = false,
        Values = new int[] { 3, 7, 15, 30 },
        Name = "Ally Damage Bonus"
    )]
    public StatInt AllyDamageBonus = 0;

    [Header("Cooldowns")]
    [StatUpgradeDirective(
        IsPercent = true,
        Values = new int[] { 3, 7, 15, 30 },
        Name = "Cooldown Decrease"
    )]
    public StatFloat PistolCooldown = new StatFloat(1.25f, 15.0f, 0.005f);
    public StatFloat ChargeSpeed = 0.0001f;

    [Header("Critical")]
    [StatUpgradeDirective(
        IsPercent = false,
        Values = new int[] { 2, 5, 9, 12 },
        Name = "Critical Chance Increase"
    )]
    public StatFloat CriticalChance = 0.05f;
    public StatFloat CriticalMultiplier = 1.1f;

    [Header("Timing")]
    public StatFloat ExplosionDelay = new StatFloat(1.5f, float.PositiveInfinity, 0.3f);

    [Header("Size")]
    [StatUpgradeDirective(
        IsPercent = true,
        Values = new int[] { 3, 7, 15, 30 },
        Name = "Exposive Size"
    )]
    public StatFloat ExplosionSize = new StatFloat(3.0f, float.PositiveInfinity, 1.0f);

    [Header("Drop chances")]
    [StatUpgradeDirective(
        IsPercent = true,
        Values = new int[] { 8, 15, 25, 40 },
        Name = "Powerup Drop Chance"
    )]
    public StatFloat EnemyPowerupDropChance = new StatFloat(0.1f, 1.0f, 0.0f);

    public bool CritWithDamage(int baseDamage, out int finalDamage)
    {
        finalDamage = baseDamage;

        if (Utilities.Randf() < CriticalChance)
        {
            finalDamage = Mathf.RoundToInt(CriticalMultiplier * baseDamage);
            return true;
        }

        return false;
    }

    public int GetDamageForBullet(Bullet bullet)
    {
        if (bullet is HitscanBullet)
        {
            return HitScanDamage;
        }
        if (bullet is GrenadeBullet)
        {
            return GrenadeExplosiveDamage;
        }
        if (bullet is RocketBullet)
        {
            return RocketDamage;
        }

        Assert.IsTrue(false, "Shouldn't get here");

        return 0;
    }
}
