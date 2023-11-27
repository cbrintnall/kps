using UnityEngine;

public class StartWavesEvent : BaseEvent { }

public class SoulFountain : MonoBehaviour, IInteractable
{
    public AudioClip StartSound;

    private RoundManager roundManager;
    private AudioManager audioManager;

    void Awake()
    {
        audioManager = SingletonLoader.Get<AudioManager>();
        roundManager = FindObjectOfType<RoundManager>();
    }

    public InteractionData GetData()
    {
        return new InteractionData()
        {
            Title = "Soul Fountain",
            Description = "Start the next round"
        };
    }

    public void Interact(InteractionPayload payload)
    {
        if (!roundManager.Active)
        {
            SingletonLoader.Get<EventManager>().Publish(new StartWavesEvent());
            roundManager.StartRound();
            audioManager.Play(
                new AudioPayload()
                {
                    Clip = StartSound,
                    PitchWobble = .05f,
                    Location = transform.position
                }
            );
        }
    }
}
