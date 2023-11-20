using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(BaseOnHitUpgrade))]
public abstract class Gun : MonoBehaviour
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
    public List<ShootPattern> ShootPatterns = new();

    [Header("Audio")]
    public AudioClip OnShoot;

    private TimeSince ts;
    private AudioManager audioManager;
    private CinemachineImpulseSource impulseSource;

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

    protected abstract void OnShot();

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
