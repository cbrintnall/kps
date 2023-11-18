using Sirenix.OdinInspector;
using UnityEngine;

public class PickupFX : MonoBehaviour
{
    public Color FXColor = Color.green;

    public ParticleSystem Circlets;
    public ParticleSystem Sparkles;
    public Material Cylinder;

    void Awake() { }

    [Button]
    void SyncColor()
    {
        var main = Circlets.main;

        main.startColor = FXColor;
        Cylinder.SetColor("_Color", FXColor);
    }
}
