using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Diagnostics;

public class WorldContainer : MonoBehaviour
{
    [BoxGroup("Dependencies")]
    public RoomMaster RoomMaster;

    [HideInInspector]
    public Bounds Bounds;

    [HideInInspector]
    public string Id;

    [HideInInspector]
    public RoomDoor ExitDoor;

    [HideInInspector]
    public RoomDoor EntranceDoor;

    [BoxGroup("Generation")]
    [ChildGameObjectsOnly]
    [SerializeField]
    List<RoomDoor> linkableDoors;

    public void Start()
    {
        Bounds = gameObject.CalculateBounds();

        Debug.Assert(linkableDoors.Count >= 1);

        var doorsCopy = linkableDoors.ToList();
        if (linkableDoors.Count == 1)
        {
            EntranceDoor = linkableDoors.First();
        }
        else
        {
            ExitDoor = doorsCopy.Random();

            if (linkableDoors.Count >= 2)
            {
                doorsCopy.Remove(ExitDoor);
                EntranceDoor = doorsCopy.Random();
            }
        }
    }

    /// <summary>
    ///
    /// Links the calling objects exit to the provided objects entrance
    /// </summary>
    /// <param name="world"></param>
    public void LinkToWorld(WorldContainer world)
    {
        Vector3 translation = world.EntranceDoor.transform.position - ExitDoor.transform.position;
        transform.position += translation;
        // GameObject doorPivot = new GameObject($"DoorPivot");
        transform.RotateAround(world.EntranceDoor.transform.position, Vector3.up, 270.0f);
    }
}
