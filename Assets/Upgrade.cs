using System;
using UnityEngine;

public struct OnHitEventData { }

public struct OnShotEventData { }

public abstract class Upgrade : MonoBehaviour
{
    public virtual UpgradeScaleType ScaleType => UpgradeScaleType.LINEAR;
    public virtual bool ConsumeDuplicates => false;
    protected virtual Gun Gun => PlayerEquipmentController.Instance.Equipment;
    protected PlayerEquipmentController controller;
    public int Count
    {
        get => count;
        set
        {
            count = value;
            OnCountIncrease(count);
        }
    }

    private int count;

    void Awake()
    {
        Count = 1;
    }

    public virtual void OnPickup(PlayerEquipmentController controller)
    {
        this.controller = controller;
    }

    public virtual void OnWillShootBullet(UpgradePipelineData pipelineData, Bullet bullet) { }

    public virtual void OnBulletMissed() { }

    public virtual void OnBulletHit(UpgradePipelineData pipelineData, BulletHitData data) { }

    public virtual void OnBulletShot(UpgradePipelineData pipelineData, Bullet bullet) { }

    public virtual void OnKick(KickPipelineData data) { }

    protected virtual void OnCountIncrease(int count) { }

    public virtual void OnExisting(PlayerEquipmentController controller, UpgradeData data) { }
}
