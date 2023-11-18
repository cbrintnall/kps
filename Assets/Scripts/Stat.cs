using System;
using UnityEngine;

public enum StatOperation
{
    Percent,
    Value
}

public class Stat<T>
    where T : unmanaged
{
    public event Action<T, T> ValueChanged;
    public readonly T Base;
    public T Max;
    public T Min;
    public T Current;

    public Stat(T _base, T max, T min)
    {
        this.Base = _base;
        this.Current = _base;
        this.Max = max;
        this.Min = min;
    }

    protected void NotifyChange(T current, T delta)
    {
        ValueChanged?.Invoke(current, delta);
    }
}

[Serializable]
public class StatInt : Stat<int>
{
    public static implicit operator StatInt(int f) => new StatInt(f, int.MaxValue, 0);

    public static implicit operator int(StatInt f) => f.Current;

    public float Normalized => (float)Current / (float)Max;

    public StatInt(int _base, int max, int min)
        : base(_base, max, min) { }

    public void Incr(int amt, StatOperation operation = StatOperation.Percent, bool fromBase = true)
    {
        int prior = Current;
        switch (operation)
        {
            case StatOperation.Percent:
                IncrPercent(amt, fromBase);
                break;
            case StatOperation.Value:
                Current += amt;
                break;
        }

        Current = Mathf.Clamp(Current, Min, Max);

        if (Current != prior)
            NotifyChange(Current, Current - prior);
    }

    public void Set(int value)
    {
        int old = Current;
        Current = Mathf.Clamp(value, Min, Max);
        NotifyChange(Current, old - Current);
    }

    public void IncrPercent(float amt, bool fromBase = true)
    {
        int prior = Current;
        Current += Mathf.RoundToInt(amt * (fromBase ? Base : Current));
        Current = Mathf.Clamp(Current, Min, Max);
        if (Current != prior)
            NotifyChange(Current, Current - prior);
    }
}

[Serializable]
public class StatFloat : Stat<float>
{
    public static implicit operator StatFloat(float f) =>
        new StatFloat(f, float.PositiveInfinity, 0);

    public static implicit operator float(StatFloat f) => f.Current;

    public float Normalized => (float)Current / (float)Max;

    public StatFloat(float _base, float max, float min)
        : base(_base, max, min) { }

    public void Set(float value)
    {
        float old = Current;
        Current = Mathf.Clamp(value, Min, Max);
        NotifyChange(Current, old - Current);
    }

    public void Incr(
        float amt,
        StatOperation operation = StatOperation.Percent,
        bool fromBase = true
    )
    {
        float prior = Current;
        switch (operation)
        {
            case StatOperation.Percent:
                Current += amt * (fromBase ? Base : Current);
                break;
            case StatOperation.Value:
                Current += amt;
                break;
        }

        Current = Mathf.Clamp(Current, Min, Max);

        if (Current != prior)
            NotifyChange(Current, Current - prior);
    }
}
