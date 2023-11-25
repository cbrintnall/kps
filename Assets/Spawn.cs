using UnityEngine;

public class Spawn : MonoBehaviour, ISpawn
{
    bool ISpawn.CanSpawn => canSpawn;
    private bool canSpawn = false;

    public void DoSpawn(Transform target)
    {
        target.position = transform.position;
    }

    void FixedUpdate()
    {
        var dir = (
            PlayerEquipmentController.Instance.transform.position - transform.position
        ).normalized;

        canSpawn = false;

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
            canSpawn = hit.collider.tag != "player";
        }
    }
}
