using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PipelineLifetime
{
    private static int Tracker;

    static PipelineLifetime()
    {
        Application.quitting += () =>
        {
            if (Tracker > 0)
                Debug.LogWarning($"{Tracker} lifetimes still open at close.");
        };
    }

    private string Name;
    public Action Callback;

    public PipelineLifetime(string name)
    {
        Name = name;
        Tracker++;
    }

    public void Release(string source = "")
    {
        Callback();
        Tracker--;
    }
}

/// <summary>
/// A pipeline for upgrades, a sort of mini state machine.
///
/// Normal flow looks like:
///   (Will shoot -> Shot -> Hit - n...)
/// where 'Hit' can happen on repeat.
/// </summary>
public class UpgradePipeline
{
    List<Upgrade> upgrades = new();
    UpgradePipelineData data;

    PipelineLifetime WillShoot;
    PipelineLifetime DidShoot;

    string id = Guid.NewGuid().ToString();
    TimeSince ts = 0;

    public UpgradePipeline(IEnumerable<Upgrade> upgrades, UpgradePipelineData data, Gun equipment)
    {
        this.upgrades = upgrades.ToList();
        this.data = data;
        this.data.Parent = this;

        equipment.WillShootBullet += OnWillShootBullet;
        data.ShotFrom = equipment;
        WillShoot = new PipelineLifetime("Will shoot")
        {
            Callback = () => equipment.WillShootBullet -= OnWillShootBullet
        };
    }

    public void ListenToBullet(Bullet bullet)
    {
        var lifetime = new PipelineLifetime("Bullet hit");

        Action<BulletHitData> onHit = (BulletHitData hitData) =>
        {
            ts = 0;
            lifetime.Release();

            foreach (var upgrade in upgrades)
            {
                upgrade.OnBulletHit(data, hitData);
            }
        };

        Action onMiss = () =>
        {
            ts = 0;
            lifetime.Release();

            foreach (var upgrade in upgrades)
            {
                upgrade.OnBulletMissed();
            }
        };

        lifetime.Callback = () =>
        {
            if (bullet != null)
            {
                bullet.Hit -= onHit;
                bullet.Missed -= onMiss;
            }
        };

        bullet.Hit += onHit;
        bullet.Missed += onMiss;
    }

    void OnWillShootBullet(Bullet bullet)
    {
        ts = 0;
        Gun target = data.ShotFrom;
        target.ShotBullet += OnShotBullet;
        WillShoot.Release();
        ListenToBullet(bullet);
        DidShoot = new PipelineLifetime("Did shoot")
        {
            Callback = () => target.ShotBullet -= OnShotBullet
        };

        foreach (var upgrade in upgrades)
        {
            upgrade.OnWillShootBullet(data, bullet);
        }
    }

    void OnShotBullet(Bullet bullet)
    {
        ts = 0;
        foreach (var upgrade in upgrades)
        {
            upgrade.OnBulletShot(data, bullet);
        }
        DidShoot.Release();
    }
}
