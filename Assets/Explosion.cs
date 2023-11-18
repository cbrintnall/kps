using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public int Damage = 1;
    public float Size = 0.0f;
    public LayerMask Layers;
    public GameObject Owner;

    void Awake()
    {
        var particles = GetComponent<ParticleSystem>();
        var shape = particles.shape;
        shape.radius = PlayerEquipmentController.Instance.ExplosionSize;
        float wait = particles.main.startLifetime.constant;
        StartCoroutine(WaitForParticles(wait));
    }

    void Start()
    {
        foreach (var hit in Physics.OverlapSphere(transform.position, Size, Layers))
        {
            if (hit.TryGetComponent(out Health health))
            {
                health.Damage(new DamagePayload() { Amount = Damage, Owner = Owner });
            }
        }
    }

    IEnumerator WaitForParticles(float time)
    {
        yield return new WaitForSeconds(0.01f);
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
