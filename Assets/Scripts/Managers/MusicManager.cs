using DG.Tweening;
using UnityEngine;

[Singleton]
public class MusicManager : MonoBehaviour
{
    AudioSource a;
    AudioSource b;

    void Awake()
    {
        a = new GameObject("MusicManagerA").AddComponent<AudioSource>();
        b = new GameObject("MusicManagerB").AddComponent<AudioSource>();
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
