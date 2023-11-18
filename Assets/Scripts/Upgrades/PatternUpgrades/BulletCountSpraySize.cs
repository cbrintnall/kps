public class BulletCountSpraySize : Upgrade
{
    public int BulletAmount = 1;
    public float SprayAmount = 0.25f;

    public override void OnPickup(PlayerEquipmentController controller)
    {
        base.OnPickup(controller);

        // Gun.ShootPattern.PerShot.Incr(BulletAmount, StatOperation.Value);
        // Gun.ShootPattern.PerBulletDirectionVariance[0].Incr(SprayAmount, StatOperation.Value);
        // Gun.ShootPattern.PerBulletDirectionVariance[1].Incr(SprayAmount, StatOperation.Value);
        // Gun.ShootPattern.PerBulletDirectionVariance[2].Incr(SprayAmount, StatOperation.Value);
    }
}
