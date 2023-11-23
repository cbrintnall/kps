using UnityEngine;

public class FlyingEnemy : GroundEnemy
{
    public float HeightOffset = 200.0f;
    public Bomb BombPrefab;

    void Start()
    {
        Animator.SetBool("Flying", true);
        transform.position += Vector3.up * HeightOffset;
    }

    protected override void UpdateAtTarget()
    {
        if (attack > 3.5f)
        {
            attack = 0.0f;
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
