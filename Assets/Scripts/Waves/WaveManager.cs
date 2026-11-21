using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    private class Wave
    {
        [Min(0)] public int trackers = 3;
        [Min(0)] public int demolishers = 1;
    }

    [Header("Enemy prefabs")]
    [SerializeField] private GameObject trackerPrefab;
    [SerializeField] private GameObject demolisherPrefab;

    [Header("Spawn points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Waves")]
    [SerializeField] private Wave[] waves =
    {
        new Wave { trackers = 3, demolishers = 0 },
        new Wave { trackers = 4, demolishers = 1 },
        new Wave { trackers = 5, demolishers = 2 }
    };

    [Header("Timing")]
    [SerializeField, Min(0f)] private float initialPreparationSeconds = 5f;
    [SerializeField, Min(0f)] private float preparationSeconds = 10f;
    [SerializeField, Min(0f)] private float spawnIntervalSeconds = 0.75f;

    private readonly List<EnemyHealth> activeEnemies = new List<EnemyHealth>();

    public int TotalWaves => waves != null ? waves.Length : 0;
    public int CurrentWave { get; private set; }
    public int WavesRemaining => Mathf.Max(0, TotalWaves - CurrentWave);
    public bool IsComplete { get; private set; }
    public bool IsStopped { get; private set; }

    public void StopWaves()
    {
        if (IsStopped || IsComplete)
            return;

        IsStopped = true;
        StopAllCoroutines();
        Debug.Log("Waves stopped: mission failed.", this);
    }

    private IEnumerator Start()
    {
        if (spawnPoints == null || spawnPoints.Length == 0 ||
            trackerPrefab == null || demolisherPrefab == null)
        {
            Debug.LogError("WaveManager: assign both enemy prefabs and at least one spawn point.", this);
            yield break;
        }

        if (initialPreparationSeconds > 0f)
        {
            Debug.Log($"First wave in {initialPreparationSeconds} seconds.", this);
            yield return new WaitForSeconds(initialPreparationSeconds);
        }

        for (int waveIndex = 0; waveIndex < waves.Length; waveIndex++)
        {
            CurrentWave = waveIndex + 1;
            Wave wave = waves[waveIndex];
            Debug.Log($"Wave {waveIndex + 1}/{waves.Length} started!", this);

            yield return SpawnEnemies(trackerPrefab, wave.trackers);
            yield return SpawnEnemies(demolisherPrefab, wave.demolishers);

            // Unity compares destroyed objects to null, so this also works
            // when EnemyHealth destroys an enemy GameObject on death.
            while (true)
            {
                activeEnemies.RemoveAll(enemy => enemy == null);
                if (activeEnemies.Count == 0)
                    break;

                yield return null;
            }

            Debug.Log($"Wave {waveIndex + 1} cleared!", this);

            if (waveIndex < waves.Length - 1)
            {
                Debug.Log($"Next wave in {preparationSeconds} seconds.", this);
                yield return new WaitForSeconds(preparationSeconds);
            }
        }

        if (!IsStopped)
        {
            IsComplete = true;
            Debug.Log("All waves cleared!", this);
        }
    }

    private IEnumerator SpawnEnemies(GameObject prefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            if (spawnPoint == null)
            {
                Debug.LogWarning("WaveManager: a spawn point is missing.", this);
                continue;
            }

            GameObject enemy = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            EnemyHealth health = enemy.GetComponent<EnemyHealth>();

            if (health == null)
            {
                Debug.LogError($"WaveManager: {prefab.name} needs EnemyHealth on its root.", enemy);
                Destroy(enemy);
            }
            else
            {
                activeEnemies.Add(health);
            }

            if (spawnIntervalSeconds > 0f)
                yield return new WaitForSeconds(spawnIntervalSeconds);
        }
    }
}
