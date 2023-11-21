using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class BulletRichocet : Upgrade
{
    const int max = 10;
    int count;

    public StatFloat ScanSize = 20.0f;
    List<Tuple<Vector3, Vector3>> reflections = new();

    public override void OnBulletHit(UpgradePipelineData pipelineData, BulletHitData data)
    {
        base.OnBulletHit(pipelineData, data);

        if (count >= max)
        {
            Debug.LogWarning("Hit ricochet max");
            return;
        }

        if (data.Health == null)
        {
            var reflected = Vector3.Reflect(data.Bullet.transform.forward, data.Hit.normal);
            reflections.Add(Tuple.Create(data.Hit.point, reflected));

            // var bullet = Instantiate(data.Bullet, data.EndPoint, Quaternion.identity);
            var bullet = data.Bullet;

            bullet.CancelFree = true;
            bullet.Barrel = null;
            bullet.transform.position = data.EndPoint;
            // bullet.Start = data.EndPoint;
            bullet.transform.forward = reflected;

            if (!bullet is HitscanBullet)
            {
                return;
            }

            count++;

            var hits = Physics.BoxCastAll(
                data.Hit.point,
                Vector3.one * ScanSize,
                reflected,
                Quaternion.identity,
                1000.0f,
                LayerMask.GetMask("Enemy")
            );

            var sorted = hits.OrderBy((a) => Vector3.Distance(a.transform.position, bullet.Start))
                .ThenByDescending(
                    hit =>
                        Vector3.Dot((hit.transform.position - bullet.Start).normalized, reflected)
                );

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

            // pipelineData.ShotFrom.VirtualShoot(bullet);
            bullet.Shoot();
        }
    }

    void Update()
    {
        count = 0;
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
