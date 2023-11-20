using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class UsageData
{
    public float HoldTime;
}

public class ShootPayload
{
    public float ChargeTime = 0.0f;
}

[RequireComponent(typeof(BaseOnHitUpgrade))]
public abstract class Gun : MonoBehaviour
{
    #region Events
    public event Action<Bullet> WillShootBullet;
    public event Action<Bullet> ShotBullet;
    #endregion

    public StatFloat StageChargeTime = 2.0f;
    public StatInt MaxChargeStage = 0;

    public PlayerEquipmentController Controller;

    public Bullet Bullet;

    [Header("Components")]
    public Transform Barrel;
    public Animator Animator;
    public List<ShootPattern> ShootPatterns = new();

    [Header("Audio")]
    public AudioClip OnShoot;

    [SerializeField]
    private ChargeOrb chargeFX;
    private TimeSince ts;
    private AudioManager audioManager;
    private CinemachineImpulseSource impulseSource;

    [Sirenix.OdinInspector.ReadOnly]
    private float charge;

    void Awake()
    {
        audioManager = SingletonLoader.Get<AudioManager>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        ShootPatterns.AddRange(GetComponents<ShootPattern>());
    }

    void Start()
    {
        ts = Controller.Stats.PistolCooldown.Current;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void AddPattern(ShootPattern pattern) => ShootPatterns.Add(pattern);

    public void Use()
    {
        if (ts < Controller.Stats.PistolCooldown)
            return;

        ts = 0;

        OnShot();
    }

    public void AlternateUse()
    {
        if (ts < Controller.Stats.PistolCooldown)
            return;

        charge = Mathf.Clamp(
            charge + Controller.Stats.ChargeSpeed,
            0.0f,
            StageChargeTime * MaxChargeStage
        );
    }

    public void MonitorBullet(Bullet bullet)
    {
        bullet.Hit += (hit) =>
        {
            bool didCrit = Controller.Stats.CritWithDamage(
                Controller.Stats.HitScanDamage,
                out int damage
            );

            hit.Health?.Damage(
                new DamagePayload()
                {
                    Amount = damage,
                    Owner = PlayerEquipmentController.Instance.gameObject
                }
            );
        };
    }

    public virtual Bullet Shoot(
        Vector3 variance,
        Bullet overrideBullet = null,
        ShootPayload payload = null
    )
    {
        var bullet = Instantiate(overrideBullet ?? Bullet);
        var mouseLook = MouseLook.Instance.LookData;

        bullet.transform.position = Barrel.transform.position;
        bullet.Start = mouseLook.StartPoint;
        bullet.Barrel = Barrel;
        bullet.transform.forward = mouseLook.Direction.normalized;
        bullet.transform.Rotate(variance);
        bullet.ExtraData = payload;

        MonitorBullet(bullet);

        WillShootBullet?.Invoke(bullet);
        bullet.Shoot();
        ShotBullet?.Invoke(bullet);

        return bullet;
    }

    protected virtual void OnShot()
    {
        charge = 0.0f;
    }

    protected virtual void ShootFX()
    {
        audioManager.Play(new AudioPayload() { Clip = OnShoot, Transform = transform });

        if (Animator)
        {
            Animator.SetTrigger("Shoot");
        }

        impulseSource.GenerateImpulse();
    }

    void Update()
    {
        chargeFX.Charge = charge;
        chargeFX.MaxChargeValue = StageChargeTime * MaxChargeStage;
    }

    void FixedUpdate()
    {
        charge = Mathf.Max(charge - 0.005f, 0.0f);
    }
}
