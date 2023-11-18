using UnityEngine;

public class RandomBulletChance : Upgrade
{
    public StatFloat Chance = new StatFloat(0.0f, 1.0f, 0.0f);
    public Bullet Prefab;
    public string Path;

    void Start()
    {
        Prefab = Resources.Load<Bullet>(Path);
    }

    public override void OnBulletShot(UpgradePipelineData pipelineData, Bullet bullet)
    {
        base.OnBulletShot(pipelineData, bullet);

        if (Utilities.Randf() > Chance)
            return;

        Gun.Shoot(Vector3.zero, Prefab);
    }
}
