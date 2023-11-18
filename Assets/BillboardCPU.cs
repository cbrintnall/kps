using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class BillboardCPU : MonoBehaviour
{
    public bool OnlyY;

    private Camera cam => Camera.main;

    // Update is called once per frame
    void Update()
    {
        var before = transform.rotation.eulerAngles;
        transform.LookAt(cam.transform);
        if (OnlyY)
            transform.rotation = Quaternion.Euler(
                before.x,
                transform.rotation.eulerAngles.y,
                transform.rotation.eulerAngles.z
            );

        transform.transform.forward = -transform.forward;
    }
}
