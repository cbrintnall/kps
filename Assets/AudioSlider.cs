using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class AudioSlider : MonoBehaviour
{
    public string ParameterName;
    public AudioMixerGroup Group;

    // Start is called before the first frame update
    void Start()
    {
        var slider = GetComponent<Slider>();
        slider.minValue = -80.0f;
        slider.maxValue = 0.0f;
        // var mixer = Resources.Load<AudioMixer>($"Audio/Game");
        slider.value = PlayerPrefs.GetFloat(ParameterName);
        Group.audioMixer.SetFloat(ParameterName, slider.value);

        slider.onValueChanged.AddListener(value =>
        {
            Group.audioMixer.SetFloat(ParameterName, value);
            PlayerPrefs.SetFloat(ParameterName, value);
            PlayerPrefs.Save();
        });
    }
}
