using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapLoader : MonoBehaviour
{
    public static MapLoader Instance;

    [SerializeField]
    int roomCount = 4;

    [SerializeField]
    GameObject player;

    [SerializeField]
    RoomDefinition[] Rooms;

    GameObject activePlayer;

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
        for (int i = 0; i < Rooms.Length; i++)
        {
            operations.Add(SceneManager.LoadSceneAsync(Rooms[i].Scene, LoadSceneMode.Additive));
        }

        yield return new WaitUntil(() => operations.All(op => op.isDone));

        this.Defer(ConnectRooms);
    }

    void ConnectRooms()
    {
        WorldContainer[] containers = FindObjectsOfType<WorldContainer>();
        for (int i = 1; i < containers.Length; i++)
        {
            containers[i - 1].LinkToWorld(containers[i]);
        }
    }

    void LoadCurrentRoom()
    {
        // string scene = Rooms[room].Scene;
        // AsyncOperation op = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        // Debug.Log($"Loading next room {scene}");
        // op.completed += (opData) => SetupNewRoom(scene);
    }

    void SetupNewRoom(string sceneName)
    {
        // var world = FindObjectsOfType<WorldContainer>()
        //     .Where(container => container.Id == "")
        //     .FirstOrDefault();

        // if (activePlayer == null)
        // {
        //     activePlayer = Instantiate(player);
        // }

        // Debug.Assert(world != null);

        // world.Id = new Guid().ToString();
        // world.RoomMaster.FinishedRoom += OnFinishedRoom;

        // if (activeRoom != null)
        // {
        //     // activeRoom.Item2.ExitPortal.SetTargetPoint(world.StartPortal.transform);
        //     // activeRoom.Item2.ExitPortal.PlayerEntered += () =>
        //     // {
        //     //     SceneManager.UnloadSceneAsync(lastRoom.Item1);
        //     //     Debug.Log($"Unloaded previous room {lastRoom.Item1}");
        //     // };
        //     world.transform.position +=
        //         Vector3.up
        //         * (activeRoom.Item2.Bounds.extents.y * 2.5f + world.Bounds.extents.y * 2.5f);
        // }

        // lastRoom = activeRoom;
        // activeRoom = Tuple.Create(sceneName, world);
    }

    void OnFinishedRoom()
    {
        // LoadNextMap();
    }
}
