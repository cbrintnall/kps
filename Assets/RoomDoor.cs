using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDoor : MonoBehaviour
{
    public Vector2 Size = new Vector2(4.0f, 8.0f);

    void OnDrawGizmos()
    {
        var prev = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(Size.x / 2.0f, Size.y / 2.0f, 0.25f));
        Gizmos.matrix = prev;
        Gizmos.DrawRay(transform.position, transform.forward);
        // Gizmos.matrix = prev;
    }
}
