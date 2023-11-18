using Unity.VisualScripting;
using UnityEngine;

public class MoneyPickup : MonoBehaviour
{
    const float PICKUP_DISTANCE = 1.0f;

    public int Value = 1;
    public float MoveSpeed = 0.05f;
    public AudioClip PickupSound;

    private PlayerEquipmentController target;
    private float targetTime;
    private RoundManager roundManager;

    void Start()
    {
        roundManager = SingletonLoader.Get<RoundManager>();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out PlayerEquipmentController controller))
        {
            target = controller;
        }
    }

    void Update()
    {
        if (
            Vector3.Distance(
                transform.position,
                PlayerEquipmentController.Instance.transform.position
            ) <= PICKUP_DISTANCE
        )
        {
            PlayerEquipmentController.Instance.Money.Incr(Value, StatOperation.Value);

            SingletonLoader
                .Get<AudioManager>()
                .Play(
                    new AudioPayload()
                    {
                        Clip = PickupSound,
                        Location = transform.position,
                        Debounce = 0.5f,
                        PitchWobble = 0.1f
                    }
                );

            Destroy(gameObject);
        }

        if (!roundManager.Active && target == null)
        {
            target = PlayerEquipmentController.Instance;
        }

        if (target)
        {
            targetTime += Mathf.Max(Time.deltaTime, targetTime) * MoveSpeed;

            transform.position = Vector3.Lerp(
                transform.position,
                target.transform.position,
                targetTime
            );
        }
    }
}
