using UnityEngine;

public class BulletBounce : Upgrade
{
    // TODO: Figure out how to better limit this causing a stack overflow (ideally, each bounce reduces the chance of the next bounce by say 10%)
    public StatFloat BounceChance = new StatFloat(0.0f, 1.0f, 0.0f);

    public override void OnBulletHit(UpgradePipelineData pipelineData, BulletHitData data)
    {
        if (EnemyMaster.ActiveEnemies.Count == 0)
            return;

        if (Utilities.Randf() > BounceChance)
            return;

        var bullet = Instantiate(
            data.Bullet,
            data.Bullet.transform.position,
            data.Bullet.transform.rotation
        );
        var barrel =
            data.Hit.collider != null ? data.Hit.collider.transform : data.Collider.transform;

        var newTarget = EnemyMaster.ActiveEnemies.Random();
        bullet.Start = bullet.transform.position;
        bullet.transform.forward = (
            newTarget.transform.position - bullet.transform.position
        ).normalized;
        bullet.Barrel = barrel;
        Gun.MonitorBullet(bullet);
        bullet.Shoot();
        Debug.Log("bounced");
    }
}
