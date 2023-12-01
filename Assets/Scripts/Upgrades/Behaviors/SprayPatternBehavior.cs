using Newtonsoft.Json;

public class SprayPatternBehavior : UpgradeBehavior
{
    public SprayPatternBehavior(UpgradeData data)
        : base(data) { }

    public override void OnExisting(Upgrade upgrade)
    {
        var reserialized = JsonConvert.SerializeObject(Data.Stats);
        JsonConvert.PopulateObject(reserialized, upgrade);
    }
}
