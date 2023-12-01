using System;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    public Vector2 HeightOffsetRange = new Vector2(20.0f, 40.0f);
    public float AttackCooldown = 3.0f;
    public Bomb BombPrefab;

    TimeSince attacked;

    void Start()
    {
        target = PlayerEquipmentController.Instance.transform;
        Animator.SetBool("Flying", true);
        transform.position +=
            Vector3.up * UnityEngine.Random.Range(HeightOffsetRange.x, HeightOffsetRange.y);
    }

    protected override void UpdateAtTarget()
    {
        if (attacked > AttackCooldown)
        {
            attacked = 0.0f;
            Attacked();
        }
    }

    protected override void Attacked()
    {
        var bomb = Instantiate(BombPrefab, transform.position, Quaternion.identity);
        bomb.Damage = Damage;
        bomb.Targets = LayerMask.GetMask("Player");
    }
}
