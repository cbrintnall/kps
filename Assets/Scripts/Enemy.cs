using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float SATISFIED_DISTANCE = 2.5f;
    public GameObject Money;
    public ParticleSystem OnHit;
    public StatFloat MoveSpeed = 4.0f;
    public StatInt Damage = 3;
    public Animator Animator;
    public Canvas Canvas;
    public bool Flying;
    public EnemyDefinition Definition
    {
        set
        {
            try
            {
                if (value == null)
                    return;

                Damage.Set(Convert.ToInt32(value.Stats["Damage"]));
                MoveSpeed.Set(Convert.ToSingle(value.Stats["MoveSpeed"]));
                GetComponent<Health>().Data.Set(Convert.ToInt32(value.Stats["Health"]));
                if (value.Stats.TryGetValue("Scale", out object additionalScale))
                {
                    transform.localScale =
                        Vector3.one + Vector3.one * Convert.ToInt32(additionalScale);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Error grabbing stats for enemy: {e}");
            }
        }
    }

    public AudioClip OnHurtSound;

    protected Transform target;
    protected AnimationEventsHandler animationEventsHandler;
    protected AudioManager audioManager;
    private CharacterController characterController;
    private Vector3 targetOffset;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animationEventsHandler = Animator.gameObject.GetComponent<AnimationEventsHandler>();
        audioManager = SingletonLoader.Get<AudioManager>();
        GetComponent<Health>().OnDamaged += OnHealthHit;
        targetOffset = new Vector3(
            UnityEngine.Random.insideUnitCircle.x,
            0.0f,
            UnityEngine.Random.insideUnitCircle.y
        );
        target = PlayerEquipmentController.Instance.transform;
        SubscribeToAnimationEvents(animationEventsHandler);
    }

    protected virtual void SubscribeToAnimationEvents(AnimationEventsHandler handler)
    {
        handler?.GetComponent<Animator>().SetBool("Holding", false);
    }

    protected virtual void UpdateAtTarget() { }

    protected virtual void BeforeDeath(HitPayload payload) { }

    private void OnHealthHit(HitPayload payload)
    {
        if (payload.Remaining <= 0)
        {
            var pulse = Instantiate(Resources.Load<GameObject>("FX/BloodPulse"));
            pulse.PositionFrom(this);

            Instantiate(Money, transform.position, Quaternion.identity);

            if (
                Utilities.Randf() <= PlayerEquipmentController.Instance.Stats.EnemyPowerupDropChance
            )
            {
                Instantiate(
                    Resources.Load<GameObject>("Upgrades/Powerup"),
                    transform.position,
                    Quaternion.identity
                );
            }

            BeforeDeath(payload);
            Destroy(gameObject);
        }
        else
        {
            OnHit.Play();
            if (OnHurtSound)
            {
                SingletonLoader
                    .Get<AudioManager>()
                    .Play(
                        new AudioPayload()
                        {
                            Location = transform.position,
                            Clip = OnHurtSound,
                            PitchWobble = 0.1f
                        }
                    );
            }
        }
    }

    protected virtual void UpdateMoving()
    {
        if (target)
        {
            var dir = (target.position + targetOffset - transform.position).normalized;
            transform.forward = new Vector3(dir.x, 0.0f, dir.z);
        }

        if (target != null)
        {
            float distance = Vector3.Distance(
                new Vector3(transform.position.x, 0.0f, transform.position.z),
                new Vector3(target.position.x, 0.0f, target.position.z)
            );

            if (distance > SATISFIED_DISTANCE)
            {
                characterController.Move(transform.forward * MoveSpeed * Time.fixedDeltaTime);
                Animator?.SetBool("Moving", true);
            }
        }
        else
        {
            UpdateAtTarget();
            Animator?.SetBool("Moving", false);
        }

        if (!Flying)
            characterController.Move(Vector3.down * 9.8f);
    }

    void FixedUpdate()
    {
        UpdateMoving();
    }
}
