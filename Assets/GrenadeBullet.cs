using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrenadeBullet : Bullet
{
    const float SHOOT_FORCE = 100.0f;

    public float Force = SHOOT_FORCE;
    public float ExplosionDelay;
    public int Damage;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Normally against this, making an exception here.
        ExplosionDelay = PlayerEquipmentController.Instance.Stats.ExplosionDelay;
        Damage = PlayerEquipmentController.Instance.Stats.GrenadeExplosiveDamage;
    }

    public override void Shoot()
    {
        Physics.SyncTransforms();

        rb.AddForce(transform.forward * Force, ForceMode.Impulse);

        StartCoroutine(DelayThenExplode());
    }

    void OnTriggerEnter(Collider collider)
    {
        var data = new BulletHitData() { Collider = collider };

        if (collider.TryGetComponent(out Health health))
        {
            data.Health = health;
        }

        data.Bullet = this;

        RaiseHit(data);

        Explode();
    }

    IEnumerator DelayThenExplode()
    {
        yield return new WaitForSeconds(ExplosionDelay);
        Explode();
    }

    bool IsCrit(StatBlock stats) => Utilities.Randf() <= stats.CriticalChance;

    void Explode()
    {
        var explosion = Instantiate(
            Resources.Load<Explosion>("Upgrades/Explosion"),
            transform.position,
            Quaternion.identity
        );

        explosion.Layers = LayerMask.GetMask("Enemy");
        explosion.Damage = PlayerEquipmentController.Instance.Stats.GrenadeExplosiveDamage;
        explosion.Size = PlayerEquipmentController.Instance.Stats.ExplosionSize;
        explosion.Owner = PlayerEquipmentController.Instance.gameObject;
        Destroy(gameObject);
    }
}
