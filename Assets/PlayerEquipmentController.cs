using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using IngameDebugConsole;
using Unity.VisualScripting;
using UnityEngine;

public struct InteractionPayload
{
    public GameObject Owner;
}

public class PlayerDeathEvent : BaseEvent { }

public class InteractionData
{
    public string Title;
    public string Description;
}

public interface IInteractable
{
    void Interact(InteractionPayload payload);
    InteractionData GetData();
}

public class UpgradeStorage
{
    public UpgradeData Data;
    public Upgrade Upgrade;
}

[RequireComponent(typeof(MouseLook))]
public class PlayerEquipmentController : MonoBehaviour
{
    public static PlayerEquipmentController Instance;

    #region stats
    public StatFloat ExplosionSize = 4.0f;
    public StatFloat UpgradeDropChance = 0.25f;
    #endregion

    public StatInt Money = 0;
    public Gun Equipment;
    public Animator Legs;
    public AnimationEventsHandler Handler;
    public StatBlock Stats = new();
    public Material HealthMaterial;
    public Health Health;

    [Header("Audio")]
    public AudioClip UpgradePickup;
    public AudioClip HitSound;

    private CinemachineImpulseSource impulseSource;
    private MouseLook mouseLook;
    private IInteractable lookingAt;
    private Dictionary<Type, UpgradeStorage> upgrades = new();
    private PlayerInputManager playerInputManager;
    private AudioManager audioManager;
    private PlayerMovement playerMovement;
    private bool amDead;

    [ConsoleMethod("die", "insta kills you, dumby")]
    public static void Die()
    {
        Instance.Health.Damage(Instance.Health.Data.Current);
    }

    public void AddUpgrade(UpgradeData data)
    {
        if (upgrades.TryGetValue(data.Class, out UpgradeStorage existing))
        {
            data.Behavior.OnExisting(existing.Upgrade);
            return;
        }

        var upgrade = data.Behavior.CreateForOwner(gameObject);
        upgrades[data.Class] = new UpgradeStorage() { Data = data, Upgrade = upgrade };
        SeedUpgrade(upgrade);

        audioManager.Play(
            new AudioPayload()
            {
                Clip = UpgradePickup,
                PitchWobble = 0.1f,
                Location = transform.position
            }
        );
    }

    public bool TryGetUpgrade(Type upgrade, out UpgradeStorage outgrade)
    {
        outgrade = null;
        if (upgrades.TryGetValue(upgrade, out UpgradeStorage value))
        {
            outgrade = value;
            return true;
        }
        return false;
    }

    public Vector3 GetGroundPosition()
    {
        if (
            Physics.Raycast(
                transform.position,
                Vector3.down,
                out var hit,
                1000.0f,
                LayerMask.GetMask("Default")
            )
        )
        {
            return hit.point;
        }

        return Vector3.one;
    }

    public void AddStatus<T>(
        float time,
        Func<PlayerEquipmentController, T> start,
        Action<PlayerEquipmentController, T> end
    )
    {
        StartCoroutine(StartStatus(time, start, end));
    }

    IEnumerator StartStatus<T>(
        float time,
        Func<PlayerEquipmentController, T> start,
        Action<PlayerEquipmentController, T> end
    )
    {
        var ret = start(this);
        yield return new WaitForSeconds(time);
        end(this, ret);
    }

    void OnDeath()
    {
        if (amDead)
            return;

        amDead = true;
        MouseLook.Instance.Camera
            .AddComponent<Rigidbody>()
            .AddForce(PlayerMovement.Instance.m_PlayerVelocity, ForceMode.VelocityChange);

        MouseLook.Instance.Camera.AddComponent<BoxCollider>();
        SingletonLoader.Get<EventManager>().Publish(new PlayerDeathEvent());
    }

    void Awake()
    {
        Instance = this;
        audioManager = SingletonLoader.Get<AudioManager>();
        playerMovement = GetComponent<PlayerMovement>();
        mouseLook = GetComponent<MouseLook>();
        Equipment.Controller = this;
        impulseSource = GetComponent<CinemachineImpulseSource>();
        Handler.LegSwung += DoInteraction;
        playerInputManager = SingletonLoader.Get<PlayerInputManager>();
        DebugLogConsole.AddCommandInstance("money", "Gives money", "GiveMoney", this, "amount");
        Health = GetComponent<Health>();
        HealthMaterial.SetFloat("_NormalizedValue", Health.Data.Normalized);
        var eventManager = SingletonLoader.Get<EventManager>();

        Money.ValueChanged += (current, delta) =>
        {
            eventManager.Publish(new MoneyChangedEvent() { Gained = delta });
        };

        Health.Data.ValueChanged += (current, delta) =>
        {
            HealthMaterial.SetFloat("_NormalizedValue", Health.Data.Normalized);
            if (current <= 0)
            {
                OnDeath();
            }
        };
        foreach (var upgrade in GetComponents<Upgrade>())
        {
            upgrades[upgrade.GetType()] = new UpgradeStorage() { Upgrade = upgrade };
            SeedUpgrade(upgrade);
        }
        PickupWeapon(Equipment);
    }

    void SeedUpgrade(Upgrade upgrade)
    {
        upgrade.OnPickup(this);
    }

    void PickupWeapon(Gun equipment)
    {
        if (Equipment)
        {
            Equipment.WillShootBullet -= OnWillShootBullet;
            Equipment.ShotBullet -= OnShotBullet;
        }

        Equipment = equipment;
        Equipment.WillShootBullet += OnWillShootBullet;
        Equipment.ShotBullet += OnShotBullet;
    }

    void OnShotBullet(Bullet bullet)
    {
        var pipeline = new UpgradePipelineData() { PlayerStats = Stats };
        foreach (var upgrade in upgrades)
        {
            upgrade.Value.Upgrade.OnBulletShot(pipeline, bullet);
        }
    }

    void OnWillShootBullet(Bullet bullet)
    {
        var pipeline = new UpgradePipelineData() { PlayerStats = Stats };
        foreach (var upgrade in upgrades)
        {
            upgrade.Value.Upgrade.OnWillShootBullet(pipeline, bullet);
        }
        bullet.Hit += (data) =>
        {
            this.PlayAtMe(
                new AudioPayload()
                {
                    Clip = HitSound,
                    Location = transform.position,
                    PitchWobble = 0.1f,
                    Debounce = 0.1f
                }
            );

            foreach (var upgrade in upgrades)
            {
                upgrade.Value.Upgrade.OnBulletHit(pipeline, data);
            }
        };
    }

    void GiveMoney(int amt)
    {
        Money.Incr(amt, StatOperation.Value);
    }

    // Update is called once per frame
    void Update()
    {
        if (amDead)
            return;

        if (Equipment)
        {
            if (playerInputManager.OnPrimaryAction)
            {
                Equipment.Use();
            }

            if (playerInputManager.OnSecondaryAction)
            {
                Equipment.AlternateUse();
            }
        }

        if (playerInputManager.OnInteract)
        {
            Legs.SetTrigger("Interact");
        }
    }

    void DoInteraction()
    {
        var lookData = MouseLook.Instance.LookData;
        var hits = Physics.RaycastAll(
            lookData.StartPoint,
            lookData.Direction,
            2.0f,
            LayerMask.GetMask("Interactable", "Enemy")
        );

        var healths = hits.Select(val =>
            {
                if (val.collider.TryGetComponent(out Health health))
                {
                    return health;
                }
                return null;
            })
            .Where(val => val != null)
            .ToArray();

        foreach (var upgrade in upgrades)
        {
            upgrade.Value.Upgrade.OnKick(
                new KickPipelineData()
                {
                    LookData = lookData,
                    LookingAt = lookingAt,
                    Health = healths,
                    CameraShake = impulseSource,
                    PlayerMovement = playerMovement
                }
            );
        }
    }

    void FixedUpdate()
    {
        if (amDead)
            return;
        var lookData = mouseLook.LookData;
        lookingAt = null;

        if (
            Physics.Raycast(
                lookData.StartPoint,
                lookData.Direction,
                out var hit,
                2.0f,
                LayerMask.GetMask("Interactable", "Enemy")
            )
        )
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                lookingAt = interactable;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (mouseLook)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(mouseLook.LookData.EndPoint, 0.2f);
        }
    }
}
