using UnityEngine;

namespace Gameplay.Building
{
    public abstract class Building : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected BuildingData buildingData;
        [SerializeField] protected BuildingSlot parentSlot;
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected Transform firePoint; // Where projectiles spawn
    
        [Header("Current Stats")]
        [SerializeField] protected int currentTier = 1;
        protected float currentFireRate;
        protected int currentDamage;
        protected float currentRange;
    
        [Header("Targeting")]
        [SerializeField] protected LayerMask enemyLayer;
        [SerializeField] protected Transform currentTarget;
    
        [Header("Visual")]
        [SerializeField] protected ParticleSystem activeEffect;
        [SerializeField] protected LineRenderer rangeIndicator;
    
        protected float lastFireTime;
        protected bool isActive = true;
    
        public virtual void Initialize(BuildingData data, int tier, BuildingSlot slot)
        {
            buildingData = data;
            currentTier = tier;
            parentSlot = slot;
        
            UpdateStats();
        
            if (spriteRenderer != null && data.IconSprite != null)
            {
                spriteRenderer.sprite = data.IconSprite;
                spriteRenderer.color = data.BuildingColor;
            }
        
            OnInitialized();
        }
    
        protected virtual void OnInitialized()
        {
            // Override in derived classes for custom initialization
        }
    
        public virtual void OnTierUpgraded(int newTier)
        {
            currentTier = newTier;
            UpdateStats();
            OnUpgraded();
        }
    
        protected virtual void OnUpgraded()
        {
            // Override for upgrade effects (particles, sound, etc.)
        }
    
        protected virtual void UpdateStats()
        {
            if (buildingData == null)
                return;
        
            currentFireRate = buildingData.GetFireRateForTier(currentTier);
            currentDamage = buildingData.GetDamageForTier(currentTier);
            currentRange = buildingData.GetRangeForTier(currentTier);
        }
    
        protected virtual void Update()
        {
            if (!isActive)
                return;
        
            FindTarget();
        
            if (currentTarget != null)
            {
                if (Time.time >= lastFireTime + currentFireRate)
                {
                    Attack();
                    lastFireTime = Time.time;
                }
            }
        }
    
        protected virtual void FindTarget()
        {
            // Clear target if dead or out of range
            if (currentTarget != null)
            {
                float distance = Vector2.Distance(transform.position, currentTarget.position);
                if (distance > currentRange)
                {
                    currentTarget = null;
                }
            
                // Check if enemy is still alive
                if(currentTarget)
                {
                    var enemy = currentTarget.GetComponent<Enemy.Enemy>();
                    if (enemy && enemy.IsDead())
                    {
                        currentTarget = null;
                    }
                }
            }
        
            // Find new target if needed
            if (!currentTarget)
            {
                currentTarget = FindClosestEnemy();
            }
        }
    
        protected Transform FindClosestEnemy()
        {
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, currentRange, enemyLayer);
        
            if (enemies.Length == 0)
                return null;
        
            Transform closest = null;
            float closestDistance = float.MaxValue;
        
            foreach (var enemyCollider in enemies)
            {
                // Check if alive
                Enemy.Enemy enemy = enemyCollider.GetComponent<Enemy.Enemy>();
                if (enemy && enemy.IsDead())
                    continue;
            
                float distance = Vector2.Distance(transform.position, enemyCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = enemyCollider.transform;
                }
            }
        
            return closest;
        }
    
        protected abstract void Attack();
    
        public void SetActive(bool active)
        {
            isActive = active;
        
            if (activeEffect != null)
            {
                if (active)
                    activeEffect.Play();
                else
                    activeEffect.Stop();
            }
        }
    
        // Public getters
        public BuildingData GetBuildingData() => buildingData;
        public int GetCurrentTier() => currentTier;
        public float GetCurrentRange() => currentRange;
    
        protected virtual void OnDrawGizmosSelected()
        {
            // Draw range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, currentRange);
        
            // Draw line to current target
            if (currentTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, currentTarget.position);
            }
        }
    }
}