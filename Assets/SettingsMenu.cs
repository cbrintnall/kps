using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public GameObject Root;
    public Button CloseButton;

    private PlayerInputManager playerInputManager;

    void Awake()
    {
        playerInputManager = SingletonLoader.Get<PlayerInputManager>();
    }

    void Start()
    {
        Open();
        CloseButton.onClick.AddListener(() => Close());
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

    void Open()
    {
        playerInputManager.PushCursor(CursorLockMode.Confined);
        playerInputManager.TogglePause(true);
    }

    void Close()
    {
        playerInputManager.PopCursor();
        playerInputManager.TogglePause(false);
        Root.SetActive(false);
    }
}
