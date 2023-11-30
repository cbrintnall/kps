using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class AudioSlider : MonoBehaviour
{
    public AudioType Type;

    private SettingsManager settingsManager;

    void Awake()
    {
        settingsManager = SingletonLoader.Get<SettingsManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        var slider = GetComponent<Slider>();
        slider.minValue = -80.0f;
        slider.maxValue = 0.0f;
        slider.value = settingsManager.GetAudio(Type);
        slider.onValueChanged.AddListener(value =>
        {
            settingsManager.SetAudio(Type, value);
        });
    }
}
