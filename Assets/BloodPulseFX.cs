using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering.Universal;

public class BloodPulseFX : MonoBehaviour
{
    public static ObjectPool<DecalProjector> BloodPool = new ObjectPool<DecalProjector>(
        () => Instantiate(Resources.Load<DecalProjector>("BloodFX")),
        fx => { },
        fx => { },
        fx => Destroy(fx),
        true,
        100,
        100
    );

    const float BLOOD_CHANCE = 0.5f;

    public AudioClip BloodSpatterLand;

    private AudioSource Player;
    private ParticleSystem particles;

    private List<ParticleCollisionEvent> events = new();

    void Awake()
    {
        Player = GetComponent<AudioSource>();
        particles = GetComponent<ParticleSystem>();
    }

    void OnParticleCollision(GameObject other)
    {
        particles.GetCollisionEvents(other, events);

        foreach (var ev in events)
        {
            if (Random.Range(0.0f, 1.0f) <= BLOOD_CHANCE)
            {
                var blood = BloodPool.Get();
                var scale = Random.Range(1.0f, 2.0f);

                blood.size = new Vector3(scale, scale, scale);
                blood.transform.position = ev.intersection;
                blood.transform.rotation = Quaternion.Euler(
                    blood.transform.rotation.eulerAngles.x,
                    Random.Range(0.0f, 360.0f),
                    0.0f
                );

                Player.PlayOneShot(BloodSpatterLand);
                BloodPool.Release(blood);
            }
        }
    }
}
