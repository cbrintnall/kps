using UnityEngine;

public class BomberEnemy : GroundEnemy
{
    private Health health;

    void Start()
    {
        health = GetComponent<Health>();
    }

    protected override void SubscribeToAnimationEvents(AnimationEventsHandler handler)
    {
        base.SubscribeToAnimationEvents(handler);

        handler.GetComponent<Animator>().SetBool("Holding", true);
    }

    protected override void BeforeDeath(HitPayload payload)
    {
        base.BeforeDeath(payload);

        if (payload?.Owner?.tag == "Player")
        {
            Explode();
        }
    }

    protected override void UpdateAtTarget()
    {
        Explode();
    }

    void Explode()
    {
        var explosion = Instantiate(
            Resources.Load<Explosion>("Upgrades/Explosion"),
            transform.position,
            Quaternion.identity
        );
        explosion.Damage = Damage;
        explosion.Layers = LayerMask.GetMask("Player");
        explosion.Size = 5.0f;
        health.Damage(new DamagePayload() { Amount = health.Data.Current, Owner = gameObject });
        Destroy(gameObject);
    }
}
