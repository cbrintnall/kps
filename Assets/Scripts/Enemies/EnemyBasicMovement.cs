using UnityEngine;

public class EnemyBasicMovement : EnemyMover
{
    public const float SATISFIED_DISTANCE = 2.5f;

    [SerializeField]
    bool flying;

    Enemy enemy;
    Animator animator;
    CharacterController characterController;
    Vector3 targetOffset;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        enemy = GetComponent<Enemy>();
        targetOffset = new Vector3(Random.insideUnitCircle.x, 0.0f, Random.insideUnitCircle.y);
    }

    public override void Move()
    {
        if (enemy.Target != null)
        {
            var dir = (enemy.Target.position + targetOffset - transform.position).normalized;
            transform.forward = new Vector3(dir.x, 0.0f, dir.z);

            float distance = Vector3.Distance(
                new Vector3(transform.position.x, 0.0f, transform.position.z),
                new Vector3(enemy.Target.position.x, 0.0f, enemy.Target.position.z)
            );

            if (distance > SATISFIED_DISTANCE)
            {
                characterController.Move(transform.forward * enemy.MoveSpeed * Time.fixedDeltaTime);
                animator?.SetBool("Moving", true);
                AtTarget = false;
            }
            else
            {
                AtTarget = true;
                animator?.SetBool("Moving", false);
            }
        }

        if (!flying)
            characterController.Move(Vector3.down * 9.8f);
    }
}
