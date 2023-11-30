using System.Collections.Generic;
using IngameDebugConsole;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugData
{
    public string PasteBinKey;
    public string PasteBinUrl;
    public string PasteBinUserKey;
}

public class PowerupStats
{
    public int HealAmount;
    public float ExplosionSize;
    public int ExplosionDamage;
    public float SpeedTime;
    public float SpeedMultiplier;
}

public class RoundData
{
    public float MaxRoundTime;
}

public class LevelingData
{
    public float Offset;
}

public class GameData
{
    public Dictionary<PowerupType, int> PowerupWeights;
    public PowerupStats PowerupStats;
    public Dictionary<UpgradeRarity, Color> UpgradeColors;
    public DebugData Debug;
    public RoundData Rounds;
    public LevelingData Leveling;
}

public enum GameState
{
    MAIN_MENU,
    PAUSE_MENU,
    INTRO,
    PRE_GAME,
    IN_GAME
}

public class GameStateChangedEvent : BaseEvent
{
    public GameState OldState;
    public GameState NewState;
}

[@Singleton]
public class FlowManager : MonoBehaviour
{
    public GameData GameData;
    public GameState CurrentState { get; private set; }

    public AudioClip MainMenuMusic;
    public AudioClip PreGameMusic;
    public AudioClip InGameMusic;

    [ConsoleMethod("reload", "Reloads the round")]
    public static void Restart()
    {
        SingletonLoader.Get<FlowManager>().SetState(GameState.INTRO);
    }

    void Awake()
    {
        var eventManager = SingletonLoader.Get<EventManager>();

        eventManager.Subscribe<PlayGameEvent>(data => SetState(GameState.INTRO));
        eventManager.Subscribe<DoorKickedEvent>(data => SetState(GameState.PRE_GAME));
        eventManager.Subscribe<StartWavesEvent>(data => SetState(GameState.IN_GAME));

        GameData = JsonConvert.DeserializeObject<GameData>(
            Resources.Load<TextAsset>("settings").text
        );

        MainMenuMusic = Resources.Load<AudioClip>("Audio/pregame");
        PreGameMusic = Resources.Load<AudioClip>("Audio/pregame");
        InGameMusic = Resources.Load<AudioClip>("Audio/RemoveThese/ingame");
    }

    void Start()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "MainMenu":
                SetState(GameState.MAIN_MENU);
                break;
            case "ObsidianLibrary":
                SetState(GameState.INTRO);
                break;
        }
    }

    public void SetState(GameState gameState)
    {
        MusicManager musicManager = SingletonLoader.Get<MusicManager>();
        var old = CurrentState;

        switch (gameState)
        {
            case GameState.MAIN_MENU:
                musicManager.Play(MainMenuMusic);
                break;
            case GameState.IN_GAME:
                musicManager.Play(InGameMusic);
                break;
            case GameState.PAUSE_MENU:
                break;
            case GameState.PRE_GAME:
                musicManager.Play(PreGameMusic);
                break;
        }

        CurrentState = gameState;

        SingletonLoader
            .Get<EventManager>()
            .Publish(new GameStateChangedEvent() { NewState = CurrentState, OldState = old });
    }
}
