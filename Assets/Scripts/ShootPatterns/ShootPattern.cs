using System;
using System.Collections;
using UnityEngine;

public delegate Bullet ShootCallback(Vector3 variance);

public class ShootPattern : MonoBehaviour
{
    public Vector3[] BulletPattern = new Vector3[] { Vector3.zero };

    /// <summary>
    /// The total amount of bullets fired during a burst, IE three round burst would be 3
    /// </summary>
    public StatInt PerBurst = 1;

    /// <summary>
    /// How much the bullet can spread from the initial position, where zero is a straight line.
    /// </summary>
    public StatFloat[] PerBulletDirectionVariance = { 0, 0, 0 };

    /// <summary>
    /// How long in between bursts do we cooldown.
    /// </summary>
    public StatFloat BurstCooldown = 0.1f;

    Coroutine activeBurst;
    int PerShot => BulletPattern.Length;

    public void Shoot(ShootCallback cb, Action fx)
    {
        if (activeBurst == null)
        {
            activeBurst = StartCoroutine(DoShot(cb, fx));
        }
    }

    IEnumerator DoShot(ShootCallback cb, Action fx)
    {
        var delay = new WaitForSeconds(BurstCooldown);
        var variance = new Vector3(
            PerShot > 1
                ? Mathf.Max(0.01f, PerBulletDirectionVariance[0])
                : PerBulletDirectionVariance[0],
            PerShot > 1
                ? Mathf.Max(0.01f, PerBulletDirectionVariance[1])
                : PerBulletDirectionVariance[1],
            PerShot > 1
                ? Mathf.Max(0.01f, PerBulletDirectionVariance[2])
                : PerBulletDirectionVariance[2]
        );

        for (int i = 0; i < PerBurst; i++)
        {
            fx.Invoke();
            for (int j = 0; j < BulletPattern.Length; j++)
            {
                var vec =
                    BulletPattern[j]
                    + new Vector3(
                        UnityEngine.Random.Range(-variance.x, variance.x),
                        UnityEngine.Random.Range(-variance.y, variance.y),
                        UnityEngine.Random.Range(-variance.z, variance.z)
                    );

                cb.Invoke(vec);
            }

            yield return delay;
        }
        activeBurst = null;
    }
}
