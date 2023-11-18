using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class SingingEnemy : Enemy
{
    static List<Transform> points = new();

    public StatFloat ShootCooldown = 5.0f;
    public ParticleSystem System;
    public HomingMissile Missile;
    public AudioClip[] Notes;
    public Spline Spline;

    TimeSince attack;

    void Start()
    {
        var target = new GameObject($"{name}-target");

        target.transform.position =
            EnemyMaster.RangedPoints.Random().position
            + new Vector3(Random.Range(1.0f, 5.0f), 0.0f, Random.Range(1.0f, 5.0f));

        this.target = target.transform;
    }

    protected override void UpdateAtTarget()
    {
        base.UpdateAtTarget();

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
    }
}
