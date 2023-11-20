using System;
using System.Collections.Generic;
using KaimiraGames;
using UnityEngine;

public enum PowerupType
{
    HEAL,
    SHOTSPEED,
    BOMB
}

public class Powerup : MonoBehaviour
{
    private static WeightedList<PowerupType> WEIGHTED_POWERUPS;

    public PowerupType Type;
    public AudioClip PickupSound;

    public GameObject Heal;
    public GameObject Speed;
    public GameObject Bomb;

    public int HealAmount => GameSettings.PowerupStats.HealAmount;
    public float SpeedDuration => GameSettings.PowerupStats.SpeedMultiplier;
    public float SpeedMultiplier => GameSettings.PowerupStats.SpeedMultiplier;
    public int BombDamage => GameSettings.PowerupStats.ExplosionDamage;
    public float BombSize => GameSettings.PowerupStats.ExplosionSize;

    private GameData GameSettings;

    void Awake()
    {
        GameSettings = SingletonLoader.Get<FlowManager>().GameData;
    }

    void Start()
    {
        if (WEIGHTED_POWERUPS == null)
        {
            WEIGHTED_POWERUPS = new WeightedList<PowerupType>();

            foreach (var combo in SingletonLoader.Get<FlowManager>().GameData.PowerupWeights)
            {
                WEIGHTED_POWERUPS.Add(combo.Key, combo.Value);
            }
        }

        Type = WEIGHTED_POWERUPS.Next();

        Heal.SetActive(Type == PowerupType.HEAL);
        Speed.SetActive(Type == PowerupType.SHOTSPEED);
        Bomb.SetActive(Type == PowerupType.BOMB);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out PlayerEquipmentController controller))
        {
            switch (Type)
            {
                case PowerupType.SHOTSPEED:
                    controller.AddStatus(
                        SpeedDuration,
                        (controller) =>
                        {
                            controller.Stats.PistolCooldown.Incr(
                                -SpeedMultiplier,
                                out float toRemove,
                                StatOperation.Percent,
                                false
                            );
                            return toRemove;
                        },
                        (controller, toRemove) =>
                        {
                            controller.Stats.PistolCooldown.Incr(toRemove, StatOperation.Value);
                        }
                    );
                    break;
                case PowerupType.HEAL:
                    controller.Health.Heal(new HealPayload() { Amount = HealAmount });
                    break;
                case PowerupType.BOMB:
                    var explosion = Instantiate(
                        Resources.Load<Explosion>("Upgrades/Explosion"),
                        transform.position,
                        Quaternion.identity
                    );

                    explosion.Layers = LayerMask.GetMask("Enemy");
                    explosion.Damage = BombDamage;
                    explosion.Size = BombSize;
                    break;
            }

            SingletonLoader
                .Get<AudioManager>()
                .Play(new AudioPayload() { Clip = PickupSound, Location = transform.position });
        }

        Destroy(gameObject);
    }
}
