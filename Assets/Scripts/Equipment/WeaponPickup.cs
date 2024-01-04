using Sirenix.OdinInspector;
using UnityEngine;

public class WeaponPickup : MonoBehaviour, IInteractable
{
    public Gun Equipment;

    public InteractionData GetData()
    {
        return new InteractionData() { };
    }

    public void Interact(InteractionPayload payload)
    {
        if (payload.Owner.TryGetComponent(out PlayerEquipmentController plr))
        {
            plr.PickupWeapon(Equipment);
            Destroy(gameObject);
        }
    }

    [Button]
    void GenerateCollider()
    {
        if (gameObject.GetComponent<Collider>() == null)
        {
            var collider = gameObject.AddComponent<SphereCollider>();
            collider.radius = 2.0f;
        }

        GetComponent<Collider>().isTrigger = true;
    }

    void Awake()
    {
        Equipment = GetComponentInChildren<Gun>();
        gameObject.layer = LayerMask.NameToLayer("Interactable");
        GenerateCollider();
    }

    void Start()
    {
        if (
            Physics.Raycast(
                transform.position,
                Vector3.down,
                out var hit,
                1.0f,
                LayerMask.GetMask("Default")
            )
        )
        {
            transform.position = hit.point + Vector3.up * 2.0f;
        }

        if (Equipment == null)
        {
            Equipment = GetComponentInChildren<Gun>();
        }
    }
}
