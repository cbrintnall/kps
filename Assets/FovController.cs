using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class FovController : MonoBehaviour
{
    public float AdditionalFov = 0.0f;
    private CinemachineVirtualCamera vCam;
    SettingsManager settingsManager;

    void Awake()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        settingsManager = SingletonLoader.Get<SettingsManager>();
    }

    void Update()
    {
        vCam.m_Lens.FieldOfView = settingsManager.FOV + AdditionalFov;
    }
}
