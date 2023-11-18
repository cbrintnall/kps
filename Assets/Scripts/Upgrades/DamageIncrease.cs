public class DamageIncrease : Upgrade
{
    public StatFloat PercentageIncrease = 0.1f;

    public override void OnPickup(PlayerEquipmentController controller)
    {
        controller.Stats.RocketDamage.IncrPercent(PercentageIncrease, false);
        controller.Stats.HitScanDamage.IncrPercent(PercentageIncrease, false);
        controller.Stats.GrenadeExplosiveDamage.IncrPercent(PercentageIncrease, false);
        controller.Stats.GrenadeHitDamage.IncrPercent(PercentageIncrease, false);
    }
}
