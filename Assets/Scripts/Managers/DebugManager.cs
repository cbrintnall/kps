using System.Reflection;
using IngameDebugConsole;
using UnityEngine;

public class ConVar : System.Attribute { }

[Singleton]
public class DebugManager : MonoBehaviour
{
    public static bool DebugEnabled;

    void Awake()
    {
        DebugLogConsole.AddCustomParameterType(typeof(object), ParseObject);
        DebugLogConsole.AddCommand<string, object>("set", "Set a static variable", SetVar);
    }

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
}
