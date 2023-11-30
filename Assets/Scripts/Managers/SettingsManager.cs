using UnityEngine;
using UnityEngine.Audio;

public enum AudioType
{
    Master,
    SFX,
    Music
}

[Singleton]
public class SettingsManager : MonoBehaviour
{
    private const string SFX = "SFXVolume";
    private const string Music = "MusicVolume";
    private const string Master = "MasterVolume";

    AudioMixer mixer;

    void Awake()
    {
        mixer = Resources.Load<AudioMixer>($"Audio/Game");
    }

    void Start()
    {
        mixer.SetFloat(SFX, PlayerPrefs.GetFloat(SFX));
        mixer.SetFloat(Music, PlayerPrefs.GetFloat(Music));
        mixer.SetFloat(Master, PlayerPrefs.GetFloat(Master, -40.0f));
    }

    public float GetAudio(AudioType type)
    {
        switch (type)
        {
            case AudioType.Master:
                return PlayerPrefs.GetFloat(Master);
            case AudioType.SFX:
                return PlayerPrefs.GetFloat(SFX);
            case AudioType.Music:
                return PlayerPrefs.GetFloat(Music);
        }

        return 0.0f;
    }

    public void SetAudio(AudioType type, float value)
    {
        switch (type)
        {
            case AudioType.Master:
                SetMaster(value);
                break;
            case AudioType.SFX:
                SetSFX(value);
                break;
            case AudioType.Music:
                SetMusic(value);
                break;
        }
    }

    public void SetMaster(float val)
    {
        SyncAudioMixer(Master, val);
    }

    public void SetSFX(float val)
    {
        SyncAudioMixer(SFX, val);
    }

    public void SetMusic(float val)
    {
        SyncAudioMixer(Music, val);
    }

    void SyncAudioMixer(string parameter, float val)
    {
        mixer.SetFloat(parameter, val);
        PlayerPrefs.SetFloat(parameter, val);
        PlayerPrefs.Save();
    }
}
