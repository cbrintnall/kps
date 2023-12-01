using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using IngameDebugConsole;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerLeveledEvent : BaseEvent
{
    public PlayerEquipmentController Controller;
    public int FromLevel;
    public int ToLevel;
}

public struct InteractionPayload
{
    public GameObject Owner;
    public Vector3 LookDir;
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

    public event Action<int> PlayerLeveled;

    #region stats
    public StatFloat ExplosionSize = 4.0f;
    public StatFloat UpgradeDropChance = 0.25f;
    #endregion

    private static Type[] DefaultUpgrades = new Type[]
    {
        typeof(DefaultKickUpgrade),
        typeof(BulletRichocet),
        typeof(ExplosiveKick)
    };

    public StatInt Money = 0;
    public Gun Equipment;
    public Animator Legs;
    public AnimationEventsHandler Handler;
    public StatBlock Stats = new();
    public Material HealthMaterial;
    public Material XPMaterial;
    public Health Health;
    public int level = 1;

    [Header("Audio")]
    public AudioClip UpgradePickup;
    public AudioClip HitSound;
    public AudioClip CritSound;

    private CinemachineImpulseSource impulseSource;
    private MouseLook mouseLook;
    private IInteractable lookingAt;
    private Dictionary<string, UpgradeStorage> upgrades = new();
    private PlayerInputManager playerInputManager;
    private AudioManager audioManager;
    private PlayerMovement playerMovement;
    private bool amDead;
    private int requiredXP => Curves.GetRequiredXP(level);
    private float smoothedMoney = 0.0f;
    private float airTime;
    private float normalizedXP =>
        Mathf.InverseLerp(
            (float)Mathf.Max(Curves.GetRequiredXP(level - 1), 0.0f),
            (float)requiredXP,
            (float)Money
        );

    // Mathf.RoundToInt(Mathf.Log10((level + 1) * 100) * (Mathf.Pow(level + 1, 2.0f) / 10.0f))
    // + Mathf.RoundToInt(Mathf.Log10(level * 100) * (Mathf.Pow(level, 2.0f) / 10.0f));

    [ConsoleMethod("die", "insta kills you, dumby")]
    public static void Die()
    {
        Instance.Health.Damage(Instance.Health.Data.Current);
    }

    public void AddUpgrade(UpgradeData data)
    {
        if (upgrades.TryGetValue(data.GenerateBaseId(), out UpgradeStorage existing))
        {
            data.Behavior.OnExisting(existing.Upgrade);
            audioManager.Play(
                new AudioPayload()
                {
                    Clip = UpgradePickup,
                    PitchWobble = 0.1f,
                    Location = transform.position
                }
            );
            return;
        }

        var upgrade = data.Behavior.CreateForOwner(this);
        upgrades[data.GenerateBaseId()] = new UpgradeStorage() { Data = data, Upgrade = upgrade };
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

    public bool TryGetUpgrade(UpgradeData data, out UpgradeStorage storage)
    {
        return TryGetUpgrade(data.GenerateBaseId(), out storage);
    }

    public bool TryGetUpgrade(string id, out UpgradeStorage outgrade)
    {
        outgrade = null;
        if (upgrades.TryGetValue(id, out UpgradeStorage value))
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

    public UpgradePipeline CreatePipeline(Gun equipment, UpgradePipelineData existing = null)
    {
        return new UpgradePipeline(
            upgrades.Values.Select(storage => storage.Upgrade),
            existing
                ?? new UpgradePipelineData() { CameraShake = impulseSource, PlayerStats = Stats },
            equipment
        );
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
        XPMaterial.SetFloat("_NormalizedValue", normalizedXP);
        var eventManager = SingletonLoader.Get<EventManager>();
        playerInputManager.PushCursor(CursorLockMode.Locked);

        Money.ValueChanged += (current, delta) =>
        {
            XPMaterial.SetFloat("_Amount", normalizedXP);
            eventManager.Publish(new MoneyChangedEvent() { Gained = delta });
            int levelBefore = level;
            while (Money >= requiredXP)
            {
                level++;
                SingletonLoader
                    .Get<EventManager>()
                    .Publish(new PlayerLeveledEvent() { Controller = this, ToLevel = level });
            }
            if (levelBefore != level)
            {
                // TODO: put this back in
                // SingletonLoader
                //     .Get<AudioManager>()
                //     .Play(
                //         new AudioPayload()
                //         {
                //             Clip = Resources.Load<AudioClip>("Audio/bell"),
                //             Is2D = true,
                //             PitchWobble = 0.1f
                //         }
                //     );
            }
        };

        Money.Set(Curves.GetRequiredXP(level - 1));
        XPMaterial.SetFloat("_Amount", normalizedXP);

        Health.Data.ValueChanged += (current, delta) =>
        {
            HealthMaterial.SetFloat("_NormalizedValue", Health.Data.Normalized);
            if (current <= 0)
            {
                OnDeath();
            }
        };
        foreach (var upgradeType in DefaultUpgrades)
        {
            gameObject.AddComponent(upgradeType);
        }
        foreach (var upgrade in GetComponents<Upgrade>())
        {
            upgrades[$"{upgrade.GetType().Name}.default"] = new UpgradeStorage()
            {
                Upgrade = upgrade
            };
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
        Equipment = equipment;
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

        // Copy list here only, since kicking can lead to modifying the collection.
        foreach (var upgrade in upgrades.ToList())
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
        Crosshair.Instance.Hint.text = "";

        Equipment.OverrideStart = Tuple.Create(false, Vector3.zero);

        if (
            Physics.Raycast(
                mouseLook.LookData.StartPoint,
                (Equipment.Barrel.transform.position - mouseLook.LookData.StartPoint).normalized,
                Vector3.Distance(
                    Equipment.Barrel.transform.position,
                    mouseLook.LookData.StartPoint
                ),
                LayerMask.GetMask("Default")
            )
        )
        {
            Equipment.OverrideStart = Tuple.Create(true, mouseLook.LookData.EndPoint);
        }

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
                Crosshair.Instance.Hint.text = "'F'";
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
