using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathPanel : MonoBehaviour
{
    Image background;

    void Awake()
    {
        background = GetComponent<Image>();
        background.color = Color.clear;
        var input = SingletonLoader.Get<PlayerInputManager>();
        input.ClearCursors();
        input.PushCursor(CursorLockMode.Confined);
    }

    void Start()
    {
        background.DOColor(Color.black, 2.5f);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Retry()
    {
        SceneManager.LoadScene("ObsidianLibrary");
    }
}
