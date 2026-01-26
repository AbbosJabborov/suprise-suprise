using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Spawn Target")]
        [SerializeField] private Transform spawnCenter; // Usually the caravan
        [SerializeField] private float spawnRadius = 15f; // Distance from center to spawn
        [SerializeField] private float minSpawnDistance = 10f; // Don't spawn too close
    
        [Header("Enemy Prefabs")]
        [SerializeField] private GameObject[] enemyPrefabs;
        [SerializeField] private float[] enemySpawnWeights; // Probability weights for each enemy type
    
        [Header("Wave Settings")]
        [SerializeField] private int currentWave = 1;
        [SerializeField] private int maxWaves = 5;
        [SerializeField] private float timeBetweenWaves = 10f;
        [SerializeField] private float waveStartDelay = 3f;
    
        [Header("Spawn Settings")]
        [SerializeField] private int baseEnemiesPerWave = 10;
        [SerializeField] private float enemiesPerWaveMultiplier = 1.5f; // Increases each wave
        [SerializeField] private float spawnInterval = 1f; // Time between individual spawns
        [SerializeField] private int maxEnemiesAlive = 50; // Performance limit
    
        [Header("Difficulty Scaling")]
        [SerializeField] private float healthScaling = 1.2f; // Health multiplier per wave
        [SerializeField] private float speedScaling = 1.1f; // Speed multiplier per wave
        [SerializeField] private float damageScaling = 1.15f; // Damage multiplier per wave
    
        [Header("Object Pooling")]
        [SerializeField] private bool usePooling = true;
        [SerializeField] private int poolSizePerType = 20;
    
        [Header("UI/Debug")]
        [SerializeField] private bool showDebugInfo = true;
    
        // Private variables
        private List<GameObject> activeEnemies = new List<GameObject>();
        private Dictionary<GameObject, Queue<GameObject>> enemyPools;
        private bool isSpawning;
        private int enemiesSpawnedThisWave;
        private int enemiesToSpawnThisWave;
        private WaveState currentWaveState = WaveState.Waiting;

        public enum WaveState
        {
            Waiting,
            Spawning,
            Active,
            Completed
        }
    
        void Start()
        {
            // Auto-find caravan if not set
            if (spawnCenter == null)
            {
                GameObject caravan = GameObject.FindGameObjectWithTag("Caravan");
                if (caravan != null)
                    spawnCenter = caravan.transform;
            }
        
            // Initialize object pools
            if (usePooling)
            {
                InitializePools();
            }
        
            // Start first wave
            StartCoroutine(WaveController());
        }
    
        void Update()
        {
            // Clean up destroyed enemies from active list
            activeEnemies.RemoveAll(enemy => enemy == null);
        
            // Check if wave is complete
            if (currentWaveState == WaveState.Active && activeEnemies.Count == 0 && !isSpawning)
            {
                currentWaveState = WaveState.Completed;
            }
        }
    
        IEnumerator WaveController()
        {
            while (currentWave <= maxWaves)
            {
                // Wait before starting wave
                currentWaveState = WaveState.Waiting;
                yield return new WaitForSeconds(waveStartDelay);
            
                // Start wave
                Debug.Log($"Starting Wave {currentWave}");
                currentWaveState = WaveState.Spawning;
                yield return StartCoroutine(SpawnWave());
            
                // Wave is active (enemies spawned, waiting for clear)
                currentWaveState = WaveState.Active;
            
                // Wait for all enemies to be defeated
                yield return new WaitUntil(() => currentWaveState == WaveState.Completed);
            
                Debug.Log($"Wave {currentWave} Complete!");
            
                // Increment wave
                currentWave++;
            
                // Wait between waves
                if (currentWave <= maxWaves)
                {
                    yield return new WaitForSeconds(timeBetweenWaves);
                }
            }
        
            Debug.Log("All waves completed!");
            OnAllWavesComplete();
        }
    
        IEnumerator SpawnWave()
        {
            isSpawning = true;
        
            // Calculate enemies for this wave
            enemiesToSpawnThisWave = Mathf.RoundToInt(baseEnemiesPerWave * Mathf.Pow(enemiesPerWaveMultiplier, currentWave - 1));
            enemiesSpawnedThisWave = 0;
        
            // Spawn enemies over time
            while (enemiesSpawnedThisWave < enemiesToSpawnThisWave)
            {
                // Check if at max capacity
                if (activeEnemies.Count >= maxEnemiesAlive)
                {
                    yield return new WaitForSeconds(0.5f);
                    continue;
                }
            
                SpawnRandomEnemy();
                enemiesSpawnedThisWave++;
            
                yield return new WaitForSeconds(spawnInterval);
            }
        
            isSpawning = false;
        }
    
        void SpawnRandomEnemy()
        {
            if (enemyPrefabs == null || enemyPrefabs.Length == 0)
            {
                Debug.LogWarning("No enemy prefabs assigned!");
                return;
            }
        
            // Select random enemy type based on weights
            GameObject enemyPrefab = SelectWeightedRandomEnemy();
        
            // Get spawn position
            Vector2 spawnPosition = GetRandomSpawnPosition();
        
            // Spawn or get from pool
            GameObject enemy;
            if (usePooling)
            {
                enemy = GetPooledEnemy(enemyPrefab);
                enemy.transform.position = spawnPosition;
                enemy.SetActive(true);
            }
            else
            {
                enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            }
        
            // Configure enemy
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                // Scale stats based on wave
                int scaledHealth = Mathf.RoundToInt(enemyScript.GetMaxHealth() * Mathf.Pow(healthScaling, currentWave - 1));
                float scaledSpeed = enemyScript.GetComponent<Enemy>().GetComponent<Rigidbody2D>().linearVelocity.magnitude * Mathf.Pow(speedScaling, currentWave - 1);
                // You'll need to expose a GetDamage() method in Enemy script for this
                // int scaledDamage = Mathf.RoundToInt(originalDamage * Mathf.Pow(damageScaling, currentWave - 1));
            
                // enemyScript.SetStats(scaledHealth, scaledSpeed, scaledDamage);
                enemyScript.SetTarget(spawnCenter);
            }
        
            // Add to active enemies
            activeEnemies.Add(enemy);
        }
    
        GameObject SelectWeightedRandomEnemy()
        {
            // If no weights set, use equal probability
            if (enemySpawnWeights == null || enemySpawnWeights.Length != enemyPrefabs.Length)
            {
                return enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            }
        
            // Calculate total weight
            float totalWeight = 0f;
            foreach (float weight in enemySpawnWeights)
            {
                totalWeight += weight;
            }
        
            // Random value
            float randomValue = Random.Range(0f, totalWeight);
        
            // Select based on weight
            float cumulativeWeight = 0f;
            for (int i = 0; i < enemyPrefabs.Length; i++)
            {
                cumulativeWeight += enemySpawnWeights[i];
                if (randomValue <= cumulativeWeight)
                {
                    return enemyPrefabs[i];
                }
            }
        
            return enemyPrefabs[^1];
        }
    
        Vector2 GetRandomSpawnPosition()
        {
            Vector2 centerPos = spawnCenter.position;
        
            // Random angle
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        
            // Random distance between min and max
            float distance = Random.Range(minSpawnDistance, spawnRadius);
        
            // Calculate position
            Vector2 spawnPos = centerPos + new Vector2(
                Mathf.Cos(angle) * distance,
                Mathf.Sin(angle) * distance
            );
        
            return spawnPos;
        }
    
        void InitializePools()
        {
            enemyPools = new Dictionary<GameObject, Queue<GameObject>>();
        
            foreach (GameObject prefab in enemyPrefabs)
            {
                Queue<GameObject> pool = new Queue<GameObject>();
            
                for (int i = 0; i < poolSizePerType; i++)
                {
                    GameObject obj = Instantiate(prefab, transform, true);
                    obj.SetActive(false);
                    pool.Enqueue(obj);
                }
            
                enemyPools[prefab] = pool;
            }
        }
    
        GameObject GetPooledEnemy(GameObject prefab)
        {
            if (!enemyPools.ContainsKey(prefab))
            {
                Debug.LogWarning($"No pool for prefab {prefab.name}");
                return Instantiate(prefab);
            }
        
            Queue<GameObject> pool = enemyPools[prefab];
        
            // Find inactive object
            foreach (GameObject obj in pool)
            {
                if (!obj.activeInHierarchy)
                {
                    return obj;
                }
            }
        
            // If all active, create new one
            GameObject newObj = Instantiate(prefab, transform, true);
            pool.Enqueue(newObj);
            return newObj;
        }
    
        void OnAllWavesComplete()
        {
            Debug.Log("Victory! All waves defeated!");
        }
    
        // Public methods for external control
        public void StartNextWave()
        {
            if (currentWaveState == WaveState.Completed || currentWaveState == WaveState.Waiting)
            {
                currentWave++;
                StartCoroutine(SpawnWave());
            }
        }
    
        public void StopSpawning()
        {
            StopAllCoroutines();
            isSpawning = false;
        }
    
        public int GetCurrentWave() => currentWave;
        public int GetActiveEnemyCount() => activeEnemies.Count;
        public int GetEnemiesRemainingThisWave() => enemiesToSpawnThisWave - enemiesSpawnedThisWave;
        public WaveState GetWaveState() => currentWaveState;
    
        void OnDrawGizmosSelected()
        {
            if (spawnCenter == null)
                return;
        
            // Draw spawn radius
            Gizmos.color = Color.red;
            DrawCircle(spawnCenter.position, spawnRadius, 32);
        
            // Draw min spawn distance
            Gizmos.color = Color.yellow;
            DrawCircle(spawnCenter.position, minSpawnDistance, 32);
        }
    
        void DrawCircle(Vector3 center, float radius, int segments)
        {
            float angleStep = 360f / segments;
            Vector3 prevPoint = center + new Vector3(radius, 0, 0);
        
            for (int i = 1; i <= segments; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector3 newPoint = center + new Vector3(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius,
                    0
                );
                Gizmos.DrawLine(prevPoint, newPoint);
                prevPoint = newPoint;
            }
        }
    }
}