using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class TransactionPayload
{
    public UpgradeData Data;
    public PlayerEquipmentController SelectedBy;
    public Upgrade Upgrade;
}

public class VendingMachine : MonoBehaviour, IInteractable
{
    public static bool Free = false;

    public UpgradeData Upgrade
    {
        get => _upgrade;
        set
        {
            _upgrade = value;
            ui.Upgrade = value;
        }
    }

    public AudioClip PurchaseSound;
    public AudioClip RejectSound;
    public AudioClip InteractedSound;
    public float WobbleAmount = 8.0f;

    [SerializeField]
    private VendingMachineUI ui;
    private UpgradeData _upgrade;
    private UpgradesManager upgradesManager;

    [IngameDebugConsole.ConsoleMethod("free", "makes all vending machines free")]
    public static void MakeFree(bool state) => Free = state;

    [IngameDebugConsole.ConsoleMethod("reroll", "forces all vending machines to reroll")]
    public static void RerollAll()
    {
        // TODO: this will eventually be done by an in-game object, but it can't
        // be done on a per machine basis since this list is stateful.
        SingletonLoader.Get<UpgradesManager>().UpdateUpgradeList();

        foreach (var machine in FindObjectsOfType<VendingMachine>())
        {
            machine.Reroll();
        }
    }

    void Start()
    {
        upgradesManager = SingletonLoader.Get<UpgradesManager>();
        Reroll();
    }

    public void Reroll()
    {
        if (upgradesManager.TryGetViableUpgrade(out UpgradeData data))
        {
            Upgrade = data;
        }
        else
        {
            Upgrade = null;
        }
    }

    public InteractionData GetData()
    {
        return new InteractionData()
        {
            Description = "I am a vending machine",
            Title = "Vending machine"
        };
    }

    public void ClearStock()
    {
        Upgrade = null;
    }

    public void Interact(InteractionPayload payload)
    {
        if (payload.Owner.TryGetComponent(out PlayerEquipmentController controller))
        {
            AudioPayload audioPayload = new AudioPayload()
            {
                Location = transform.position,
                Clip = RejectSound
            };

            if (Upgrade != null && (controller.Money >= Upgrade.Cost || Free))
            {
                audioPayload.Clip = PurchaseSound;
                Debug.Log($"bzzt, dispensing upgrade {Upgrade.Class}");
                controller.AddUpgrade(Upgrade);
                controller.Money.Incr(-Upgrade.Cost, StatOperation.Value);
            }

            var manager = SingletonLoader.Get<AudioManager>();

            manager.Play(audioPayload);
            manager.Play(
                new AudioPayload()
                {
                    Clip = InteractedSound,
                    Location = transform.position,
                    PitchWobble = 0.25f,
                    Volume = 0.75f
                }
            );

            transform.DOPunchRotation(
                new Vector3(
                    Utilities.Randf() * WobbleAmount,
                    0.0f,
                    Utilities.Randf() * WobbleAmount
                ),
                0.25f
            );
        }
    }
}
