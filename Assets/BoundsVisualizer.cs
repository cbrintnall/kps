using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteInEditMode]
public class BoundsVisualizer : MonoBehaviour
{
    [SerializeField]
    GameObject target;

    Bounds? bounds;

    [Button]
    void Clear()
    {
        bounds = null;
    }

    void Update()
    {
        if (bounds == null)
        {
            GameObject targ = target != null ? target : gameObject;

            bounds = targ.CalculateBounds();
        }
    }

    void OnDrawGizmos()
    {
        if (bounds == null)
            return;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(bounds.Value.center, bounds.Value.size);
    }
}
