using System;
using System.Collections.Generic;
using System.Reflection;
using IngameDebugConsole;
using UnityEngine;

public class ConVar : System.Attribute { }

public class DrawVar
{
    public string Name;
    public Func<object> Callback;
}

[Singleton]
public class DebugManager : MonoBehaviour
{
    public const float DRAW_VAR_HEIGHT = 24.0f;
    public static bool DebugEnabled;

    Dictionary<Action, TimeSince> timedDraw = new();
    List<DrawVar> debugDraw = new();

    private static bool ParseObject(string input, out object output)
    {
        output = null;

        if (bool.TryParse(input, out bool resb))
        {
            output = resb;
            return true;
        }

        if (float.TryParse(input, out float resf))
        {
            output = resf;
            return true;
        }

        if (int.TryParse(input, out int resi))
        {
            output = resi;
            return true;
        }

        output = input;
        return true;
    }

    public static void SetVar(string var, object value)
    {
        string cls = var.Split(".")[0];
        string variable = var.Split(".")[1];

        var info = Assembly
            .GetExecutingAssembly()
            .GetType(cls)
            .GetField(variable, BindingFlags.Static | BindingFlags.NonPublic);

        if (info != null)
        {
            Debug.Log($"Setting var {var} to {value}");

            if (info.GetValue(null).GetType() == typeof(StatFloat))
            {
                var stat = info.GetValue(null) as StatFloat;
                stat.Set((float)value);
                info.SetValue(null, stat);
            }
            else
            {
                info.SetValue(null, value);
            }
        }
    }

    public void AddDrawVar(DrawVar v)
    {
        debugDraw.Add(v);
    }

    void OnGUI()
    {
        GUI.BeginGroup(new Rect(0, 0, 300, 1000));
        int cnt = 0;
        List<DrawVar> toRemove = new();
        foreach (var var in debugDraw)
        {
            try
            {
                GUI.Label(
                    new Rect(0, DRAW_VAR_HEIGHT * cnt, 300, DRAW_VAR_HEIGHT),
                    $"{var.Name}={var.Callback()}"
                );
                cnt++;
            }
            catch (Exception e)
            {
                toRemove.Add(var);
            }
        }

        foreach (var var in toRemove)
        {
            debugDraw.Remove(var);
        }
        GUI.EndGroup();
    }

    void Awake()
    {
        DebugLogConsole.AddCustomParameterType(typeof(object), ParseObject);
        DebugLogConsole.AddCommand<string, object>("set", "Set a static variable", SetVar);
    }

    // public void DrawForTime(Action cb, float time) {
    //     timedDraw[cb] = time
    //  }

    void OnDrawGizmos() { }
}
