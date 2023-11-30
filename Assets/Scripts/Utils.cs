using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate K SelectedEnumerable<T, K>(T input, int idx);

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

    public static Vector3 ToVec3(this Vector2 v) => new Vector3(v.x, 0.0f, v.y);

    public static IEnumerable<T> Tap<T>(this IEnumerable<T> tapped, Action<T> cb)
    {
        foreach (var el in tapped)
        {
            cb(el);
        }
        return tapped;
    }

    public static IEnumerable<T> Enumerate<T>(this IEnumerable<T> enumerator, Action<T, int> cb)
    {
        int count = 0;
        foreach (var el in enumerator)
        {
            cb(el, count);
            count++;
        }

        return enumerator;
    }

    public static IEnumerable<K> SelectEnumerate<T, K>(
        this IEnumerable<T> enumerator,
        SelectedEnumerable<T, K> cb
    )
    {
        int count = 0;
        List<K> enumerated = new();
        foreach (var el in enumerator)
        {
            enumerated.Add(cb(el, count));
            count++;
        }

        return enumerated;
    }

    public static void Defer(this MonoBehaviour m, Action cb)
    {
        m.StartCoroutine(_Defer(cb));
    }

    private static IEnumerator _Defer(Action cb)
    {
        yield return new WaitForEndOfFrame();
        cb();
    }
}
