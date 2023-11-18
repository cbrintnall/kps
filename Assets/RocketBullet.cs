using UnityEngine;

public class RocketBullet : Bullet
{
    const float BULLET_RADIUS = 1.0f;
    public float Speed = 1.0f;

    bool moving;
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void Shoot()
    {
        Physics.SyncTransforms();
        moving = true;
    }

    void FixedUpdate()
    {
        if (!moving)
            return;

        rb.MovePosition(transform.position + transform.forward * Speed);

        if (
            Physics.SphereCast(
                transform.position,
                BULLET_RADIUS,
                transform.forward,
                out RaycastHit hit,
                ~LayerMask.GetMask("Player") ^ LayerMask.GetMask("EnemiesOnly")
            )
        )
        {
            OnCollide(hit.collider);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        OnCollide(collider);
    }

    void OnCollide(Collider collider)
    {
        var hit = new BulletHitData() { Collider = collider, Bullet = this };

        if (collider.TryGetComponent(out Health health))
        {
            hit.Health = health;
        }

        RaiseHit(hit);
    }
}
