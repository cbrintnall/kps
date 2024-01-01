using IngameDebugConsole;

public static class PlayerCommands
{
    [ConsoleMethod("health", "change health")]
    public static void Health(int amt)
    {
        PlayerEquipmentController.Instance.Health.Damage(amt);
    }
}
