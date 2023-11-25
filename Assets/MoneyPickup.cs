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
    private RoundManager roundManager;

    void Start()
    {
        roundManager = SingletonLoader.Get<RoundManager>();
    }

    void Update()
    {
        if (PlayerEquipmentController.Instance.Health.Dead)
            Destroy(gameObject);

        if (
            Vector3.Distance(
                transform.position,
                PlayerEquipmentController.Instance.transform.position
            ) <= PICKUP_DISTANCE
        )
        {
            int value = (SingletonLoader.Get<RoundManager>().KPS + 1) * Value;
            PlayerEquipmentController.Instance.Money.Incr(value, StatOperation.Value);

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
