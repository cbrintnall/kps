using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class FovController : MonoBehaviour
{
    public float AdditionalFov = 0.0f;

    private float FOV;
    private CinemachineVirtualCamera vCam;

    void Awake()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        FOV = vCam.m_Lens.FieldOfView;
    }

    void Update()
    {
        vCam.m_Lens.FieldOfView = FOV + AdditionalFov;
    }
}
