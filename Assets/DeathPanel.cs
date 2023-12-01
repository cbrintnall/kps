using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathPanel : MonoBehaviour
{
    public TextMeshProUGUI Score;
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
        EnemyMaster enemyMaster = FindObjectOfType<EnemyMaster>();
        SettingsManager settingsManager = SingletonLoader.Get<SettingsManager>();
        int score = enemyMaster.Score;
        bool newHighscore = false;

        if (score > settingsManager.Highscore)
        {
            newHighscore = true;
        }

        string scoreText = $"Score: {score}";

        if (newHighscore)
        {
            settingsManager.SetHighScore(score);
            scoreText += "\n<color=#ebbd34><size=64>(NEW HIGHSCORE)</size></color>";
            SingletonLoader
                .Get<AudioManager>()
                .Play(
                    new AudioPayload()
                    {
                        Clip = Resources.Load<AudioClip>("Audio/Drink potion 4"),
                        Is2D = true
                    }
                );
        }

        Score.text = scoreText;
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
