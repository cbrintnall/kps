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

public class EnemyDefinition
{
    public GameObject Prefab;
    public Type Component;
    public int Value;
    public Color PrimaryColor;
    public Color SecondaryColor;
    public Dictionary<string, object> Stats = new();
    public int StartsAt = 0;

    public string GenerateId() => Value.ToString();
}

public class EnemySequence
{
    public EnemyDefinition Enemy;
    public int Count;
    public int TotalValue => Enemy.Value * Count;
}

public class EnemyMaster : MonoBehaviour, IReloadable
{
    public static HashSet<GameObject> ActiveEnemies = new();
    public static List<Transform> RangedPoints = new();

    [ReadOnly, DebugGUIGraphAttribute(0.0f, 1.0f, 0.0f)]
    public int CurrentValue;

    [ReadOnly, DebugGUIGraphAttribute(1.0f, 0.0f, 0.0f)]
    public int TargetValue;

    [DebugGUIGraphAttribute(0.0f, 0.0f, 1.0f)]
    public float SpawnCheckSeconds = 1.0f;
    public Material BaseEnemyMaterial;
    public EnemyData Data;
    public EnemyDefinition[] Enemies => Data.Definitions;
    public int Deficit => TargetValue - CurrentValue;
    public List<EnemySequence> Sequences = new();

    // This should eventually become a curve, dictated by difficulty
    public float TargetIncreaseSeconds => Data.TargetRateIncrease;

    int lastRoundValue;
    TimeSince targetts;
    TimeSince spawnts;
    TimeSince killCounter;
    List<Spawn> spawns = new();
    int killedInLastSecond;
    public float killsPerSecond;
    int sequenceCount = 0;
    EnemySequence currentSequence;
    RoundManager roundManager;
    Vector3 targetPoint;

    [IngameDebugConsole.ConsoleMethod("spawn", "Spawns monster with ID")]
    public static void ForceSpawn(string id)
    {
        var master = FindObjectOfType<EnemyMaster>();
        var payload = master.Data.Definitions.Where(def => def.GenerateId() == id).FirstOrDefault();

        if (payload != null)
        {
            master.SpawnPayload(payload);
        }
    }

    void Awake()
    {
        roundManager = SingletonLoader.Get<RoundManager>();
        RangedPoints = GameObject
            .FindGameObjectsWithTag("ranged")
            .Select(go => go.transform)
            .ToList();
        ActiveEnemies = new();
        Reload();
        TargetValue = Data.StartValue;
    }

    void BuildSequences()
    {
        Sequences = new();
        for (
            int i = 0;
            i < UnityEngine.Random.Range(Data.MinSequencesPerRound, Data.MaxSequencesPerRound);
            i++
        )
        {
            var target = Data.Definitions
                .Where(def => def.StartsAt <= roundManager.Round)
                .Where(def => def.Value * Data.MinSequenceSize < TargetValue)
                .Random();

            var sequence = new EnemySequence
            {
                Count = UnityEngine.Random.Range(Data.MinSequenceSize, Data.MaxSequenceSize),
                Enemy = target
            };

            Sequences.Add(sequence);
        }
    }

    void Start()
    {
        targetts = 0;
        spawnts = 0;
        killCounter = 0;

        CollectSpawns();
        BuildSequences();
        Enemies.Sort((a, b) => b.Value - a.Value);
        enabled = false;
        lastRoundValue = 0;
    }

    void Update()
    {
        IncrTarget();
        CheckSpawn();

        SpawnCheckSeconds = Mathf.Max(
            Mathf.Min(
                Mathf.InverseLerp(lastRoundValue, TargetValue, CurrentValue) * 2.0f,
                Mathf.Abs(Mathf.Log10(10 - roundManager.Round))
            ),
            Mathf.Max(Mathf.Log10(Data.MinCoefficient - roundManager.Round) * 0.5f, 0.1f)
        );

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
            enemy?.GetComponent<Health>().InstaKill();
        }

        ActiveEnemies = new();
        CurrentValue = 0;
        BuildSequences();
    }

    void CollectSpawns()
    {
        spawns.AddRange(FindObjectsOfType<Spawn>());
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
            currentSequence = Sequences.Random();
            sequenceCount = 0;
        }

        if (currentSequence.Enemy.Value <= Deficit)
        {
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

        var enemy = Instantiate(payload.Prefab, spawn.transform.position, Quaternion.identity);

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
            if (current == 0)
            {
                CurrentValue -= payload.Value;
                ActiveEnemies.Remove(enemy);
                killedInLastSecond++;
            }
        };

        ActiveEnemies.Add(enemy);

        CurrentValue += payload.Value;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        foreach (var spawn in spawns)
        {
            Gizmos.DrawSphere(spawn.transform.position, 0.5f);
        }
    }

    public void Reload()
    {
        var txt = Resources.Load<TextAsset>("Enemies/enemies");
        Data = JsonConvert.DeserializeObject<EnemyData>(txt.text);
    }
}
