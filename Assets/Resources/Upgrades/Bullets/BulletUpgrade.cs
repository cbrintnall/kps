using UnityEngine;

public class BulletData { }

public class HitScanBulletData
{
    public int Pierces;
}

public class BulletUpgrade : Upgrade
{
    public Bullet Bullet;

    public override void OnPickup(PlayerEquipmentController controller)
    {
        base.OnPickup(controller);

        controller.Equipment.Bullet = Bullet;
    }
}
