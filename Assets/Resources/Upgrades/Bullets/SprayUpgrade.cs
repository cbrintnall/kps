using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class SprayData
{
    public StatFloat Delay = 0.0f;
    public Vector3[] BulletPattern = new Vector3[] { Vector3.zero };
    public StatInt PerBurst = 1;
    public float[] PerBulletDirectionVariance = { 0, 0, 0 };
    public StatFloat BurstCooldown = 0.1f;
}

public class SprayUpgrade : Upgrade
{
    public SprayData Data;

    protected ShootPattern pattern;

    public override void OnPickup(PlayerEquipmentController controller)
    {
        base.OnPickup(controller);

        pattern = controller.Equipment.AddComponent<ShootPattern>();
        pattern.Data = Data;
        controller.Equipment.AddPattern(pattern);
    }
}
