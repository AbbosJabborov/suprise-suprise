using UnityEngine;
using System.Collections.Generic;

namespace Gameplay
{
    public class EnemyFactory : IEnemyFactory
    {
        [System.Serializable]
        public class EnemyPrefabMap
        {
            public EnemyType type;
            public GameObject prefab;
        }
        
        private readonly Dictionary<EnemyType, GameObject> enemyPrefabs;
        private readonly Dictionary<EnemyType, Queue<GameObject>> enemyPools;
        private readonly Transform poolContainer;
        private readonly int poolSizePerType = 20;
        
        public EnemyFactory(EnemyPrefabMap[] prefabMappings, Transform container = null)
        {
            enemyPrefabs = new Dictionary<EnemyType, GameObject>();
            enemyPools = new Dictionary<EnemyType, Queue<GameObject>>();
            poolContainer = container;
            
            // Map prefabs
            foreach (var mapping in prefabMappings)
            {
                enemyPrefabs[mapping.type] = mapping.prefab;
            }
            
            // Initialize pools
            InitializePools();
        }
        
        void InitializePools()
        {
            foreach (var kvp in enemyPrefabs)
            {
                var pool = new Queue<GameObject>();
                
                for (int i = 0; i < poolSizePerType; i++)
                {
                    GameObject obj = Object.Instantiate(kvp.Value);
                    obj.SetActive(false);
                    
                    if (poolContainer != null)
                        obj.transform.SetParent(poolContainer);
                    
                    pool.Enqueue(obj);
                }
                
                enemyPools[kvp.Key] = pool;
            }
            
            Debug.Log($"[EnemyFactory] Initialized pools for {enemyPrefabs.Count} enemy types");
        }
        
        public GameObject CreateEnemy(EnemyType type, Vector3 position)
        {
            if (!enemyPools.ContainsKey(type))
            {
                Debug.LogError($"[EnemyFactory] No prefab for enemy type: {type}");
                return null;
            }
            
            GameObject enemy = GetPooledEnemy(type);
            
            if (enemy != null)
            {
                enemy.transform.position = position;
                enemy.SetActive(true);
                
                // Reset enemy state
                var enemyScript = enemy.GetComponent<Enemy.Enemy>();
                if (enemyScript != null)
                {
                    // Enemy will auto-acquire target on Start()
                }
                
                var health = enemy.GetComponent<Health>();
                if (health != null)
                {
                    health.FullHeal();
                }
            }
            
            return enemy;
        }
        
        GameObject GetPooledEnemy(EnemyType type)
        {
            var pool = enemyPools[type];
            
            // Find inactive enemy
            foreach (var obj in pool)
            {
                if (!obj.activeInHierarchy)
                {
                    return obj;
                }
            }
            
            // All active, create new one
            GameObject newEnemy = Object.Instantiate(enemyPrefabs[type]);
            if (poolContainer != null)
                newEnemy.transform.SetParent(poolContainer);
            
            pool.Enqueue(newEnemy);
            Debug.LogWarning($"[EnemyFactory] Pool exhausted for {type}, created new instance");
            
            return newEnemy;
        }
    }
}