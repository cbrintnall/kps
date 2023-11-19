using UnityEngine;

public class Spawn : MonoBehaviour
{
    public bool CanSpawn = true;

    void FixedUpdate()
    {
        var dir = (
            PlayerEquipmentController.Instance.transform.position - transform.position
        ).normalized;

        CanSpawn = false;

        if (
            Physics.Raycast(
                transform.position,
                dir,
                out var hit,
                float.PositiveInfinity,
                LayerMask.GetMask("Default", "Player")
            )
        )
        {
            CanSpawn = hit.collider.tag != "player";
        }
    }
}
