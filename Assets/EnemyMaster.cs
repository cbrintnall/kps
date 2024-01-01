using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine.Assertions;
using UnityEditor;
using System;
using Newtonsoft.Json;
using Sirenix.Utilities;

public class EnemyData
{
    public EnemyDefinition[] Definitions;
    public float TargetRateIncrease;
    public int StartValue;
    public int MaxSequenceSize;
    public int MinSequenceSize;
    public int MaxSequencesPerRound;
    public int MinSequencesPerRound;
    public float MinCoefficient;
}

public class EnemySequence
{
    public EnemyDefinition Enemy;
    public int Count;
    public int TotalValue => Enemy.Value * Count;

    public void Consume()
    {
        Count--;
    }

    public bool IsValid() => Count > 0;
}

public interface ISpawn
{
    bool CanSpawn { get; }

    void DoSpawn(Transform transform);
}

public class EnemyMaster : MonoBehaviour, IReloadable
{
    public static HashSet<GameObject> ActiveEnemies = new();
    public static List<Transform> RangedPoints = new();

    [ReadOnly, DebugGUIGraph(0.0f, 1.0f, 0.0f)]
    public int CurrentValue;

    [ReadOnly, DebugGUIGraph(1.0f, 0.0f, 0.0f)]
    public int TargetValue;

    [DebugGUIGraph(0.0f, 0.0f, 1.0f)]
    public float SpawnCheckSeconds = 1.0f;
    public Material BaseEnemyMaterial;
    public EnemyData Data;
    public EnemyDefinition[] Enemies => Data.Definitions;
    public int Deficit => TargetValue - CurrentValue;
    public List<EnemySequence> Sequences = new();
    public int KPS => Mathf.RoundToInt(killsPerSecond);
    public int Score => killCount;

    // This should eventually become a curve, dictated by difficulty
    public float TargetIncreaseSeconds =>
        Curves.GetTargetRateIncreaseTime(roundManager.ElapsedSeconds);
    int lastRoundValue;
    TimeSince targetts;
    TimeSince spawnts;
    TimeSince killCounter;
    List<ISpawn> spawns = new();
    int killedInLastSecond;
    public float killsPerSecond;
    int sequenceCount = 0;
    EnemySequence currentSequence;
    RoundManager roundManager;
    int killCount;

    [IngameDebugConsole.ConsoleMethod("spawn", "Spawns monster with ID")]
    public static void ForceSpawn(string id)
    {
        var master = FindObjectOfType<EnemyMaster>();
        // var payload = master.Data.Definitions.Where(def => def.GenerateId() == id).FirstOrDefault();

        // if (payload != null)
        // {
        //     master.SpawnPayload(payload);
        // }
    }

    void Awake()
    {
        roundManager = FindObjectOfType<RoundManager>();
        ActiveEnemies = new();
        Reload();
        TargetValue = Data.StartValue;
    }

    EnemySequence BuildSequence()
    {
        var target = Data.Definitions
            // .Where(def => def.StartsAt <= roundManager.TimeLeft)
            .Where(def => def.Value * Data.MinSequenceSize < TargetValue)
            .Random();

        var sequence = new EnemySequence
        {
            Count = UnityEngine.Random.Range(Data.MinSequenceSize, Data.MaxSequenceSize),
            Enemy = target
        };

        return sequence;
    }

    void Start()
    {
        targetts = 0;
        spawnts = 0;
        killCounter = 0;

        CollectSpawns();
        Enemies.Sort((a, b) => b.Value - a.Value);
        enabled = false;
        lastRoundValue = 0;
    }

    void Update()
    {
        IncrTarget();
        CheckSpawn();

        SpawnCheckSeconds = Mathf.InverseLerp(0, TargetValue, CurrentValue);

        if (killCounter > 1.0f)
        {
            killsPerSecond = killedInLastSecond;
            killCounter = 0.0f;
            killedInLastSecond = 0;
        }
    }

    void OnEnable()
    {
        lastRoundValue = TargetValue;
    }

    void OnDisable()
    {
        foreach (var enemy in ActiveEnemies.ToList())
        {
            enemy?.GetComponent<Health>()?.InstaKill();
        }

        ActiveEnemies = new();
        CurrentValue = 0;
    }

    void CollectSpawns()
    {
        spawns.AddRange(
            GameObject.FindGameObjectsWithTag("spawn").Select(spawn => spawn.GetComponent<ISpawn>())
        );
    }

    void IncrTarget()
    {
        if (targetts < TargetIncreaseSeconds)
            return;

        targetts = 0;

        TargetValue++;
    }

    void CheckSpawn()
    {
        // explicitly ignore 0.0 second spawn checks, the timer is figuring its life out.
        if (spawnts < SpawnCheckSeconds || SpawnCheckSeconds == 0.0f)
            return;

        spawnts = 0;

        if (currentSequence == null)
        {
            currentSequence = BuildSequence();
            sequenceCount = 0;
        }

        if (currentSequence.Enemy.Value <= Deficit)
        {
            currentSequence.Consume();
            SpawnPayload(currentSequence.Enemy);
            sequenceCount++;
            if (sequenceCount >= currentSequence.Count)
            {
                currentSequence = null;
            }
        }
    }

    void SpawnPayload(EnemyDefinition payload)
    {
        var spawn = spawns.Where(spawn => spawn.CanSpawn)?.Random();

        if (spawn == null)
        {
            Debug.LogWarning($"Can't find a valid spawn!");
            return;
        }

        var enemy = Instantiate(payload.Prefab);

        spawn.DoSpawn(enemy.transform);
        Physics.SyncTransforms();

        var health = enemy.GetComponent<Health>();
        enemy.GetComponent<Enemy>().Definition = payload;

        foreach (var renderer in enemy.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            renderer.materials.ForEach(mat =>
            {
                renderer.material.SetColor("_Primary", payload.PrimaryColor);
                renderer.material.SetColor("_Secondary", payload.SecondaryColor);
            });
        }

        Assert.IsTrue(health != null);

        health.Data.ValueChanged += (int current, int delta) =>
        {
            if (current <= 0)
            {
                CurrentValue -= payload.Value;
                ActiveEnemies.Remove(enemy);
                killedInLastSecond++;

                if (
                    PlayerEquipmentController.Instance != null
                    && !PlayerEquipmentController.Instance.Health.Dead
                )
                {
                    killCount++;
                }
            }
        };

        ActiveEnemies.Add(enemy);

        CurrentValue += payload.Value;
    }

    public void Reload()
    {
        var txt = Resources.Load<TextAsset>("Enemies/enemies");
        Data = JsonConvert.DeserializeObject<EnemyData>(txt.text);
    }
}
