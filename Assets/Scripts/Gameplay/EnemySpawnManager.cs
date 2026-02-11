using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VContainer;

namespace Gameplay
{
    public class EnemySpawnManager : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private float spawnRadius = 15f;
        [SerializeField] private float waveInterval = 30f;
        
        [Header("Wave Configuration")]
        [SerializeField] private int baseEnemiesPerWave = 10;
        [SerializeField] private float difficultyScaling = 1.2f;
        
        [Header("References")]
        [SerializeField] private Transform caravanTransform;
        
        private IEnemyFactory enemyFactory;
        private MailboxManager mailboxManager;
        
        private int currentWave = 0;
        private bool isSpawningEnabled = true;
        private List<GameObject> activeEnemies = new List<GameObject>();
        
        [Inject]
        public void Construct(IEnemyFactory factory, MailboxManager manager)
        {
            enemyFactory = factory;
            mailboxManager = manager;
            
            if (mailboxManager != null)
            {
                mailboxManager.OnMailboxReached += OnMailboxReached;
                mailboxManager.OnResumeJourney += OnJourneyResumed;
            }
        }
        
        void Start()
        {
            if (caravanTransform == null)
            {
                var caravan = FindFirstObjectByType<Caravan.CaravanController>();
                if (caravan != null)
                    caravanTransform = caravan.transform;
            }
            
            StartCoroutine(SpawnWaves());
        }
        
        IEnumerator SpawnWaves()
        {
            while (true)
            {
                if (isSpawningEnabled)
                {
                    SpawnWave();
                    currentWave++;
                }
                
                yield return new WaitForSeconds(waveInterval);
            }
        }
        
        void SpawnWave()
        {
            int enemyCount = Mathf.RoundToInt(baseEnemiesPerWave * Mathf.Pow(difficultyScaling, currentWave));
            
            Debug.Log($"[EnemySpawnManager] Spawning wave {currentWave + 1} with {enemyCount} enemies");
            
            for (int i = 0; i < enemyCount; i++)
            {
                SpawnEnemy();
            }
        }
        
        void SpawnEnemy()
        {
            Vector3 spawnPos = GetRandomSpawnPosition();
            EnemyType type = GetRandomEnemyType();
            
            GameObject enemy = enemyFactory.CreateEnemy(type, spawnPos);
            
            if (enemy != null)
            {
                activeEnemies.Add(enemy);
                
                // Subscribe to death event to remove from list
                var health = enemy.GetComponent<Health>();
                if (health != null)
                {
                    health.OnDeath.AddListener(() => OnEnemyDied(enemy));
                }
            }
        }
        
        void OnEnemyDied(GameObject enemy)
        {
            activeEnemies.Remove(enemy);
        }
        
        Vector3 GetRandomSpawnPosition()
        {
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                return spawnPoint.position + (Vector3)Random.insideUnitCircle * 2f;
            }
            else if (caravanTransform != null)
            {
                Vector2 randomDir = Random.insideUnitCircle.normalized;
                Vector3 offset = new Vector3(randomDir.x, randomDir.y, 0) * spawnRadius;
                return caravanTransform.position + offset;
            }
            
            return Vector3.zero;
        }
        
        EnemyType GetRandomEnemyType()
        {
            float roll = Random.value;
            
            // 60% Melee, 25% Shooter, 10% Tank, 5% Swarmer
            if (roll < 0.6f) return EnemyType.Melee;
            else if (roll < 0.85f) return EnemyType.Shooter;
            else if (roll < 0.95f) return EnemyType.Tank;
            else return EnemyType.Swarmer;
        }
        
        void OnMailboxReached(int index)
        {
            isSpawningEnabled = false;
            Debug.Log("[EnemySpawnManager] Spawning stopped - mailbox reached");
        }
        
        void OnJourneyResumed()
        {
            isSpawningEnabled = true;
            Debug.Log("[EnemySpawnManager] Spawning resumed");
        }
        
        public void ClearAllEnemies()
        {
            foreach (var enemy in activeEnemies)
            {
                if (enemy != null)
                    Destroy(enemy);
            }
            activeEnemies.Clear();
        }
        
        void OnDestroy()
        {
            if (mailboxManager != null)
            {
                mailboxManager.OnMailboxReached -= OnMailboxReached;
                mailboxManager.OnResumeJourney -= OnJourneyResumed;
            }
        }
    }
 
    public enum EnemyType
    {
        Melee,
        Shooter,
        Tank,
        Swarmer,
        Elite
    }
    
}