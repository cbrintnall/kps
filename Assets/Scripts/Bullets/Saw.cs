using UnityEngine;

public class SawBullet : Bullet
{
    public float RotationSpeed = 2.0f;
    public float Speed = 2.0f;
    public AudioClip ReflectSound;
    public AudioClip HitSound;

    private Vector3 dir;
    private Rigidbody rb;
    private AudioManager audioManager;
    private int bounce;

    public override void Shoot()
    {
        audioManager = SingletonLoader.Get<AudioManager>();
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
            if (bounce >= PlayerEquipmentController.Instance.Stats.MaxSawBounces)
                Destroy(gameObject);

            bounce++;
            dir = Vector3.Reflect(dir, hit.normal);
            audioManager.Play(
                new AudioPayload()
                {
                    Clip = ReflectSound,
                    Location = transform.position,
                    PitchWobble = 0.2f
                }
            );
        }
        rb.MovePosition(transform.position + dir * Speed);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out Health _))
        {
            audioManager.Play(
                new AudioPayload()
                {
                    Clip = HitSound,
                    Location = transform.position,
                    PitchWobble = 0.2f
                }
            );
        }
        RaiseHitFromCollider(collider);
    }
}
