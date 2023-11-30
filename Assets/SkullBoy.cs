using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullBoy : MonoBehaviour
{
    public static SkullBoy Instance;

    public Transform Jaw;
    public Transform Head;

    bool speaking = true;

    void Awake()
    {
        Instance = this;
    }

    public void Say() { }

    void Update()
    {
        if (speaking)
        {
            var rot = Jaw.transform.rotation;
            rot.eulerAngles = Random.insideUnitSphere;
            var hrot = Head.transform.rotation;
            hrot.eulerAngles = Random.insideUnitSphere;
        }
    }
}
