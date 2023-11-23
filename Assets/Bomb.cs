using UnityEngine;

public class Bomb : MonoBehaviour
{
    public LayerMask Targets;
    public int Damage;

    void OnTriggerEnter(Collider _)
    {
        var explosion = Instantiate(
            Resources.Load<Explosion>("Upgrades/Explosion"),
            transform.position,
            Quaternion.identity
        );
        explosion.Damage = Damage;
        explosion.Layers = Targets;
        explosion.Size = 5.0f;
        Destroy(gameObject);
    }
}
