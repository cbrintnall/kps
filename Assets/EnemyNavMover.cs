using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class EnemyNavMover : EnemyMover
{
    NavMeshAgent nav;
    Animator animator;
    Enemy enemy;
    bool ready => nav.enabled;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        enemy = GetComponent<Enemy>();
        nav = GetComponent<NavMeshAgent>();
        nav.enabled = false;
    }

    void Start()
    {
        nav.speed = enemy.Definition.Stats.MoveSpeed;
        nav.avoidancePriority = Random.Range(1, 99);
        if (!nav.isOnNavMesh)
        {
            if (
                NavMesh.SamplePosition(
                    transform.position,
                    out NavMeshHit hit,
                    10.0f,
                    NavMesh.AllAreas
                )
            )
            {
                nav.Warp(hit.position);
                nav.enabled = true;
            }
            else
            {
                Debug.LogWarning(
                    $"{nameof(EnemyNavMover)} not on mesh, but can't sample point to move to."
                );
            }
        }
        else
        {
            nav.enabled = true;
        }
    }

    public bool WithinRange()
    {
        return nav.remainingDistance <= nav.stoppingDistance;
    }

    public override void Move()
    {
        if (enemy.Target == null || !nav.isOnNavMesh || !ready)
            return;

        animator.SetBool("Moving", !AtTarget);

        if (!AtTarget && nav.pathStatus == NavMeshPathStatus.PathComplete)
        {
            transform.forward = (nav.steeringTarget - transform.position).normalized;
        }
    }

    void FixedUpdate()
    {
        AtTarget = nav.isOnNavMesh && AtDestination();
        nav.SetDestination(enemy.Target.transform.position);
    }

    bool AtDestination()
    {
        return (!nav.pathPending && WithinRange()) || !nav.hasPath;
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
