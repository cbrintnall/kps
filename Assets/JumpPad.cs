using Sirenix.OdinInspector;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;

public class JumpPad : MonoBehaviour
{
    [SerializeField]
    float force;

    [SerializeField]
    bool canMoveAI = true;

    [SerializeField, Range(1f, 500f)]
    float splineSnapSearchRadius = 25f;

    SplineContainer spline;

    void Awake()
    {
        spline = GetComponent<SplineContainer>();

        if (canMoveAI)
        {
            Debug.Assert(spline != null, "Can't create nav link without spline");
            Debug.Assert(spline.Spline.Count >= 2);

            var link = gameObject.AddComponent<NavMeshLink>();

            link.startPoint = spline.Spline[0].Position;
            link.endPoint = spline.Spline[1].Position;
        }
    }

    [Button, DisableIf("@!canMoveAI")]
    void SnapSplinesToGround()
    {
        var spline = GetComponent<SplineContainer>();
        Undo.RecordObject(spline, "Snap points to navmesh");
        for (int i = 0; i < spline.Spline.Count; i++)
        {
            var pt = spline.Spline[i];

            if (
                NavMesh.SamplePosition(
                    spline.transform.TransformPoint(pt.Position),
                    out var hit,
                    splineSnapSearchRadius,
                    NavMesh.AllAreas
                )
            )
            {
                pt.Position = spline.transform.InverseTransformPoint(hit.position);
                spline.Spline[i] = pt;
            }
        }

        // var link = GetComponent<NavMeshLink>();
        // if (link == null)
        // {
        //     link = gameObject.AddComponent<NavMeshLink>();
        // }

        // link.startPoint = spline.Spline[0].Position;
        // link.endPoint = spline.Spline[1].Position;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out PlayerMovement player))
        {
            player.DoJump(force);
        }

        if (collider.TryGetComponent(out Enemy enemy))
        {
            print($"{enemy} entered!");
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * force);
    }
}
