using System;
using System.Linq;
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

    public int Highscore => PlayerPrefs.GetInt("highscore", 0);
    public bool Fullscreen => Convert.ToBoolean(PlayerPrefs.GetInt("fullscreen", 1));
    public Tuple<int, int>[] Resolutions =>
        Screen.resolutions.Select(res => Tuple.Create(res.width, res.height)).ToArray();

    public Tuple<int, int> Resolution =>
        Tuple.Create(
            PlayerPrefs.GetInt("res.width", Screen.currentResolution.width),
            PlayerPrefs.GetInt("res.height", Screen.currentResolution.height)
        );

    public int FOV => GetFov();

    private int _cachedFov;

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

    int GetFov()
    {
        if (_cachedFov == 0)
            _cachedFov = PlayerPrefs.GetInt("FOV", 75);

        return _cachedFov;
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

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        PlayerPrefs.SetInt("fullscreen", Convert.ToInt32(fullscreen));
        PlayerPrefs.Save();
    }

    public void SetResolution(Tuple<int, int> resolution)
    {
        Screen.SetResolution(resolution.Item1, resolution.Item2, Fullscreen);
        PlayerPrefs.SetInt("res.width", resolution.Item1);
        PlayerPrefs.SetInt("res.height", resolution.Item2);
        PlayerPrefs.Save();
    }

    public void SetFov(int val)
    {
        int truefov = Mathf.Clamp(val, 50, 140);
        _cachedFov = truefov;
        PlayerPrefs.SetInt("FOV", truefov);
        PlayerPrefs.Save();
    }

    public void SetHighScore(int val)
    {
        PlayerPrefs.SetInt("highscore", val);
        PlayerPrefs.Save();
    }

    void SyncAudioMixer(string parameter, float val)
    {
        mixer.SetFloat(parameter, val);
        PlayerPrefs.SetFloat(parameter, val);
        PlayerPrefs.Save();
    }
}
