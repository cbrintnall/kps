using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class SingingEnemy : Enemy
{
    static Area[] areas;

    public StatFloat ShootCooldown = 5.0f;
    public ParticleSystem System;
    public HomingMissile Missile;
    public AudioClip[] Notes;
    public Spline Spline;

    TimeSince attack;
    bool singing;

    void Start()
    {
        if (areas == null)
        {
            areas = FindObjectsOfType<Area>();
        }

        var target = new GameObject($"{name}-target");

        target.transform.position = areas.Random().GetRandomLocation();

        this.target = target.transform;
    }

    void OnDestroy()
    {
        if (target != null)
        {
            Destroy(target.gameObject);
        }
    }

    // this is bad, dont follow this
    protected override void UpdateMoving()
    {
        if (!singing)
        {
            base.UpdateMoving();
        }
        else
        {
            UpdateAtTarget();
        }
    }

    protected override void UpdateAtTarget()
    {
        base.UpdateAtTarget();

        if (!singing)
        {
            singing = true;
        }

        if (target)
        {
            Destroy(target.gameObject);
            target = null;
        }

        if (!Animator.GetBool("Singing"))
        {
            Animator.SetBool("Singing", true);
            System.Play();
        }

        if (Animator.GetBool("Moving"))
        {
            Animator.SetBool("Singing", false);
            System.Stop();
        }

        if (attack > ShootCooldown)
        {
            attack = 0;
            Attacked();
        }
    }

    protected override void Attacked()
    {
        var missile = Instantiate(
            Missile,
            transform.position + Vector3.up * 3.0f,
            Quaternion.identity
        );

        missile.Target = PlayerEquipmentController.Instance.transform;

        this.PlayAtMe(
            new AudioPayload()
            {
                Clip = Notes.Random(),
                Location = transform.position,
                PitchWobble = 0.1f
            }
        );
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(target.transform.position, 0.5f);
    }
}
