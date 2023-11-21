using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;

public class BulletRichocet : Upgrade
{
    public StatFloat ScanSize = 20.0f;
    List<Tuple<Vector3, Vector3>> reflections = new();

    public override void OnBulletHit(UpgradePipelineData pipelineData, BulletHitData data)
    {
        base.OnBulletHit(pipelineData, data);

        if (data.Health == null)
        {
            var reflected = Vector3.Reflect(data.Bullet.transform.forward, data.Hit.normal);
            reflections.Add(Tuple.Create(data.Hit.point, reflected));

            var bullet = Instantiate(data.Bullet, data.EndPoint, Quaternion.identity);

            bullet.Barrel = null;
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

            hits.Sort(
                (a, b) =>
                    Vector3.Distance(data.EndPoint, a.point)
                    > Vector3.Distance(data.EndPoint, b.point)
                        ? -1
                        : 1
            );

            foreach (var hit in hits)
            {
                if (hit.collider.TryGetComponent(out Health health))
                {
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

            pipelineData.ShotFrom.VirtualShoot(bullet);
        }
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
