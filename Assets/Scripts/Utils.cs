using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class Utilities
{
    public static void PositionFrom(this GameObject go, GameObject target)
    {
        go.transform.position = target.transform.position;
    }

    public static void PositionFrom(this GameObject go, Component target)
    {
        go.transform.position = target.transform.position;
    }

    public static void PositionFrom(this Component go, Component target)
    {
        go.transform.position = target.transform.position;
    }

    public static void PositionFrom(this Component go, GameObject target)
    {
        go.transform.position = target.transform.position;
    }

    public static T Random<T>(this IEnumerable<T> list)
    {
        var l = list.ToList();
        return l[UnityEngine.Random.Range(0, l.Count)];
    }

    public static float Randf() => UnityEngine.Random.Range(0.0f, 1.0f);

    public static void WaitThen(this MonoBehaviour t, float time, Action then)
    {
        t.StartCoroutine(WaitAndThen(time, then));
    }

    private static IEnumerator WaitAndThen(float time, Action then)
    {
        yield return new WaitForSeconds(time);
        then();
    }
}
