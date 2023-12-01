using System.Collections.Generic;
using UnityEngine;

public class BulletChanceData
{
    public Dictionary<Bullet, int> Counts = new();
}

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

        var custom = pipelineData.GetCustom<BulletChanceData>();

        if (custom.Counts.TryGetValue(Prefab, out int count))
        {
            if (count > 1)
                return;
        }
        else
        {
            custom.Counts[Prefab] = 0;
        }

        if (Utilities.Randf() > Chance)
            return;

        custom.Counts[Prefab]++;

        Gun.Shoot(new ShootRequestPayload() { PipelineData = pipelineData, BulletPrefab = Prefab });
    }
}
