using UnityEngine;

namespace Gameplay.Weapon
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        private WeaponData weaponData;
        private int damage;
        private float lifetime;
        private float lifetimeTimer;
        private bool isPlayerBullet;
        private int piercesRemaining;
        
        void Awake()
        {
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        public void Initialize(Vector2 direction, WeaponData weapon, bool isPlayerBullet)
        {
            this.weaponData = weapon;
            this.isPlayerBullet = isPlayerBullet;
            this.damage = weapon.Damage;
            this.lifetime = weapon.BulletLifetime;
            this.lifetimeTimer = 0;
            this.piercesRemaining = weapon.PiercingShot ? weapon.PiercingCount : 0;
            
            // Set velocity
            rb.linearVelocity = direction.normalized * weapon.BulletSpeed;
            
            // Set visual
            if (spriteRenderer != null)
            {
                spriteRenderer.color = weapon.BulletColor;
            }
        }
        
        void Update()
        {
            lifetimeTimer += Time.deltaTime;
            
            if (lifetimeTimer >= lifetime)
            {
                Deactivate();
            }
        }
        
        void OnTriggerEnter2D(Collider2D other)
        {
            // Hit enemy
            if (isPlayerBullet && other.CompareTag("Enemy"))
            {
                var enemy = other.GetComponent<Enemy.Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                    
                    // Apply knockback
                    Rigidbody2D enemyRb = other.GetComponent<Rigidbody2D>();
                    if (enemyRb != null && weaponData != null)
                    {
                        Vector2 knockbackDir = rb.linearVelocity.normalized;
                        enemyRb.AddForce(knockbackDir * weaponData.KnockbackForce, ForceMode2D.Impulse);
                    }
                }
                
                // Handle piercing
                if (piercesRemaining > 0)
                {
                    piercesRemaining--;
                }
                else
                {
                    Deactivate();
                }
            }
            // Hit player
            else if (!isPlayerBullet && other.CompareTag("Player"))
            {
                var player = other.GetComponent<Gameplay.Player.PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(damage);
                }
                Deactivate();
            }
            // Hit wall/obstacle
            else if (other.CompareTag("Obstacle") || other.CompareTag("Wall"))
            {
                Deactivate();
            }
        }
        
        void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}