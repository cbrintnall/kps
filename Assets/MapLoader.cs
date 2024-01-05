using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapLoader : MonoBehaviour
{
    public static MapLoader Instance;

    [SerializeField]
    RoomDefinition startRoom;

    [SerializeField]
    int roomCount = 4;

    [SerializeField]
    Player player;

    [SerializeField]
    RoomDefinition[] Rooms;

    Player activePlayer;

    void Awake()
    {
        Instance = this;
    }

    [Button]
    void LoadAllRoomsToBuildIndex()
    {
#if UNITY_EDITOR
        var existingScenes = EditorBuildSettings.scenes.Select(
            scene => (AssetDatabase.LoadMainAssetAtPath(scene.path) as SceneAsset).name
        );
        var existingScenePaths = EditorBuildSettings.scenes.Select(scene => scene.path);
        // Rooms.Where(room => !existingScenes.Contains(room.Scene))
#endif
    }

    public void Start()
    {
        Debug.Assert(roomCount > 1);
        GenerateMap();
    }

    [Button]
    public void GenerateMap()
    {
        StartCoroutine(LoadScenes());
    }

    IEnumerator LoadScenes()
    {
        List<AsyncOperation> operations = new();
        List<RoomDefinition> loadedRooms = new() { startRoom };
        loadedRooms.AddRange(Rooms.ToList());

        for (int i = 0; i < loadedRooms.Count; i++)
        {
            SceneManager.LoadScene(loadedRooms[i].Scene, LoadSceneMode.Additive);
        }

        yield return null;
        StartCoroutine(ConnectRooms());
    }

    IEnumerator ConnectRooms()
    {
        List<WorldContainer> containers = FindObjectsOfType<WorldContainer>()
            .OrderByDescending(world => world.gameObject.scene.name == startRoom.Scene)
            .ToList();

        Debug.Assert(containers.Count > 2);
        Debug.Assert(containers[0].IsStart);
        Debug.Assert(!containers[1].IsStart);

        var wait = 3.0f;
        for (int i = 1; i < containers.Count; i++)
        {
            yield return new WaitForSeconds(wait);
            WorldContainer first = containers[i - 1];
            WorldContainer second = containers[i];

            Debug.Log($"Link {first.gameObject.scene.name} -> {second.gameObject.scene.name}");

            transform.forward = -second.EntranceDoor.transform.forward;
            yield return new WaitForSeconds(wait);
            Vector3 translation =
                second.EntranceDoor.transform.position - first.ExitDoor.transform.position;
            transform.position += translation;
            yield return new WaitForSeconds(wait);
            // transform.RotateAround(world.EntranceDoor.transform.position, Vector3.up, 270.0f);
            Physics.SyncTransforms();

            // first.LinkToWorld(second);
        }

        Physics.SyncTransforms();
        activePlayer = Instantiate(player);
        containers[0].DoPlayerSpawn(activePlayer);
    }
}
