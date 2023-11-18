using System;
using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    public event Action Destroyed;
    public bool DestroyOnTouch = true;
    public int Damage = 1;
    public AudioClip HitSound;

    float scaleTimeSec = .25f;
    TimeSince ts;

    void Awake()
    {
        ts = 0;
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        transform.localScale = Vector3.one * Mathf.Lerp(0.0f, 1.0f, ts / scaleTimeSec);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out Health health))
        {
            health.Damage(Damage);
            this.PlayAtMe(HitSound);

            if (DestroyOnTouch)
            {
                Destroyed?.Invoke();
                Destroy(gameObject);
            }
        }
    }
}
