public class StatApplicator : Upgrade
{
    public string Field;
    public bool IsPercent;
    public object Amount;

    public override void OnPickup(PlayerEquipmentController controller)
    {
        base.OnPickup(controller);
    }

    public override void OnExisting(PlayerEquipmentController controller, UpgradeData data)
    {
        base.OnExisting(controller, data);
    }
}
