using System;
using System.Collections.Generic;
using System.Linq;
using DelaunatorSharp;
using KaimiraGames;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

public static class TriangleExtensions
{
    public static float Area(this ITriangle triangle)
    {
        var arr = triangle.Points
            .Select(pt => new Vector2(Convert.ToSingle(pt.X), Convert.ToSingle(pt.Y)))
            .ToArray();

        return ((arr[0] - arr[2]) * (arr[1] - arr[2])).magnitude / 2;
    }

    public static Vector2 RandomPoint(this ITriangle triangle)
    {
        var arr = triangle.Points
            .Select(pt => new Vector2(Convert.ToSingle(pt.X), Convert.ToSingle(pt.Y)))
            .ToArray();

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
public class Area : MonoBehaviour
{
    [SerializeField]
    public Vector3[] points;
    Delaunator triangulator;
    float totalArea;
    private List<Vector3> testpts;

    [Button]
    void Test()
    {
        for (int i = 0; i < 500; i++)
            testpts.Add(GetRandomLocation());
    }

    [Button]
    void ResetY()
    {
        if (points.Length == 0)
            return;

        float y = points[0].y;
        for (int i = 0; i < points.Length; i++)
        {
            points[i].y = y;
        }
    }

    void Awake()
    {
        Recalculate();
        Test();
    }

    public Vector3 GetRandomLocation()
    {
        var pt = PickRandomTriangle().RandomPoint();
        return new Vector3(pt.x, 0.0f, pt.y);
    }

    private void Recalculate()
    {
        IPoint[] pts = new IPoint[points.Length + 1];
        for (int i = 0; i < points.Length; i++)
            pts[i] = new Point(points[i].x, points[i].y);
        pts[points.Length] = pts[0];
        triangulator = new Delaunator(pts);
        totalArea = triangulator.GetTriangles().Select(triangle => triangle.Area()).Sum();
    }

    private ITriangle PickRandomTriangle()
    {
        var rng = UnityEngine.Random.Range(0f, totalArea);
        var triangles = triangulator.GetTriangles().ToArray();
        for (int i = 0; i < triangles.Length; ++i)
        {
            if (rng < triangles[i].Area())
            {
                return triangles[i];
            }
            rng -= triangles[i].Area();
        }

        return triangles.Last();
    }

    void OnDrawGizmos()
    {
        if (testpts == null || testpts.Count == 0)
            return;

        Gizmos.color = Color.green;
        foreach (var pt in testpts)
        {
            Gizmos.DrawSphere(pt, 0.5f);
        }
    }
}
