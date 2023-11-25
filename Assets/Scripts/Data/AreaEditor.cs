#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Area))]
public class AreaEditor : Editor
{
    private void OnSceneGUI()
    {
        var area = target as Area;
        Transform basis = area.transform;

        if (area.points.Length == 0)
            return;

        EditorGUI.BeginChangeCheck();
        var fval = Handles.DoPositionHandle(
            basis.TransformPoint(area.points[0]),
            Quaternion.identity
        );

        if (EditorGUI.EndChangeCheck())
            area.points[0] = basis.InverseTransformPoint(fval);

        if (area.points.Length == 1)
            return;

        for (int i = 0; i < area.triangles.Count; i++)
        {
            var p0 = basis.TransformPoint(area.triangles[i].Pt1.ToVec3());
            var p1 = basis.TransformPoint(area.triangles[i].Pt2.ToVec3());
            var p2 = basis.TransformPoint(area.triangles[i].Pt3.ToVec3());
            Handles.DrawLine(p0, p1, 2.0f);
            Handles.DrawLine(p1, p2, 2.0f);
            Handles.DrawLine(p2, p0, 2.0f);
        }

        for (int i = 1; i < area.points.Length; i++)
        {
            var p0 = basis.TransformPoint(area.points[i - 1]);
            var p1 = basis.TransformPoint(area.points[i]);
            Handles.DrawLine(p0, p1);

            EditorGUI.BeginChangeCheck();
            var val = Handles.DoPositionHandle(p1, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
                area.points[i] = basis.InverseTransformPoint(val);
        }

        var end = basis.TransformPoint(area.points[area.points.Length - 1]);
        var start = basis.TransformPoint(area.points[0]);
        Handles.DrawLine(end, start);
    }
}
#endif
