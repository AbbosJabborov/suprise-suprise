using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Building
{
    public class FlameBuilding : Building
    {
        [Header("Flame Settings")]
        [SerializeField] private float coneAngle = 60f; // Total cone width
        [SerializeField] private ParticleSystem flameParticles;
        [SerializeField] private float damageTickRate = 0.2f; // Damage every 0.2 seconds
    
        private float lastDamageTime;
        private List<Enemy.Enemy> enemiesInRange = new List<Enemy.Enemy>();
    
        protected override void Update()
        {
            base.Update();
        
            if (!isActive)
                return;
        
            // Apply damage over time to enemies in cone
            if (Time.time >= lastDamageTime + damageTickRate)
            {
                DamageEnemiesInCone();
                lastDamageTime = Time.time;
            }
        }
    
        protected override void Attack()
        {
            // Flamethrower is continuous, just activate particles
            if (flameParticles != null && !flameParticles.isPlaying)
            {
                flameParticles.Play();
            }
        }
    
        void DamageEnemiesInCone()
        {
            enemiesInRange.Clear();
        
            // Find all enemies in range
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, currentRange, enemyLayer);
        
            foreach (var enemyCollider in enemies)
            {
                Enemy.Enemy enemy = enemyCollider.GetComponent<Enemy.Enemy>();
                if (enemy == null || enemy.IsDead())
                    continue;
            
                // Check if in cone
                Vector2 toEnemy = (enemyCollider.transform.position - transform.position).normalized;
                Vector2 facingDirection = GetFacingDirection();
            
                float angle = Vector2.Angle(facingDirection, toEnemy);
            
                if (angle <= coneAngle / 2f)
                {
                    enemiesInRange.Add(enemy);
                }
            }
        
            // Damage all enemies in cone
            int damagePerTick = Mathf.RoundToInt(currentDamage * damageTickRate / currentFireRate);
        
            foreach (var enemy in enemiesInRange)
            {
                enemy.TakeDamage(damagePerTick);
            }
        
            // Control particles based on enemies
            if (flameParticles != null)
            {
                if (enemiesInRange.Count > 0 && !flameParticles.isPlaying)
                {
                    flameParticles.Play();
                }
                else if (enemiesInRange.Count == 0 && flameParticles.isPlaying)
                {
                    flameParticles.Stop();
                }
            }
        }
    
        Vector2 GetFacingDirection()
        {
            // Face toward caravan center or most enemies
            if (currentTarget != null)
            {
                return ((Vector2)currentTarget.position - (Vector2)transform.position).normalized;
            }
        
            // Default facing based on slot position
            if (parentSlot != null)
            {
                switch (parentSlot.GetSlotPosition())
                {
                    case BuildingSlot.BuildingSlotPosition.Top:
                        return Vector2.up;
                    case BuildingSlot.BuildingSlotPosition.Left:
                        return Vector2.left;
                    case BuildingSlot.BuildingSlotPosition.Right:
                        return Vector2.right;
                    case BuildingSlot.BuildingSlotPosition.Back:
                        return Vector2.down;
                }
            }
        
            return Vector2.right;
        }
    
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
        
            // Draw cone
            Vector2 facingDir = GetFacingDirection();
            float halfAngle = coneAngle / 2f;
        
            // Left edge of cone
            Vector2 leftEdge = Rotate(facingDir, -halfAngle) * currentRange;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + leftEdge);
        
            // Right edge of cone
            Vector2 rightEdge = Rotate(facingDir, halfAngle) * currentRange;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + rightEdge);
        
            // Arc
            int segments = 20;
            for (int i = 0; i < segments; i++)
            {
                float angle1 = -halfAngle + (coneAngle * i / segments);
                float angle2 = -halfAngle + (coneAngle * (i + 1) / segments);
            
                Vector2 point1 = (Vector2)transform.position + Rotate(facingDir, angle1) * currentRange;
                Vector2 point2 = (Vector2)transform.position + Rotate(facingDir, angle2) * currentRange;
            
                Gizmos.DrawLine(point1, point2);
            }
        }
    
        Vector2 Rotate(Vector2 vector, float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(radians);
            float sin = Mathf.Sin(radians);
        
            return new Vector2(
                vector.x * cos - vector.y * sin,
                vector.x * sin + vector.y * cos
            );
        }
    }
}