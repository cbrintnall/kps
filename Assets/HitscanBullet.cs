using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEngine;

// TODO: this doesn't need to be a full game object, creating then destroying immediately isn't wise
public class HitscanBullet : Bullet
{
    const float SHOOT_DISTANCE = 1000.0f;
    const float MIN_FX_DISTANCE = 0.1f;

    public LayerMask Mask;
    public float BulletSize = 0.75f;
    public int PierceAmount = 1;

    bool debug = false;

    /// <summary>
    /// "Shoots" the bullet, as in runs the physics simulation and searches for targets.
    /// </summary>
    public override void Shoot()
    {
        var end = Barrel.transform.position + transform.forward * SHOOT_DISTANCE;
        var hits = Physics.SphereCastAll(
            Start,
            BulletSize,
            transform.forward,
            SHOOT_DISTANCE,
            Mask,
            QueryTriggerInteraction.Collide
        );

        if (hits.Length > 0)
        {
            hits.Sort(
                (a, b) =>
                    Mathf.RoundToInt(
                        Vector3.Distance(a.point, Start) - Vector3.Distance(b.point, Start)
                    )
            );

            int extra =
                Mathf.FloorToInt(ExtraData?.ChargeTime ?? 0.0f) >= 1
                    ? Mathf.FloorToInt(ExtraData?.ChargeTime ?? 0.0f) + 1
                    : 1;

            foreach (
                var hit in hits.Where(cast => cast.collider != null).Take(PierceAmount * extra)
            )
            {
                if ((1 << hit.collider.gameObject.layer & LayerMask.GetMask("Enemy")) == 0)
                {
                    break;
                }

                var data = new BulletHitData() { Hit = hit };

                if (hit.collider.TryGetComponent(out Health health))
                {
                    data.Health = health;
                    data.Bullet = this;
                }

                RaiseHit(data);

                end = hit.point;
            }
        }

        if (Vector3.Distance(Start, end) > MIN_FX_DISTANCE)
        {
            var fx = Instantiate(Resources.Load("BulletFX")).GetComponent<LineRenderer>();
            fx.SetPositions(new Vector3[] { Barrel.transform.position, end });
            var t = fx.DOColor(
                new Color2() { ca = Color.white, cb = Color.white },
                new Color2() { ca = Color.clear, cb = Color.clear },
                0.4f
            );

            t.OnComplete(() =>
            {
                Destroy(fx.gameObject);
                if (!debug || !CancelFree)
                {
                    Destroy(gameObject);
                }
            });
        }
        else
        {
            if (!debug || !CancelFree)
                Destroy(gameObject);
        }
    }
}
