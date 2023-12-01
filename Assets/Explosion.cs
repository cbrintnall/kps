using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public int Damage = 1;
    public float Size = 0.0f;
    public LayerMask Layers;
    public GameObject Owner;
    public Transform[] Scalars;

    private float waitTime;
    private HashSet<Health> applied = new();

    void Awake()
    {
        var particles = GetComponent<ParticleSystem>();
        var shape = particles.shape;
        shape.radius = PlayerEquipmentController.Instance.ExplosionSize;
        waitTime = particles.main.startLifetime.constant;
        StartCoroutine(WaitForParticles());
    }

    void Start()
    {
        foreach (var hit in Physics.OverlapSphere(transform.position, Size, Layers))
        {
            if (hit.TryGetComponent(out Health health) && !applied.Contains(health))
            {
                applied.Add(health);
                health.Damage(new DamagePayload() { Amount = Damage, Owner = Owner });
            }
        }

        foreach (Transform scalar in Scalars)
        {
            scalar.DOScale(0.0f, waitTime).SetEase(Ease.InCubic);
        }
    }

    IEnumerator WaitForParticles()
    {
        yield return new WaitForSeconds(0.01f);
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}
