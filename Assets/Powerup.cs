using UnityEngine;

public enum PowerupType
{
    HEAL,
    SHOTSPEED
}

public class Powerup : MonoBehaviour
{
    public PowerupType Type;
    public AudioClip PickupSound;

    public GameObject Heal;
    public GameObject Speed;

    public int HealAmount = 1;
    public float SpeedDuration = 5.0f;
    public float SpeedMultiplier = 0.5f;

    void Start()
    {
        Type = Utilities.Randf() <= 0.5f ? PowerupType.HEAL : PowerupType.SHOTSPEED;

        switch (Type)
        {
            case PowerupType.SHOTSPEED:
                Heal.SetActive(false);
                Speed.SetActive(true);
                break;
            case PowerupType.HEAL:
                Heal.SetActive(true);
                Speed.SetActive(false);
                break;
        }
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
                            var before = controller.Stats.PistolCooldown.Current;
                            controller.Stats.PistolCooldown.Incr(
                                -SpeedMultiplier,
                                out float toRemove,
                                StatOperation.Percent,
                                false
                            );
                            var after = controller.Stats.PistolCooldown.Current;

                            Debug.Log($"Before: {before}, after: {after}, to remove: {toRemove}");

                            return toRemove;
                        },
                        (controller, toRemove) =>
                        {
                            Debug.Log("Removing");
                            var before = controller.Stats.PistolCooldown.Current;
                            controller.Stats.PistolCooldown.Incr(toRemove, StatOperation.Value);
                            var after = controller.Stats.PistolCooldown.Current;
                            Debug.Log($"Before: {before}, after: {after}, to remove: {toRemove}");
                        }
                    );
                    break;
                case PowerupType.HEAL:
                    controller.Health.Heal(new HealPayload() { Amount = HealAmount });
                    break;
            }

            SingletonLoader
                .Get<AudioManager>()
                .Play(new AudioPayload() { Clip = PickupSound, Location = transform.position });
        }

        Destroy(gameObject);
    }
}
