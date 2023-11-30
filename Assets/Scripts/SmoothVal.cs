using Sirenix.OdinInspector;
using UnityEngine;

public class SmoothValue
{
    public float Value;
    public float Smoothed;
    public float Speed = 0.75f;

    public static implicit operator float(SmoothValue val)
    {
        val.Smoothed = Mathf.Lerp(val.Smoothed, val.Value, val.Speed * Time.deltaTime);
        return val.Smoothed;
    }
}
