using System.Collections;
using UnityEngine;

public class BaseOnHitUpgrade : Upgrade
{
    // public override void OnBulletHit(UpgradePipelineData pipelineData, BulletHitData data)
    // {
    //     if (data.Bullet is GrenadeBullet gb)
    //     {
    //         if (data.Health)
    //         {
    //             data.Health.Damage(
    //                 Mathf.RoundToInt(
    //                     IsCrit(pipelineData.PlayerStats)
    //                         ? pipelineData.PlayerStats.GrenadeHitDamage
    //                             * pipelineData.PlayerStats.CriticalMultiplier
    //                         : pipelineData.PlayerStats.GrenadeHitDamage
    //                 )
    //             );
    //             Explode(gb, pipelineData.PlayerStats);
    //         }
    //     }

    //     if (data.Bullet is HitscanBullet hs)
    //     {
    //         data.Health?.Damage(
    //             Mathf.RoundToInt(
    //                 IsCrit(pipelineData.PlayerStats)
    //                     ? pipelineData.PlayerStats.HitScanDamage
    //                         * pipelineData.PlayerStats.CriticalMultiplier
    //                     : pipelineData.PlayerStats.HitScanDamage
    //             )
    //         );
    //     }

    //     if (data.Bullet is RocketBullet rb)
    //     {
    //         data.Health?.Damage(
    //             Mathf.RoundToInt(
    //                 IsCrit(pipelineData.PlayerStats)
    //                     ? pipelineData.PlayerStats.RocketDamage
    //                         * pipelineData.PlayerStats.CriticalMultiplier
    //                     : pipelineData.PlayerStats.RocketDamage
    //             )
    //         );
    //     }
    // }

    // public override void OnBulletShot(UpgradePipelineData pipelineData, Bullet bullet)
    // {
    //     if (bullet is GrenadeBullet gb)
    //     {
    //         // StartCoroutine(DelayThenExplode(gb, pipelineData.PlayerStats));
    //     }
    // }

    // public override void OnPickup(PlayerEquipmentController controller) { }
}
