using System;
using UnityEngine;

public class AnimationEventsHandler : MonoBehaviour
{
    public event Action LegSwung;
    public event Action Attacked;
    public event Action IsAttacking;
    public float Debounce = 0.1f;
    AudioManager audioManager;

    void Awake()
    {
        audioManager = SingletonLoader.Get<AudioManager>();
    }

    void Attacking()
    {
        IsAttacking?.Invoke();
    }

    void Attack()
    {
        Attacked?.Invoke();
    }

    void PlaySound(AudioClip clip)
    {
        audioManager.Play(
            new AudioPayload()
            {
                Clip = clip,
                Location = transform.position,
                Debounce = 0.1f
            }
        );
    }

    void LegHit()
    {
        LegSwung?.Invoke();
    }
}
