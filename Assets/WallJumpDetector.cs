using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct WallData
{
    public bool Hitting;
    public Vector3 Normal;
}

public class WallJumpDetector : MonoBehaviour
{
    public WallData Data;
    public Transform OwningTransform;

    [SerializeField]
    float checkDistance = 1.0f;
    Vector3[] checkDirs =>
        new Vector3[] { -OwningTransform.forward, OwningTransform.right, -OwningTransform.right, };

    void Awake()
    {
        Data = new();
    }

    void FixedUpdate()
    {
        Data.Hitting = false;

        foreach (var dir in checkDirs)
        {
            if (
                Physics.Raycast(
                    transform.position,
                    dir,
                    out var hit,
                    checkDistance,
                    LayerMask.GetMask("Default"),
                    QueryTriggerInteraction.Ignore
                )
            )
            {
                OnRaycastHit(hit);
                break;
            }
        }
    }

    void OnRaycastHit(RaycastHit hit)
    {
        Data.Hitting = true;
        Data.Normal = hit.normal;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Data.Hitting ? Color.green : Color.red;
        foreach (var dir in checkDirs)
        {
            Gizmos.DrawRay(new Ray(transform.position, dir));
        }
    }
}
