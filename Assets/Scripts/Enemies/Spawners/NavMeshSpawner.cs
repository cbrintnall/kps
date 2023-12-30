using UnityEngine;
using UnityEngine.AI;

public class NavMeshSpawner : MonoBehaviour, ISpawn
{
    public bool CanSpawn => true;

    [SerializeField]
    float spawnOffset = 2.0f;

    Vector3[] spawnPoints;

    void Awake()
    {
        var triangles = NavMesh.CalculateTriangulation();
        spawnPoints = triangles.vertices;
    }

    public void DoSpawn(Transform transform)
    {
        transform.position = spawnPoints.Random() + Vector3.up * spawnOffset;
    }
}
