using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerTrigger : MonoBehaviour
{
    public event Action<PlayerEquipmentController> PlayerEntered;

    void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("TargetPlayer");
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out PlayerEquipmentController controller))
        {
            PlayerEntered?.Invoke(controller);
        }
    }
}
