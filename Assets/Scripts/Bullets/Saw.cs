using UnityEngine;

public class SawBullet : Bullet
{
    public float RotationSpeed = 2.0f;
    public float Speed = 2.0f;
    public AudioClip ReflectSound;
    public AudioClip HitSound;

    private Vector3 dir;
    private Rigidbody rb;
    private int bounces;

    public override void Shoot()
    {
        Physics.SyncTransforms();
        rb = GetComponent<Rigidbody>();
        dir = transform.forward;
    }

    void FixedUpdate()
    {
        if (
            Physics.SphereCast(
                transform.position,
                0.5f,
                dir,
                out var hit,
                2.5f,
                LayerMask.GetMask("Default")
            )
        )
        {
            dir = Vector3.Reflect(dir, hit.normal);
        }
        rb.MovePosition(transform.position + dir * Speed);
    }

    void OnTriggerEnter(Collider collider)
    {
        RaiseHitFromCollider(collider);
    }
}
