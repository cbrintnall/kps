using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
    public StatInt HitScanDamage = 1;
    public StatInt GrenadeExplosiveDamage = 2;
    public StatInt GrenadeHitDamage = 1;

    [Header("Cooldowns")]
    public StatFloat PistolCooldown = new StatFloat(1.25f, 15.0f, 0.001f);

    [Header("Critical")]
    public StatFloat CriticalChance = 0.05f;
    public StatFloat CriticalMultiplier = 1.1f;

    [Header("Timing")]
    public StatFloat ExplosionDelay = new StatFloat(1.5f, float.PositiveInfinity, 0.3f);

    [Header("Size")]
    public StatFloat ExplosionSize = new StatFloat(3.0f, float.PositiveInfinity, 1.0f);

    [Header("Drop chances")]
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
