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

public class ShootRequestPayload
{
    public UpgradePipelineData PipelineData;
    public Vector3 Variance;
    public Bullet BulletPrefab;
    public ShootPayload Payload = new();
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
    public ShootPattern Pattern;

    [Header("Audio")]
    public AudioClip OnShoot;

    [Header("FX")]
    public GameObject ShotFlare;
    public Tuple<bool, Vector3> OverrideStart;

    [SerializeField]
    private ChargeOrb chargeFX;
    private TimeSince ts;
    private AudioManager audioManager;
    private CinemachineImpulseSource impulseSource;

    [Sirenix.OdinInspector.ReadOnly]
    private float charge;

    void Awake()
    {
        ShotFlare.SetActive(false);
        audioManager = SingletonLoader.Get<AudioManager>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        AddPattern(GetComponent<ShootPattern>());
    }

    void Start()
    {
        SingletonLoader.Get<PlayerInputManager>().PushCursor(CursorLockMode.Locked);
    }

    public void NotifyPickedUp()
    {
        ts = Controller.Stats.PistolCooldown.Current;
    }

    public void NotifyDropped(Vector3 dropPoint)
    {
        gameObject.SetLayerForMeAndChildren(LayerMask.NameToLayer("Default"));
        var pickup = new GameObject($"{name}-pickup").AddComponent<WeaponPickup>();
        pickup.transform.position = dropPoint;
        transform.SetParent(pickup.transform);
        transform.LocalReset();
    }

    public void AddPattern(ShootPattern pattern)
    {
        Destroy(Pattern);
        Pattern = pattern;
    }

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
            if (hit.Health)
            {
                bool didCrit = Controller.Stats.CritWithDamage(
                    Controller.Stats.HitScanDamage,
                    out int damage
                );

                hit.Health.Damage(
                    new DamagePayload()
                    {
                        Amount = damage,
                        Owner = PlayerEquipmentController.Instance.gameObject,
                        WasCrit = didCrit
                    }
                );

                audioManager.Play(
                    new AudioPayload()
                    {
                        Clip = didCrit ? Controller.CritSound : Controller.HitSound,
                        Is2D = true,
                        PitchWobble = didCrit ? 1.0f : 0.0f
                    }
                );
            }
        };
    }

    /// <summary>
    /// Allows sending a bullet through the gun's pipeline as if it was fired from this gun.
    /// This method will handle calling shoot.
    /// </summary>
    /// <param name="bullet">The bullet to be fired, should already be setup and instantiated</param>
    public void VirtualShoot(Bullet bullet)
    {
        MonitorBullet(bullet);
        WillShootBullet?.Invoke(bullet);
        bullet.Shoot();
        ShotBullet?.Invoke(bullet);
    }

    public virtual Bullet Shoot(
        Vector3 variance,
        Bullet overrideBullet = null,
        ShootPayload payload = null
    )
    {
        return Shoot(
            new ShootRequestPayload()
            {
                Variance = variance,
                BulletPrefab = overrideBullet,
                Payload = payload
            }
        );
    }

    public virtual Bullet Shoot(ShootRequestPayload request)
    {
        var bullet = Instantiate(request.BulletPrefab ?? Bullet);
        var mouseLook = MouseLook.Instance.LookData;

        Controller.CreatePipeline(this, request.PipelineData);

        bullet.transform.position = Barrel.transform.position;
        bullet.Start = mouseLook.StartPoint;
        bullet.Barrel = Barrel;
        bullet.transform.forward = mouseLook.Direction.normalized;
        bullet.transform.Rotate(request.Variance);
        bullet.ExtraData = request.Payload;

        if (bullet is GrenadeBullet gb)
        {
            gb.Force += PlayerMovement.Instance.m_PlayerVelocity.magnitude;
        }

        VirtualShoot(bullet);

        return bullet;
    }

    public virtual void OnShot(ShootRequestPayload request = null)
    {
        charge = 0.0f;
        Pattern.Shoot(
            (variance) =>
            {
                ShootRequestPayload usedPayload = request;
                if (request != null)
                {
                    request.Variance = variance;
                }
                else
                {
                    usedPayload = new ShootRequestPayload() { Variance = variance };
                }

                return Shoot(usedPayload);
            },
            ShootFX
        );
    }

    protected virtual void ShootFX()
    {
        audioManager.Play(new AudioPayload() { Clip = OnShoot, Transform = transform });

        if (Animator)
        {
            Animator.SetTrigger("Shoot");
        }

        impulseSource.GenerateImpulse();

        ShotFlare.SetActive(true);
        this.WaitThen(0.1f, () => ShotFlare.SetActive(false));
    }

    void Update()
    {
        if (chargeFX)
        {
            chargeFX.Charge = charge;
            chargeFX.MaxChargeValue = StageChargeTime * MaxChargeStage;
        }
    }

    void FixedUpdate()
    {
        if (chargeFX)
        {
            charge = Mathf.Max(charge - 0.005f, 0.0f);
        }
    }
}
