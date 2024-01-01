using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class RoomMaster : MonoBehaviour
{
    public bool RoomFinished => spawners.All(spawner => spawner.Finished);
    public bool WaveIsFinished => spawners.All(spawner => spawner.ActiveWaveFinished);

    [SerializeField]
    [RequiredListLength(MinLength = 1)]
    AreaSpawner[] spawners;

    [SerializeField]
    [Required]
    PlayerTrigger startTrigger;

    int wave = -1;

    void Awake()
    {
        spawners = GetComponentsInChildren<AreaSpawner>();
        startTrigger.PlayerEntered += (_) => StartRoom();

        foreach (var spawner in spawners)
        {
            spawner.Cleared += OnClearSpawner;
        }
    }

    public void StartRoom()
    {
        Destroy(startTrigger.gameObject);
        wave++;
        DoWave();
    }

    void OnClearSpawner()
    {
        if (WaveIsFinished)
        {
            if (RoomFinished)
            {
                Debug.Log("Room done");
            }
            else
            {
                DoWave();
            }
        }
    }

    void DoWave()
    {
        foreach (var spawner in spawners)
        {
            spawner.DoWaveSpawn(wave);
        }

        wave++;
    }
}
