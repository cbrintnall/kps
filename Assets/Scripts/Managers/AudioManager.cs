using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

public class AudioPayload
{
    public AudioClip Clip;
    public Vector3 Location;
    public Transform Transform;
    public float Volume = 1.0f;
    public float PitchWobble = 0.0f;
    public float Debounce = 0.0f;
    public bool Is2D = false;

    public static implicit operator AudioPayload(AudioClip clip) =>
        new AudioPayload() { Clip = clip };
}

public static class AudioExtensions
{
    public static void PlayAtMe(this Component component, AudioClip clip)
    {
        SingletonLoader
            .Get<AudioManager>()
            .Play(new AudioPayload() { Clip = clip, Location = component.transform.position });
    }

    public static void PlayAtMe(this Component component, AudioPayload payload)
    {
        SingletonLoader.Get<AudioManager>().Play(payload);
    }
}

[Singleton]
public class AudioManager : MonoBehaviour
{
    private ObjectPool<AudioSource> pool;

    private Dictionary<AudioClip, TimeSince> debounce = new();
    private AudioMixerGroup mixerGroup;

    void Awake()
    {
        mixerGroup = Resources.Load<AudioMixer>("Audio/Game").FindMatchingGroups("SFX")[0];
        pool = new(
            () =>
            {
                var source = new GameObject($"audio-source-{Guid.NewGuid()}");
                source.transform.SetParent(transform);

                var player = source.AddComponent<AudioSource>();
                source.AddComponent<Follower>();
                player.spatialBlend = 1.0f;
                player.dopplerLevel = 0.0f;
                player.outputAudioMixerGroup = mixerGroup;
                return player;
            },
            source =>
            {
                source.enabled = true;
            },
            source =>
            {
                source.enabled = false;
                source.Stop();
            },
            source => Destroy(source.gameObject)
        );
    }

    public void Play(AudioPayload payload)
    {
        if (payload.Debounce > 0.0)
        {
            if (debounce.TryGetValue(payload.Clip, out TimeSince ts))
            {
                if (ts < payload.Debounce)
                {
                    return;
                }
                else
                {
                    debounce[payload.Clip] = Time.time;
                }
            }
        }

        var player = pool.Get();

        player.spatialBlend = payload.Is2D ? 0.0f : 1.0f;
        player.pitch = 1.0f + Utilities.Randf() * payload.PitchWobble;
        player.volume = payload.Volume;

        if (payload.Location != Vector3.zero)
        {
            player.transform.position = payload.Location;
            player.PlayOneShot(payload.Clip);
        }
        else if (payload.Transform)
        {
            StartCoroutine(Track(player.transform, payload.Transform, payload.Clip.length));
            player.PlayOneShot(payload.Clip);
        }
        else
        {
            player.PlayOneShot(payload.Clip);
        }

        StartCoroutine(WaitForDone(player, payload.Clip.length));
    }

    IEnumerator WaitForDone(AudioSource source, float length)
    {
        yield return new WaitForSeconds(length);
        pool.Release(source);
    }

    IEnumerator Track(Transform child, Transform transform, float length)
    {
        var follower = child.GetComponent<Follower>();
        follower.Transform = transform;
        yield return new WaitForSeconds(length);
        follower.Transform = null;
    }
}
