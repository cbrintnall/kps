using Newtonsoft.Json;

public class StatBehavior : UpgradeBehavior
{
    public StatBehavior(UpgradeData data)
        : base(data) { }

    public override void OnExisting(Upgrade upgrade)
    {
        var reserialized = JsonConvert.SerializeObject(Data.Stats);
        JsonConvert.PopulateObject(reserialized, upgrade);

        upgrade.OnExisting(PlayerEquipmentController.Instance, Data);
    }
}
