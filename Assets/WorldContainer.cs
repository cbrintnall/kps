using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class WorldContainer : MonoBehaviour
{
    [BoxGroup("Dependencies")]
    public RoomMaster RoomMaster;

    [HideInInspector]
    public Bounds Bounds;

    [HideInInspector]
    public string Id;

    public bool IsStart;

    [SerializeField]
    GameObject geometryRoot;

    [BoxGroup("Generation")]
    [SerializeField]
    bool customDoors;

    [BoxGroup("Generation")]
    [HideIf("@!customDoors")]
    public RoomDoor ExitDoor;

    [BoxGroup("Generation")]
    [HideIf("@!customDoors")]
    public RoomDoor EntranceDoor;

    [BoxGroup("Generation")]
    [ChildGameObjectsOnly]
    [SerializeField]
    List<RoomDoor> linkableDoors;

    void Awake()
    {
        StaticBatchingUtility.Combine(geometryRoot);
    }

    public void Start()
    {
        Bounds = gameObject.CalculateBounds();

        if (customDoors)
            return;

        Debug.Assert(linkableDoors.Count >= 1);
    }

    public void DoPlayerSpawn(Player player)
    {
        if (!IsStart)
        {
            Debug.LogWarning($"Attempted to spawn player in non-start room");
            return;
        }

        Debug.Assert(EntranceDoor != null);

        player.Movement.Warp(EntranceDoor.transform.position + Vector3.up * 1.0f);
    }

    /// <summary>
    ///
    /// Links the calling objects exit to the provided objects entrance
    /// </summary>
    /// <param name="world"></param>
    public IEnumerator LinkToWorld(WorldContainer world)
    {
        transform.forward = -world.EntranceDoor.transform.forward;
        yield return new WaitForSeconds(10.0f);
        Vector3 translation = world.EntranceDoor.transform.position - ExitDoor.transform.position;
        transform.position += translation;
        yield return new WaitForSeconds(10.0f);
        // transform.RotateAround(world.EntranceDoor.transform.position, Vector3.up, 270.0f);
        Physics.SyncTransforms();
        yield return new WaitForSeconds(10.0f);
    }
}
