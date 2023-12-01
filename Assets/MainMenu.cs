using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayGameEvent : BaseEvent { }

public class MainMenu : MonoBehaviour
{
    public GameObject Skull;
    public Camera SkullCamera;
    public Button Options;
    public SettingsMenu OptionsMenu;
    public TextMeshProUGUI HighscoreText;

    public float SkullLookSpeed = 2.0f;

    void Start()
    {
        int highscore = SingletonLoader.Get<SettingsManager>().Highscore;
        HighscoreText.gameObject.SetActive(highscore > 0);

        if (highscore > 0)
        {
            HighscoreText.text = $"Highscore: {highscore} kills";
        }
    }

    public void Play()
    {
        SingletonLoader.Get<EventManager>().Publish(new PlayGameEvent());
        SceneManager.LoadScene("ObsidianLibrary");
    }

    public void Quit() => Application.Quit();

    void Update()
    {
        var t = SkullCamera.ScreenToWorldPoint(Input.mousePosition);

        Skull.transform.forward = Vector3.Lerp(
            Skull.transform.forward,
            t,
            Time.deltaTime * SkullLookSpeed
        );
    }
}
