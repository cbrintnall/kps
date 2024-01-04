using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class RoomMaster : MonoBehaviour
{
    public bool RoomFinished => spawners.All(spawner => spawner.Finished);
    public bool WaveIsFinished => spawners.All(spawner => spawner.ActiveWaveFinished);
    public event Action FinishedRoom;

    [HorizontalGroup("Debug", Title = "Debug")]
    [SerializeField]
    [Tooltip(
        "Shouldn't be used in live - useful for testing constant combat, causes the spawners to never stop and loops around."
    )]
    bool loops;

    [HorizontalGroup("Debug")]
    [SerializeField]
    [Tooltip("Causes the spawners to never trigger, the opposite of 'loops'.")]
    bool dontSpawn;

    [VerticalGroup("Dependencies")]
    [SerializeField]
    [RequiredListLength(MinLength = 1)]
    AreaSpawner[] spawners;

    [VerticalGroup("Dependencies")]
    [SerializeField]
    [Required]
    PlayerTrigger startTrigger;

    int wave = -1;

    void Awake()
    {
        spawners = GetComponentsInChildren<AreaSpawner>();

        if (!dontSpawn)
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
                if (loops)
                {
                    wave = -1;
                    DoWave();
                }
                else
                {
                    FinishedRoom?.Invoke();
                }
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
