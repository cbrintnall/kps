using System;
using DG.Tweening;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public GameObject Money;
    public ParticleSystem OnHit;
    public StatFloat MoveSpeed = 4.0f;
    public StatInt Damage = 3;
    public Animator Animator;
    public Canvas Canvas;
    public bool Flying;
    public Transform Target => target;
    public EnemyDefinition Definition
    {
        get => definition_;
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
                definition_ = value;
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
    private Tweener scalePunch;
    private EnemyMover mover;
    EnemyDefinition definition_;

    void Awake()
    {
        mover = GetComponent<EnemyMover>();
        animationEventsHandler = Animator.gameObject.GetComponent<AnimationEventsHandler>();
        audioManager = SingletonLoader.Get<AudioManager>();
        GetComponent<Health>().OnDamaged += OnHealthHit;

        target = PlayerEquipmentController.Instance.transform;
        SubscribeToAnimationEvents(animationEventsHandler);
    }

    protected abstract void Attacked();

    protected virtual void SubscribeToAnimationEvents(AnimationEventsHandler handler)
    {
        handler?.GetComponent<Animator>().SetBool("Holding", false);
    }

    protected virtual void UpdateAtTarget() { }

    protected virtual void BeforeDeath(HitPayload payload)
    {
        if (Utilities.Randf() > PlayerEquipmentController.Instance.Stats.ExplodeOnEnemyDeathChance)
            return;

        var explosion = Instantiate(
            Resources.Load<Explosion>("Upgrades/Explosion"),
            transform.position,
            Quaternion.identity
        );
        explosion.Owner = gameObject;
        explosion.Damage = PlayerEquipmentController.Instance.Stats.ExplosionDamage;
        explosion.Layers = LayerMask.GetMask("Enemy");
        explosion.Size = PlayerEquipmentController.Instance.Stats.ExplosionSize;
    }

    private void OnHealthHit(HitPayload payload)
    {
        if (payload.Remaining <= 0)
        {
            var pulse = Instantiate(Resources.Load<GameObject>("FX/BloodPulse"));
            pulse.PositionFrom(this);

            var moneys = Instantiate(Money, transform.position, Quaternion.identity);

            moneys.GetComponent<MoneyPickup>().Value = Definition.Value;

            if (
                Utilities.Randf() <= PlayerEquipmentController.Instance.Stats.EnemyPowerupDropChance
            )
            {
                Instantiate(
                    Resources.Load<GameObject>("Upgrades/Powerup"),
                    transform.position + Vector3.up * 2.5f,
                    Quaternion.identity
                );
            }

            BeforeDeath(payload);
            Destroy(gameObject);
        }
        else
        {
            OnHit.Play();
            if (scalePunch == null || !scalePunch.active)
                scalePunch = transform.DOPunchScale(Vector3.one * 1.05f, 0.2f);
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
        if (mover.AtTarget)
        {
            UpdateAtTarget();
        }
        else
        {
            mover.Move();
        }
    }

    void FixedUpdate()
    {
        UpdateMoving();
    }
}
