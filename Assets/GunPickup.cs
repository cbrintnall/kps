using UnityEngine;

public class GunPickup : MonoBehaviour
{
    public string GunId;

    private GunManager gunManager;

    void Awake()
    {
        gunManager = SingletonLoader.Get<GunManager>();
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out PlayerEquipmentController controller))
        {
            if (gunManager.TryGetGun(GunId, out GunDefinition gunDef))
            {
                controller.PickupWeapon(gunDef.Create());
            }

            Destroy(gameObject);
        }
    }
}
