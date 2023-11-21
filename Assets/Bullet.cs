using System;
using UnityEngine;

public class BulletHitData
{
    public Health Health;
    public RaycastHit Hit;
    public Collider Collider;
    public Bullet Bullet;
    public Vector3 EndPoint;

    public Collider Target => Hit.collider ?? Collider;
}

public abstract class Bullet : MonoBehaviour
{
    public event Action<BulletHitData> Hit;
    public Transform Barrel;
    public Vector3 Start;
    public Vector3 Direction;
    public ShootPayload ExtraData;

    // Used for upgrades to stop the bullet from freeing itself
    public bool CancelFree = false;

    public abstract void Shoot();

    protected void RaiseHit(BulletHitData target)
    {
        target.Bullet = this;

        Hit?.Invoke(target);
    }
}
