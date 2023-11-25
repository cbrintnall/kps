using System.Linq;
using UnityEngine;

public static class Curves
{
    public static int GetRequiredXP(int level)
    {
        return Mathf.RoundToInt(
            Enumerable
                .Range(1, level + 1)
                .Sum(
                    i =>
                        (Mathf.Log10((i + 1) * 10.0f) * Mathf.Pow(i + 1, 2.0f))
                        + SingletonLoader.Get<FlowManager>().GameData.Leveling.Offset
                )
        );
    }
}
