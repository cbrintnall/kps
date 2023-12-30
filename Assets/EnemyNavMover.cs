using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyNavMover : EnemyMover
{
    NavMeshAgent nav;
    CharacterController characterController;
    Animator animator;
    Enemy enemy;

    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        enemy = GetComponent<Enemy>();
    }

    public override void Move()
    {
        if (enemy.Target == null || !nav.isOnNavMesh)
            return;

        nav.SetDestination(enemy.Target.transform.position);
        animator.SetBool("Moving", !AtTarget);

        if (!AtTarget && nav.pathStatus == NavMeshPathStatus.PathComplete)
        {
            transform.forward = (nav.steeringTarget - transform.position).normalized;
            characterController.Move(transform.forward * enemy.MoveSpeed * Time.deltaTime);
            nav.velocity = characterController.velocity;
        }
    }

    void FixedUpdate()
    {
        AtTarget = nav.isOnNavMesh && AtDestination();
    }

    bool AtDestination()
    {
        return !nav.pathPending && nav.remainingDistance <= nav.stoppingDistance && !nav.hasPath;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(nav.nextPosition, 0.5f);
        Gizmos.color = Color.white;

        foreach (var pt in nav.path.corners)
        {
            Gizmos.DrawSphere(pt, 1.0f);
        }
    }
}
