using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletRichocet : Upgrade
{
    public StatFloat ScanSize = 5.0f;

    AudioClip flyby;
    List<Tuple<Vector3, Vector3>> reflections = new();

    void Awake()
    {
        flyby = Resources.Load<AudioClip>("Audio/Bullet Flyby 7");
    }

    public override void OnBulletHit(UpgradePipelineData pipelineData, BulletHitData data)
    {
        base.OnBulletHit(pipelineData, data);

        var ricochetData = pipelineData.GetCustom<RicochetData>();

        if (ricochetData.Count >= RicochetData.MaxBounces)
        {
            return;
        }

        ricochetData.Count++;

        if (data.Bullet is HitscanBullet)
        {
            HandleHitscan(pipelineData, data);
        }
    }

    public override void OnWillShootBullet(UpgradePipelineData pipelineData, Bullet bullet)
    {
        base.OnWillShootBullet(pipelineData, bullet);

        if (bullet is GrenadeBullet)
        {
            HandleGrenade(pipelineData, bullet);
        }
    }

    void HandleGrenade(UpgradePipelineData pipelineData, Bullet data)
    {
        var attractor = data.gameObject.AddComponent<Attractor>();

        attractor.Targets = LayerMask.GetMask("Enemy");
        attractor.Radius = 20.0f;
        attractor.Force = 7.5f;
    }

    void HandleHitscan(UpgradePipelineData pipelineData, BulletHitData data)
    {
        if (data.Health != null)
            return;

        var reflected = Vector3.Reflect(data.Bullet.transform.forward, data.Hit.normal);
        reflections.Add(Tuple.Create(data.Hit.point, reflected));

        var bullet = Instantiate(data.Bullet, data.EndPoint, Quaternion.identity);

        bullet.Barrel = null;
        bullet.transform.position = data.EndPoint;
        bullet.Start = data.EndPoint;
        bullet.transform.forward = reflected;

        var hits = Physics.BoxCastAll(
            data.Hit.point,
            Vector3.one * ScanSize,
            reflected,
            Quaternion.identity,
            1000.0f,
            LayerMask.GetMask("Enemy")
        );

        var sorted = hits.OrderBy((a) => Vector3.Distance(a.point, data.Hit.point));
        // .ThenByDescending(hit => Vector3.Dot((hit.point - bullet.Start).normalized, reflected));

        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent(out Health health))
            {
                if (health.Dead)
                    continue;

                if (
                    !Physics.Raycast(
                        bullet.Start,
                        health.transform.position - bullet.Start,
                        Vector3.Distance(bullet.Start, health.transform.position),
                        LayerMask.GetMask("Default")
                    )
                )
                {
                    bullet.transform.forward =
                        hit.collider.transform.position - bullet.transform.position;

                    bullet.transform.forward.Normalize();

                    break;
                }
            }
        }

        pipelineData.Parent.ListenToBullet(bullet);
        pipelineData.ShotFrom.MonitorBullet(bullet);
        bullet.Shoot();

        SingletonLoader
            .Get<AudioManager>()
            .Play(
                new AudioPayload()
                {
                    Clip = flyby,
                    Location = bullet.transform.position,
                    PitchWobble = 0.2f
                }
            );
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var r in reflections)
        {
            Gizmos.DrawLine(r.Item1, r.Item1 + r.Item2 * 100.0f);
        }
    }
}
