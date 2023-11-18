using System;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public AudioClip PickupSound;

    Type Upgrade;

    void Start()
    {
        Upgrade = SingletonLoader.Get<UpgradesManager>().RequestUpgradeType().Class;
    }

    // void OnTriggerEnter(Collider collider)
    // {
    //     if (collider.TryGetComponent(out PlayerEquipmentController controller))
    //     {
    //         controller.Equipment.AddUpgrade(Upgrade);

    //         SingletonLoader
    //             .Get<AudioManager>()
    //             .Play(new AudioPayload() { Clip = PickupSound, Location = transform.position });

    //         Debug.Log($"Picked up upgrade {Upgrade.Name}");

    //         Destroy(gameObject);
    //     }
    // }
}
