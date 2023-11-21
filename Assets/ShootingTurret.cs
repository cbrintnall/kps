using UnityEngine;

public class ShootingTurret : MonoBehaviour
{
    public Bullet Bullet;
    public StatFloat Cooldown = 1f;
    public Transform Barrel;
    public Transform Head;
    public StatInt Damage = 1;
    public StatFloat MonitorRadius = 10.0f;
    public AudioClip ShootSound;
    public Faction Faction = Faction.PLAYER;

    TimeSince ts;
    Transform target;

    void Awake()
    {
        ts = 0;
    }

    void FixedUpdate()
    {
        if (target != null)
            return;

        foreach (
            var collider in Physics.OverlapSphere(
                transform.position,
                MonitorRadius,
                LayerMask.GetMask("Enemy")
            )
        )
        {
            target = collider.transform;
            break;
        }
    }

    void Update()
    {
        if (target)
        {
            Head.transform.forward = Vector3.Lerp(
                Head.transform.forward,
                (target.position - Head.transform.position).normalized,
                Time.deltaTime * 0.6f
            );
            if (Vector3.Distance(target.position, transform.position) > MonitorRadius)
            {
                target = null;
            }
        }

        if (target != null && ts > Cooldown)
        {
            ts = 0;
            var bullet = Instantiate(Bullet);
            ShootBullet(bullet);
        }
    }

    void ShootBullet(Bullet bullet)
    {
        bullet.transform.position = Barrel.transform.position;
        bullet.Start = Barrel.transform.position;
        bullet.Barrel = Barrel;
        bullet.transform.forward = (target.position - Barrel.transform.position).normalized;

        if (bullet is HitscanBullet b)
        {
            b.BulletSize = 1.25f;
            b.Mask = LayerMask.GetMask("Enemy");
        }

        bullet.Hit += (hit) =>
        {
            var collider = hit.Collider ?? hit.Hit.collider;
            if (collider.TryGetComponent(out Health health))
            {
                health.Damage(new DamagePayload() { Owner = gameObject, Amount = GetDamage() });
            }
        };

        bullet.Shoot();
        this.PlayAtMe(ShootSound);
    }

    int GetDamage()
    {
        return (
                (Faction.Alliances & Alliance.PLAYER) > 0
                    ? PlayerEquipmentController.Instance.Stats.AllyDamageBonus
                    : 0
            ) + Damage;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, MonitorRadius);
    }
}
