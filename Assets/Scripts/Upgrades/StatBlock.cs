using System;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// A large class holding all the stats relevant to a given player
/// </summary>
[Serializable]
public class StatBlock
{
    [Header("Base Damage")]
    public StatInt RocketDamage = 3;

    [StatUpgradeDirective(IsPercent = false, Values = new int[] { 3, 7, 15, 30 }, Name = "Damage")]
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
        Values = new int[] { -3, -7, -15, -30 },
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

    [StatUpgradeDirective(
        IsPercent = false,
        Values = new int[] { 1, 3, 5, 8 },
        Name = "Turret Damage"
    )]
    public StatInt TurretDamage = 1;

    [Header("Timing")]
    public StatFloat ExplosionDelay = new StatFloat(1.5f, float.PositiveInfinity, 0.3f);

    [Header("Size")]
    [StatUpgradeDirective(
        IsPercent = true,
        Values = new int[] { 3, 7, 15, 30 },
        Name = "Exposive Size"
    )]
    public StatFloat ExplosionSize = new StatFloat(3.0f, float.PositiveInfinity, 1.0f);

    [StatUpgradeDirective(
        IsPercent = false,
        Values = new int[] { 3, 7, 8, 15 },
        Name = "Exposive Damage"
    )]
    public StatInt ExplosionDamage = new StatInt(3, 999999, 0);

    [Header("Drop chances")]
    [StatUpgradeDirective(
        IsPercent = true,
        Values = new int[] { 8, 15, 25, 40 },
        Name = "Powerup Drop Chance"
    )]
    public StatFloat EnemyPowerupDropChance = new StatFloat(0.1f, 1.0f, 0.0f);

    [StatUpgradeDirective(
        IsPercent = false,
        Values = new int[] { 1, 2, 4, 6 },
        Name = "Bullet Pierce"
    )]
    public StatInt PierceAmount = new StatInt(1, 999, 1);

    [StatUpgradeDirective(
        IsPercent = false,
        Values = new int[] { 3, 5, 9, 13 },
        Name = "Corpse Explosion"
    )]
    public StatInt ExplodeOnEnemyDeathChance = new StatInt(0, 100, 0);

    [StatUpgradeDirective(
        IsPercent = false,
        Values = new int[] { 3, 5, 9, 13 },
        Name = "Kick Explosion"
    )]
    public StatInt KickExplosion = new StatInt(0, 100, 0);

    [StatUpgradeDirective(
        IsPercent = false,
        Values = new int[] { 1, 2, 4, 8 },
        Name = "Saw Bounces",
        Requires = new string[] { "saw" }
    )]
    public StatInt MaxSawBounces = new StatInt(3, 100, 1);

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
