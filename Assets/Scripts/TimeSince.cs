using UnityEngine;

public struct TimeSince
{
    float time;

    public override string ToString()
    {
        return $"{Time.time - time}";
    }

    public static implicit operator float(TimeSince ts)
    {
        return Time.time - ts.time;
    }

    public static implicit operator TimeSince(float ts)
    {
        return new TimeSince { time = Time.time - ts };
    }
}
