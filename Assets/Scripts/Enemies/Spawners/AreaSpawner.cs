using System;
using System.Collections;
using UnityEngine;

public class AreaSpawner : MonoBehaviour
{
    public event Action Cleared;

    public bool Finished => lastWave >= Groups.Length - 1;
    public bool ActiveWaveFinished => activeEnemyCount <= 0;

    [SerializeField]
    Area Area;

    [SerializeField]
    EnemyGroup[] Groups;

    [SerializeField]
    AudioClip SpawnSound;

    int lastWave = -1;
    int activeEnemyCount = 0;
    AudioManager audioManager;

    void Awake()
    {
        audioManager = SingletonLoader.Get<AudioManager>();
    }

    public void DoWaveSpawn(int wave)
    {
        lastWave = wave;

        if (wave >= Groups.Length)
            return;

        if (Groups[wave] == null)
            return;

        StartCoroutine(SpawnEnemiesStaggered(Groups[wave]));
    }

    IEnumerator SpawnEnemiesStaggered(EnemyGroup group)
    {
        activeEnemyCount = group.Count;
        for (int i = 0; i < group.Count; i++)
        {
            var enemy = group.Enemy.Create();
            enemy.transform.position = Area.GetRandomLocation();
            enemy.Health.OnDamaged += (payload) =>
            {
                if (enemy.Health.Dead)
                {
                    activeEnemyCount--;

                    if (ActiveWaveFinished)
                        Cleared?.Invoke();
                }
            };

            audioManager.Play(
                new AudioPayload()
                {
                    Location = enemy.transform.position,
                    PitchWobble = 0.1f,
                    Clip = SpawnSound
                }
            );
            yield return new WaitForSeconds(Utilities.Randf() * 0.5f);
        }
    }
}
