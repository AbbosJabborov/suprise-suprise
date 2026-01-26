using Gameplay.Managers;
using UnityEngine;

namespace Gameplay
{
    public class Bullet : MonoBehaviour
    {
        [Header("Bullet Properties")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private TrailRenderer trailRenderer;
    
        [Header("Hit Effects")]
        [SerializeField] private GameObject hitEffectPrefab;
        [SerializeField] private LayerMask enemyLayer; 
        private float speed;
    
        private Vector2 direction;
        private int damage;
        private float lifetime;
        private float spawnTime;

        private void Awake()
        {
            if (rb == null)
                rb = GetComponent<Rigidbody2D>();
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            if (trailRenderer == null)
                trailRenderer = GetComponent<TrailRenderer>();
        }

        private void OnEnable()
        {
            spawnTime = Time.time;
        
            // Clear trail when bullet is reused from pool
            if (trailRenderer != null)
            {
                trailRenderer.Clear();
            }
        }
    
        public void Initialize(Vector2 dir, float spd, int dmg, float life)
        {
            direction = dir.normalized;
            speed = spd;
            damage = dmg;
            lifetime = life;
            spawnTime = Time.time;
        
            // Set velocity
            if (rb)
            {
                rb.linearVelocity = direction * speed;
            }
        }

        private void Update()
        {
            // Deactivate after lifetime expires
            if (Time.time - spawnTime >= lifetime)
            {
                DeactivateBullet();
            }
        }

        private void FixedUpdate()
        {
            // Keep velocity constant (in case of physics interference)
            if (rb)
            {
                rb.linearVelocity = direction * speed;
            }
        }
    
        void OnTriggerEnter2D(Collider2D collision)
        {
            // Check if hit an enemy
            if (collision.CompareTag("Enemy"))
            {
                // Try built-in Enemy health first
                var enemy = collision.GetComponent<Enemy.Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
        
                // Or use Health component
                Health health = collision.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                }
        
                // Register kill if dead (add to Enemy.cs Die() method)
                if (GameManager.Instance != null && enemy != null && enemy.IsDead())
                {
                    GameManager.Instance.RegisterKill();
                }
        
                SpawnHitEffect(collision.transform.position);
                DeactivateBullet();
            }
        }
        private void SpawnHitEffect(Vector3 position)
        {
            if (hitEffectPrefab != null)
            {
                GameObject effect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
                Destroy(effect, 1f); // Auto-destroy effect after 1 second
            }
        }

        private void DeactivateBullet()
        {
            // Reset velocity
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        
            // Return to pool
            gameObject.SetActive(false);
        }
    }
}