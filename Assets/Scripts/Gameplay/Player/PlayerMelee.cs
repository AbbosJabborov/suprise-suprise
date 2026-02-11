using UnityEngine;
using System;
using Core.Services;

namespace Gameplay.Player
{
    /// <summary>
    /// Handles melee slash and bullet deflection
    /// Pure C# class, no MonoBehaviour
    /// </summary>
    public class PlayerMelee
    {
        private readonly PlayerData playerData;
        private readonly Transform transform;
        private readonly IAudioService audioService;
        
        // State
        private bool isSlashing;
        private float slashTimer;
        private float cooldownTimer;
        private Vector2 slashDirection;
        
        // Events
        public event Action<Vector2> OnSlashPerformed;
        
        public PlayerMelee(PlayerData playerData, Transform transform, IAudioService audioService)
        {
            this.playerData = playerData;
            this.transform = transform;
            this.audioService = audioService;
        }
        
        public void Update(float deltaTime)
        {
            // Update cooldown
            if (cooldownTimer > 0)
            {
                cooldownTimer -= deltaTime;
            }
            
            // Update slash
            if (isSlashing)
            {
                slashTimer -= deltaTime;
                
                if (slashTimer <= 0)
                {
                    EndSlash();
                }
            }
        }
        
        public bool CanSlash()
        {
            return !isSlashing && cooldownTimer <= 0;
        }
        
        public void PerformSlash(Vector2 direction)
        {
            if (!CanSlash())
                return;
            
            slashDirection = direction.normalized;
            isSlashing = true;
            slashTimer = playerData.SlashDuration;
            cooldownTimer = playerData.SlashCooldown;
            
            // Detect and deflect bullets
            DeflectBullets();
            
            // Detect and damage enemies
            DamageEnemies();
            
            // Play slash sound
            // audioService.PlaySFX(slashSound);
            
            OnSlashPerformed?.Invoke(slashDirection);
        }
        
        void DeflectBullets()
        {
            // Find all bullets in deflection radius
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                transform.position,
                playerData.DeflectionRadius,
                LayerMask.GetMask("EnemyBullet")
            );
            
            foreach (var hit in hits)
            {
                // Check if bullet is in front of player (within slash arc)
                Vector2 toBullet = (hit.transform.position - transform.position).normalized;
                float angle = Vector2.Angle(slashDirection, toBullet);
                
                if (angle < 60f) // 120-degree arc total
                {
                    DeflectBullet(hit.gameObject);
                }
            }
        }
        
        void DeflectBullet(GameObject bullet)
        {
            // Reverse bullet direction
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                bulletRb.linearVelocity = -bulletRb.linearVelocity;
            }
            
            // Change bullet layer to player bullet (so it damages enemies)
            bullet.layer = LayerMask.NameToLayer("PlayerBullet");
            
            // Visual effect (could add particle effect here)
            Debug.Log("[PlayerMelee] Deflected bullet!");
        }
        
        void DamageEnemies()
        {
            // Find all enemies in slash range
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                transform.position,
                playerData.SlashRange,
                LayerMask.GetMask("Enemy")
            );
            
            foreach (var hit in hits)
            {
                // Check if enemy is in front of player
                Vector2 toEnemy = (hit.transform.position - transform.position).normalized;
                float angle = Vector2.Angle(slashDirection, toEnemy);
                
                if (angle < 60f)
                {
                    DamageEnemy(hit.gameObject);
                }
            }
        }
        
        void DamageEnemy(GameObject enemy)
        {
            var enemyHealth = enemy.GetComponent<Enemy.Enemy>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(playerData.SlashDamage);
                Debug.Log($"[PlayerMelee] Slashed {enemy.name} for {playerData.SlashDamage} damage");
            }
        }
        
        void EndSlash()
        {
            isSlashing = false;
        }
        
        public bool IsSlashing() => isSlashing;
        public float GetCooldownPercent() => Mathf.Clamp01(1f - (cooldownTimer / playerData.SlashCooldown));
        
        // Draw gizmos for debugging
        public void DrawGizmos()
        {
            if (!isSlashing)
                return;
            
            // Draw slash arc
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, playerData.SlashRange);
            
            // Draw deflection radius
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, playerData.DeflectionRadius);
        }
    }
}