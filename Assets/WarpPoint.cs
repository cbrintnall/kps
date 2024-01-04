using System;
using UnityEngine;

public class WarpPoint : MonoBehaviour
{
    public event Action PlayerEntered;

    [SerializeField]
    [Tooltip("Is this a start or end")]
    public bool isStart = true;

    [Tooltip("Where the player is teleported to")]
    public Transform TargetPoint;

    [SerializeField]
    GameObject fx;

    public void SetTargetPoint(Transform target)
    {
        fx.SetActive(target != null);
        TargetPoint = target;
    }

    void Awake()
    {
        SetTargetPoint(TargetPoint);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (TargetPoint != null && collider.TryGetComponent(out Player player))
        {
            player.Movement.Warp(TargetPoint.transform.position + Vector3.up);
            PlayerEntered?.Invoke();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.5f);
        if (TargetPoint)
        {
            Gizmos.DrawLine(transform.position, TargetPoint.position);
            Gizmos.DrawSphere(TargetPoint.position, 0.75f);
        }
    }
}
