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
    public event Action Missed;
    public Transform Barrel;
    public Vector3 Start;
    public ShootPayload ExtraData;

    // Used for upgrades to stop the bullet from freeing itself
    public bool CancelFree = false;
    public bool DidHit = false;

    void Awake()
    {
        DidHit = false;
    }

    public abstract void Shoot();

    protected void RaiseMiss()
    {
        Missed?.Invoke();
    }

    protected void RaiseHit(BulletHitData target)
    {
        target.Bullet = this;

        DidHit = true;
        Hit?.Invoke(target);
    }

    protected void RaiseHitFromCollider(Collider collider)
    {
        BulletHitData payload = new BulletHitData() { Collider = collider, Bullet = this };

        if (collider.TryGetComponent(out Health health))
        {
            payload.Health = health;
        }

        RaiseHit(payload);
    }

    void OnDestroy()
    {
        if (!DidHit)
        {
            Missed?.Invoke();
        }
    }
}
