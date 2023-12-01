using UnityEngine;

public class Bomb : MonoBehaviour
{
    public LayerMask Targets;
    public TargetPoint TargetPoint;
    public float ExplosionSize = 5.0f;
    public int Damage;

    void Start()
    {
        TargetPoint.transform.SetParent(null);
        TargetPoint.transform.localScale *= ExplosionSize;
    }

    void OnDestroy()
    {
        Destroy(TargetPoint.gameObject);
    }

    void OnTriggerEnter(Collider _)
    {
        var explosion = Instantiate(
            Resources.Load<Explosion>("Upgrades/Explosion"),
            transform.position,
            Quaternion.identity
        );
        explosion.Damage = Damage;
        explosion.Layers = Targets;
        explosion.Size = ExplosionSize;
        Destroy(gameObject);
    }
}
