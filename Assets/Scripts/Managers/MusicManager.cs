using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

[Singleton]
public class MusicManager : MonoBehaviour
{
    AudioSource a;
    AudioSource b;

    void Awake()
    {
        a = new GameObject("MusicManagerA").AddComponent<AudioSource>();
        b = new GameObject("MusicManagerB").AddComponent<AudioSource>();

        var mixer = Resources.Load<AudioMixer>("Audio/Game").FindMatchingGroups("Music")[0];

        a.transform.SetParent(transform);
        b.transform.SetParent(transform);

        a.outputAudioMixerGroup = mixer;
        b.outputAudioMixerGroup = mixer;

        a.loop = true;
        b.loop = true;
    }

    public void Play(AudioClip music, float fadetime = 0.0f)
    {
        AudioSource target = a.isPlaying ? b : a;
        AudioSource other = target == b ? a : b;

        target.clip = music;
        target.volume = fadetime > 0.0f ? 0.0f : 1.0f;
        if (fadetime == 0.0f)
        {
            other.Stop();
        }
        target.Play();

        if (fadetime > 0.0 && other.isPlaying)
        {
            other.DOFade(0.0f, fadetime).OnComplete(() => other.Stop());
            target.DOFade(1.0f, fadetime);
        }
    }
}
