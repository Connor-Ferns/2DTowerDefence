using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [System.Serializable]
    public class Wave
    {
        public GameObject[] enemyPrefabs;
        public int[] enemyCounts;
        public float timeBetweenSpawns = 1f;
        public float timeToNextWave = 10f;
    }

    [Header("Wave Settings")]
    [SerializeField] private Wave[] waves;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform[] waypoints;
    
    [Header("Difficulty Scaling")]
    [SerializeField] private float healthIncreasePerWave = 0.1f;
    [SerializeField] private float speedIncreasePerWave = 0.05f;
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI countdownText;

    private int currentWave = 0;
    private int remainingEnemies = 0;
    private bool isSpawning = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(StartNextWave());
    }

    private IEnumerator StartNextWave()
    {
        if (currentWave > 0)
        {
            // Wait between waves
            float countdown = waves[currentWave - 1].timeToNextWave;
            while (countdown > 0)
            {
                countdown -= Time.deltaTime;
                if (countdownText != null)
                {
                    countdownText.text = $"Next Wave in: {Mathf.Ceil(countdown)}";
                }
                yield return null;
            }
        }

        if (currentWave >= waves.Length)
        {
            // Generate infinite waves with increasing difficulty
            GenerateNewWave();
        }

        if (waveText != null)
        {
            waveText.text = $"Wave {currentWave + 1}";
        }
        
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        isSpawning = true;
        Wave currentWaveData = waves[currentWave];

        for (int i = 0; i < currentWaveData.enemyPrefabs.Length; i++)
        {
            for (int j = 0; j < currentWaveData.enemyCounts[i]; j++)
            {
                SpawnEnemy(currentWaveData.enemyPrefabs[i]);
                yield return new WaitForSeconds(currentWaveData.timeBetweenSpawns);
            }
        }

        isSpawning = false;
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        
        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            // Apply difficulty scaling
            float healthMultiplier = 1f + (healthIncreasePerWave * currentWave);
            float speedMultiplier = 1f + (speedIncreasePerWave * currentWave);
            
            enemyComponent.SetWaypoints(waypoints);
            enemyComponent.ApplyWaveModifiers(healthMultiplier, speedMultiplier);
            
            enemyComponent.OnEnemyDeath += HandleEnemyDeath;
            enemyComponent.OnEnemyReachedEnd += HandleEnemyDeath;
            remainingEnemies++;
        }
    }

    private void HandleEnemyDeath(Enemy enemy)
    {
        remainingEnemies--;
        if (remainingEnemies <= 0 && !isSpawning)
        {
            // Award wave completion bonus
            GameManager.Instance.AwardWaveCompletionBonus(currentWave);
            currentWave++;
            StartCoroutine(StartNextWave());
        }
    }

    private void GenerateNewWave()
    {
        // Create a new wave with increased difficulty
        Wave newWave = new Wave();
        Wave lastWave = waves[waves.Length - 1];

        // Copy the last wave's settings with some modifications
        newWave.enemyPrefabs = lastWave.enemyPrefabs;
        newWave.enemyCounts = new int[lastWave.enemyCounts.Length];
        for (int i = 0; i < lastWave.enemyCounts.Length; i++)
        {
            // Add 1 enemy and increase by 20%
            newWave.enemyCounts[i] = Mathf.RoundToInt((lastWave.enemyCounts[i] * 1.2f) + 1);
        }
        newWave.timeBetweenSpawns = Mathf.Max(0.5f, lastWave.timeBetweenSpawns * 0.9f); // 10% faster spawning
        newWave.timeToNextWave = lastWave.timeToNextWave;

        // Add the new wave to the array
        System.Array.Resize(ref waves, waves.Length + 1);
        waves[waves.Length - 1] = newWave;
    }

    public int GetCurrentWave()
    {
        return currentWave + 1;
    }
} 