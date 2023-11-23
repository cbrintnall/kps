using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetPointType
{
    DOWN
}

public class TargetPoint : MonoBehaviour
{
    void FixedUpdate()
    {
        if (
            Physics.Raycast(
                transform.position,
                Vector3.down,
                out var hit,
                1000.0f,
                LayerMask.GetMask("Default")
            )
        )
        {
            transform.position = hit.point + Vector3.up;
        }
    }
}
