using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public DeathPanel DeathPanel;

    void Awake()
    {
        var eventManager = SingletonLoader.Get<EventManager>();

        eventManager.Subscribe<PlayerDeathEvent>(data =>
        {
            Instantiate(DeathPanel, transform);
        });
    }
}
