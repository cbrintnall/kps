using System;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(ShootPattern))]
[RequireComponent(typeof(BaseOnHitUpgrade))]
public class Gun : MonoBehaviour
{
    #region Events

    public event Action<Bullet> WillShootBullet;
    public event Action<Bullet> ShotBullet;

    #endregion

    public PlayerEquipmentController Controller;

    public Bullet Bullet;

    [Header("Components")]
    public Transform Barrel;
    public Animator Animator;
    public ShootPattern ShootPattern;

    [Header("Audio")]
    public AudioClip OnShoot;

    private TimeSince ts;
    private AudioManager audioManager;
    private CinemachineImpulseSource impulseSource;

    void Awake()
    {
        audioManager = SingletonLoader.Get<AudioManager>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void Start()
    {
        ts = Controller.Stats.PistolCooldown.Current;
        Cursor.lockState = CursorLockMode.Locked;

        ShootPattern = GetComponent<ShootPattern>();
    }

    public void Use()
    {
        if (ts < Controller.Stats.PistolCooldown)
            return;

        ts = 0;

        ShootPattern.Shoot((variance) => Shoot(variance, null), ShootFX);
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

    public virtual Bullet Shoot(Vector3 variance, Bullet overrideBullet = null)
    {
        var bullet = Instantiate(overrideBullet ?? Bullet);
        var mouseLook = MouseLook.Instance.LookData;

        bullet.transform.position = Barrel.transform.position;
        bullet.Start = mouseLook.StartPoint;
        bullet.Barrel = Barrel;
        bullet.transform.forward = mouseLook.Direction.normalized;
        bullet.transform.Rotate(variance);

        MonitorBullet(bullet);

        WillShootBullet?.Invoke(bullet);
        bullet.Shoot();
        ShotBullet?.Invoke(bullet);

        return bullet;
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
}
