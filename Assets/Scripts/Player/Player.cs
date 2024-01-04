using UnityEngine;

public class PlayerSpawnedEvent : BaseEvent { }

public class Player : MonoBehaviour
{
    public static Player Instance;

    public PlayerEquipmentController Equipment;
    public PlayerMovement Movement;

    [IngameDebugConsole.ConsoleMethod("godmode", "Toggles godmode")]
    public static void GodMode(bool toggle)
    {
        Instance.GetComponent<Health>().Invincible = toggle;
    }

    void Awake()
    {
        Instance = this;

        Movement = GetComponent<PlayerMovement>();
        Equipment = GetComponent<PlayerEquipmentController>();

        var debugManager = SingletonLoader.Get<DebugManager>();

        debugManager.AddDrawVar(
            new DrawVar() { Name = "Player position", Callback = () => transform.position }
        );
    }

    void Start()
    {
        SingletonLoader.Get<EventManager>().Publish(new PlayerSpawnedEvent());
    }
}
