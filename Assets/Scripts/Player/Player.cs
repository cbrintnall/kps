using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public PlayerEquipmentController Equipment;

    [IngameDebugConsole.ConsoleMethod("godmode", "Toggles godmode")]
    public static void GodMode(bool toggle)
    {
        Instance.GetComponent<Health>().Invincible = toggle;
    }

    void Awake()
    {
        Instance = this;

        var debugManager = SingletonLoader.Get<DebugManager>();

        debugManager.AddDrawVar(
            new DrawVar() { Name = "Player position", Callback = () => transform.position }
        );
    }
}
