using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Calls a callback after a certain amount of time.
/// </summary>
public class TimedStatus : MonoBehaviour
{
    public float Time = 0.0f;
    public Action Callback;
}
