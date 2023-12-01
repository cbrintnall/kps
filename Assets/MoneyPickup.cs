using UnityEngine;

public class MoneyPickup : MonoBehaviour
{
    const float PICKUP_DISTANCE = 2.5f;

    public float PickupRadius = 15.0f;
    public int Value = 1;
    public float MoveSpeed = 0.05f;
    public AudioClip PickupSound;

    private PlayerEquipmentController target;
    private float targetTime;

    void Start()
    {
        gameObject.AddComponent<DestroyOnRoundEnd>();
        target = PlayerEquipmentController.Instance;
    }

    void Update()
    {
        if (
            PlayerEquipmentController.Instance == null
            || PlayerEquipmentController.Instance.Health.Dead
        )
            Destroy(gameObject);

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
                        PitchWobble = 0.3f
                    }
                );

            Destroy(gameObject);
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

    void FixedUpdate()
    {
        if (target != null)
            return;

        foreach (
            var collider in Physics.OverlapSphere(
                transform.position,
                PickupRadius,
                LayerMask.GetMask("Player")
            )
        )
        {
            if (collider.TryGetComponent(out PlayerEquipmentController controller))
            {
                target = controller;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, PickupRadius);
    }
}
