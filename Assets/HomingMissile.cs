using UnityEngine;

public enum HomingStrategy
{
    BREAKOFF,
    TARGETPOINT
}

public class HomingMissile : MonoBehaviour
{
    public float Height = 10.0f;
    public float BreakOffDistance = 10.0f;
    public float MonitorRadius = 1.0f;
    public float Speed = 5.0f;
    public int Damage = 3;
    public HomingStrategy Strategy = HomingStrategy.TARGETPOINT;
    public GameObject TargetPointFX;

    [Header("Explosion data")]
    public Explosion Explosion;
    public int ExplosionSize;

    public Transform Target;
    public Vector3 TargetPoint;

    bool broke;
    TimeSince ts;
    Vector3 start;
    float cummHeight;
    GameObject tp;

    void Awake()
    {
        ts = 0;
    }

    void Start()
    {
        foreach (var c in GetComponentsInChildren<ParticleSystem>())
            c.Play();

        start = transform.position;
        if (Strategy == HomingStrategy.TARGETPOINT)
        {
            TargetPoint = PlayerEquipmentController.Instance.GetGroundPosition();
            var pt = Instantiate(TargetPointFX);
            pt.transform.position = TargetPoint + Vector3.up * 0.25f;
            tp = pt;
            if (Explosion != null)
            {
                tp.transform.localScale *= ExplosionSize;
            }
            else
            {
                tp.transform.localScale = Vector3.one * MonitorRadius * 4;
            }
        }
    }

    void FixedUpdate()
    {
        switch (Strategy)
        {
            case HomingStrategy.BREAKOFF:
                ResolveBreakOff();
                break;
            case HomingStrategy.TARGETPOINT:
                ResolveTargetPoint();
                break;
        }
    }

    void OnDestroy()
    {
        Destroy(tp);
    }

    void ResolveTargetPoint()
    {
        var nexttarget = TargetPoint;
        var dir = (nexttarget - transform.position).normalized;
        transform.forward = dir;
        var climbRate = 0.25f;
        cummHeight += climbRate;
        dir = new Vector3(dir.x, cummHeight > 50.0f ? dir.y : climbRate, dir.z);

        if (Vector3.Distance(transform.position, nexttarget) < 10.0f)
        {
            if (!broke)
            {
                broke = true;
            }
        }

        if (Vector3.Distance(transform.position, nexttarget) < 2.0f)
        {
            CheckExplosion();
            Destroy(gameObject);
        }

        transform.position += dir * Speed;
    }

    void ResolveBreakOff()
    {
        var nexttarget = broke ? TargetPoint : Target.transform.position;
        var dir = (nexttarget - transform.position).normalized;
        transform.forward = dir;
        var climbRate = 0.25f;
        cummHeight += climbRate;
        dir = new Vector3(dir.x, cummHeight > 15.0f ? dir.y : climbRate, dir.z);

        if (ts > 15.0f && !broke)
        {
            broke = true;
            TargetPoint = PlayerEquipmentController.Instance.GetGroundPosition();
        }

        if (Vector3.Distance(transform.position, nexttarget) < 10.0f)
        {
            if (!broke)
            {
                broke = true;
                TargetPoint = PlayerEquipmentController.Instance.GetGroundPosition();
            }
            if (broke)
            {
                if (
                    Physics.SphereCast(
                        transform.position,
                        MonitorRadius,
                        transform.forward,
                        out var hit,
                        2.5f,
                        LayerMask.GetMask("Player")
                    )
                )
                {
                    if (hit.collider.TryGetComponent(out Health health))
                    {
                        health.Damage(Damage);
                    }

                    CheckExplosion();
                    Destroy(gameObject);
                }

                foreach (
                    var _ in Physics.OverlapSphere(
                        transform.position,
                        2.0f,
                        LayerMask.GetMask("Default")
                    )
                )
                {
                    CheckExplosion();
                    Destroy(gameObject);
                    return;
                }
            }
        }

        transform.position += dir * Speed;
    }

    void CheckExplosion()
    {
        if (Explosion != null)
        {
            var explosion = Instantiate(Explosion, transform.position, Quaternion.identity);
            explosion.Size = ExplosionSize;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, MonitorRadius);
    }
}
