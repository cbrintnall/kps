using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class Triangulator
{
    private List<Vector2> m_points = new List<Vector2>();

    public Triangulator(Vector2[] points)
    {
        m_points = new List<Vector2>(points);
    }

    public int[] Triangulate()
    {
        List<int> indices = new List<int>();

        int n = m_points.Count;
        if (n < 3)
            return indices.ToArray();

        int[] V = new int[n];
        if (Area() > 0)
        {
            for (int v = 0; v < n; v++)
                V[v] = v;
        }
        else
        {
            for (int v = 0; v < n; v++)
                V[v] = (n - 1) - v;
        }

        int nv = n;
        int count = 2 * nv;
        for (int m = 0, v = nv - 1; nv > 2; )
        {
            if ((count--) <= 0)
                return indices.ToArray();

            int u = v;
            if (nv <= u)
                u = 0;
            v = u + 1;
            if (nv <= v)
                v = 0;
            int w = v + 1;
            if (nv <= w)
                w = 0;

            if (Snip(u, v, w, nv, V))
            {
                int a,
                    b,
                    c,
                    s,
                    t;
                a = V[u];
                b = V[v];
                c = V[w];
                indices.Add(a);
                indices.Add(b);
                indices.Add(c);
                m++;
                for (s = v, t = v + 1; t < nv; s++, t++)
                    V[s] = V[t];
                nv--;
                count = 2 * nv;
            }
        }

        indices.Reverse();
        return indices.ToArray();
    }

    public float Area()
    {
        int n = m_points.Count;
        float A = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++)
        {
            Vector2 pval = m_points[p];
            Vector2 qval = m_points[q];
            A += pval.x * qval.y - qval.x * pval.y;
        }
        return (A * 0.5f);
    }

    private bool Snip(int u, int v, int w, int n, int[] V)
    {
        int p;
        Vector2 A = m_points[V[u]];
        Vector2 B = m_points[V[v]];
        Vector2 C = m_points[V[w]];
        if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
            return false;
        for (p = 0; p < n; p++)
        {
            if ((p == u) || (p == v) || (p == w))
                continue;
            Vector2 P = m_points[V[p]];
            if (InsideTriangle(A, B, C, P))
                return false;
        }
        return true;
    }

    private bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
    {
        float ax,
            ay,
            bx,
            by,
            cx,
            cy,
            apx,
            apy,
            bpx,
            bpy,
            cpx,
            cpy;
        float cCROSSap,
            bCROSScp,
            aCROSSbp;

        ax = C.x - B.x;
        ay = C.y - B.y;
        bx = A.x - C.x;
        by = A.y - C.y;
        cx = B.x - A.x;
        cy = B.y - A.y;
        apx = P.x - A.x;
        apy = P.y - A.y;
        bpx = P.x - B.x;
        bpy = P.y - B.y;
        cpx = P.x - C.x;
        cpy = P.y - C.y;

        aCROSSbp = ax * bpy - ay * bpx;
        cCROSSap = cx * apy - cy * apx;
        bCROSScp = bx * cpy - by * cpx;

        return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
    }
}

public static class TriangleExtensions { }

public class Triangle
{
    public float Area => ((arr[0] - arr[2]) * (arr[1] - arr[2])).magnitude / 2.0f;
    public Vector2 Pt1 => arr[0];
    public Vector2 Pt2 => arr[1];
    public Vector2 Pt3 => arr[2];

    private Vector2[] arr;

    public Triangle(Vector2[] points)
    {
        arr = points;
    }

    public Vector2 RandomPoint()
    {
        var r1 = Mathf.Sqrt(UnityEngine.Random.Range(0f, 1f));
        var r2 = UnityEngine.Random.Range(0f, 1f);
        var m1 = 1 - r1;
        var m2 = r1 * (1 - r2);
        var m3 = r2 * r1;

        var p1 = new Vector2(arr[0].x, arr[0].y);
        var p2 = new Vector2(arr[1].x, arr[1].y);
        var p3 = new Vector2(arr[2].x, arr[2].y);
        return (m1 * p1) + (m2 * p2) + (m3 * p3);
    }
}

// adapted from: https://dev.to/bogdanalexandru/generating-random-points-within-a-polygon-in-unity-nce
public class Area : MonoBehaviour, ISpawn
{
    [SerializeField]
    public Vector3[] points;
    public List<Triangle> triangles = new();
    Triangulator triangulator;
    float totalArea;
    private List<Vector3> testpts = new();
    private Vector2[] v2points;
    private int[] indices;

    public bool CanSpawn => true;

    void Awake()
    {
        v2points = points.Select(pt => new Vector2(pt.x, pt.z)).ToArray();
        Recalculate();

        // for (int i = 0; i < 300; i++)
        // {
        //     testpts.Add(GetRandomLocation());
        // }
    }

    public void DoSpawn(Transform target)
    {
        target.position = GetRandomLocation();
    }

    public Vector3 GetRandomLocation()
    {
        var pt = PickRandomTriangle().RandomPoint();
        // return transform.TransformPoint(new Vector3(pt.x, 0.0f, pt.y));
        return transform.TransformPoint(new Vector3(pt.x, points.Average(pt => pt.y), pt.y));
    }

    private void Recalculate()
    {
        triangulator = new Triangulator(v2points);
        indices = triangulator.Triangulate();

        for (int i = 0; i < indices.Length; i += 3)
        {
            triangles.Add(
                new Triangle(
                    new Vector2[]
                    {
                        v2points[indices[i]],
                        v2points[indices[i + 1]],
                        v2points[indices[i + 2]]
                    }
                )
            );
        }

        totalArea = triangles.Sum(triangle => triangle.Area);

        Debug.Log($"idx count = {indices.Length}, triangle count = {triangles.Count}");
    }

    private Triangle PickRandomTriangle()
    {
        var rng = UnityEngine.Random.Range(0f, totalArea);
        for (int i = 0; i < triangles.Count; ++i)
        {
            if (rng < triangles[i].Area)
            {
                return triangles[i];
            }
            rng -= triangles[i].Area;
        }

        return triangles.Last();
    }

    [Button]
    void SnapPointsToGround()
    {
        for (int i = 0; i < points.Length; i++)
        {
            var pt = points[i];
            if (
                Physics.Raycast(
                    transform.TransformPoint(pt),
                    Vector3.down,
                    out RaycastHit hit,
                    float.PositiveInfinity,
                    LayerMask.GetMask("Default")
                )
            )
            {
                pt = hit.point;
            }

            points[i] = transform.InverseTransformPoint(pt);
        }
    }

    void OnDrawGizmos()
    {
        if (testpts == null || testpts.Count == 0)
            return;

        Gizmos.color = Color.green;
        foreach (var pt in testpts)
        {
            Gizmos.DrawSphere(transform.TransformPoint(pt), 0.25f);
        }
    }
}
