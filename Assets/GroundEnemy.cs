using Sirenix.Utilities;
using UnityEngine;

public class GroundEnemy : Enemy
{
    public AudioClip[] SwingSounds;
    public AudioClip[] HitSounds;
    protected TimeSince attack;

    protected override void SubscribeToAnimationEvents(AnimationEventsHandler handler)
    {
        base.SubscribeToAnimationEvents(handler);

        attack = 0.0f;

        handler.Attacked += Attacked;
        handler.IsAttacking += () =>
        {
            audioManager.Play(
                new AudioPayload()
                {
                    Clip = SwingSounds.Random(),
                    Transform = transform,
                    PitchWobble = 0.1f
                }
            );
        };
    }

    protected override void Attacked()
    {
        attack = 0.0f;

        Physics
            .OverlapBox(
                transform.position + transform.forward,
                Vector3.one * 2.0f,
                Quaternion.identity,
                LayerMask.GetMask("Player")
            )
            .ForEach(hit =>
            {
                if (hit.TryGetComponent(out Health health))
                {
                    health.Damage(Damage);
                    audioManager.Play(
                        new AudioPayload()
                        {
                            Clip = HitSounds.Random(),
                            Transform = transform,
                            PitchWobble = 0.1f
                        }
                    );
                }
            });
    }

    protected override void UpdateAtTarget()
    {
        base.UpdateAtTarget();

        if (attack > 3.5f)
        {
            Animator.SetTrigger("Attack");
            attack = 0.0f;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        target = PlayerEquipmentController.Instance.transform;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + transform.forward, Vector3.one * 2);
    }
}
