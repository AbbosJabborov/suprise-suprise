using UnityEngine;

namespace Gameplay.Building
{
    public class BallistaBuilding : Building
    {
        [Header("Ballista Settings")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private int projectilePoolSize = 20;
    
        private GameObject[] projectilePool;
        private int currentProjectileIndex;
    
        protected override void OnInitialized()
        {
            base.OnInitialized();
            InitializeProjectilePool();
        }
    
        void InitializeProjectilePool()
        {
            if (projectilePrefab == null)
            {
                Debug.LogWarning($"Ballista {gameObject.name} has no projectile prefab!");
                return;
            }
        
            projectilePool = new GameObject[projectilePoolSize];
        
            for (int i = 0; i < projectilePoolSize; i++)
            {
                projectilePool[i] = Instantiate(projectilePrefab);
                projectilePool[i].SetActive(false);
                projectilePool[i].transform.parent = transform;
            }
        }
    
        protected override void Attack()
        {
            if (currentTarget == null)
                return;
        
            Vector2 direction = ((Vector2)currentTarget.position - (Vector2)transform.position).normalized;
        
            // Spawn projectile
            GameObject projectile = GetPooledProjectile();
        
            if (projectile != null)
            {
                Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
                projectile.transform.position = spawnPos;
            
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
            
                projectile.SetActive(true);
            
                // Initialize projectile
                BuildingProjectile proj = projectile.GetComponent<BuildingProjectile>();
                if (proj != null)
                {
                    proj.Initialize(
                        direction,
                        buildingData.ProjectileSpeed,
                        currentDamage,
                        2f, // Lifetime
                        buildingData.Piercing,
                        buildingData.PiercingTargets
                    );
                }
            }
        
            // Play fire animation/sound
            OnFired();
        }
    
        protected virtual void OnFired()
        {
            // Override for effects
            if (activeEffect != null)
            {
                activeEffect.Play();
            }
        }
    
        GameObject GetPooledProjectile()
        {
            if (projectilePool == null)
                return null;
        
            for (int i = 0; i < projectilePoolSize; i++)
            {
                currentProjectileIndex = (currentProjectileIndex + 1) % projectilePoolSize;
            
                if (!projectilePool[currentProjectileIndex].activeInHierarchy)
                {
                    return projectilePool[currentProjectileIndex];
                }
            }
        
            currentProjectileIndex = (currentProjectileIndex + 1) % projectilePoolSize;
            return projectilePool[currentProjectileIndex];
        }
    }
}