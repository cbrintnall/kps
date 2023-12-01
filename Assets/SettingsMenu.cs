using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public GameObject Root;
    public Button CloseButton;

    public Toggle Fullscreen;
    public TMPro.TMP_Dropdown Resolutions;
    public Slider FOV;
    public TextMeshProUGUI FOVText;

    private PlayerInputManager playerInputManager;
    SettingsManager settingsManager;

    void Awake()
    {
        settingsManager = SingletonLoader.Get<SettingsManager>();
        playerInputManager = SingletonLoader.Get<PlayerInputManager>();

        Fullscreen.isOn = settingsManager.Fullscreen;
        Fullscreen.onValueChanged.AddListener(value =>
        {
            settingsManager.SetFullscreen(value);
        });

        var resolutions = settingsManager.Resolutions;
        var options = resolutions
            .Select(res => new TMP_Dropdown.OptionData($"{res.Item1}x{res.Item2}"))
            .ToList();

        Resolutions.AddOptions(options);

        Resolutions.value = resolutions.ToList().IndexOf(settingsManager.Resolution);
        Resolutions.onValueChanged.AddListener(idx =>
        {
            settingsManager.SetResolution(resolutions[idx]);
        });

        FOV.value = settingsManager.FOV;
        FOV.onValueChanged.AddListener(val =>
        {
            settingsManager.SetFov(Convert.ToInt32(val));
            FOVText.text = Convert.ToInt32(val).ToString();
        });

        FOVText.text = settingsManager.FOV.ToString();
        FOV.minValue = 50;
        FOV.maxValue = 140;
    }

    void Start()
    {
        Open();
        CloseButton.onClick.AddListener(() => Close());

        if (transform.parent.TryGetComponent(out Canvas canvas))
        {
            Destroy(GetComponent<GraphicRaycaster>());
            Destroy(GetComponent<CanvasScaler>());
            Destroy(GetComponent<Canvas>());

            Debug.Log("Settings menu was child of canvas, freeing our own.");
        }

        Close();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInputManager.Cancel.Started)
        {
            Root.SetActive(!Root.activeInHierarchy);

            if (Root.activeInHierarchy)
                Open();
            else
                Close();
        }
    }

    public void Open()
    {
        if (!Root.activeInHierarchy)
            Root.SetActive(true);
        playerInputManager.PushCursor(CursorLockMode.Confined);
        playerInputManager.TogglePause(true);
    }

    public void Close()
    {
        if (Root.activeInHierarchy)
            Root.SetActive(false);
        playerInputManager.PopCursor();
        playerInputManager.TogglePause(false);
        Root.SetActive(false);
    }
}
