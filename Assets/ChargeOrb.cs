using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeOrb : MonoBehaviour
{
    public float MaxChargeValue = 0.0f;
    public float Charge
    {
        get => charge_;
        set { charge_ = value; }
    }
    public ParticleSystem FX;

    float charge_ = 0.1f;
    MeshRenderer mesh;

    void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        bool chargin = Charge > 0.0f;
        mesh.enabled = chargin;
        if (!FX.isPlaying && chargin)
        {
            FX.Play();
        }

        if (FX.isPlaying && !chargin)
        {
            FX.Stop();
        }

        transform.localScale = Vector3.one * Mathf.Clamp(Mathf.Max(Charge, 0.001f), 0.0f, 0.5f);
        var emission = FX.emission;
        emission.rateOverTime = Mathf.Clamp(Mathf.RoundToInt(Mathf.Max(Charge, 1) * 15), 5, 50);
        var main = FX.main;
        main.startSpeed = -Mathf.Clamp(
            Mathf.Lerp(5.0f, 30.0f, Charge / MaxChargeValue),
            0.0f,
            50.0f
        );
        var shape = FX.shape;
        shape.radius = Mathf.Max(Mathf.Lerp(1.0f, 3.0f, Charge / MaxChargeValue), 0.0f);
    }
}
