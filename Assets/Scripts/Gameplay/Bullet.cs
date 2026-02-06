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
    
        private Vector2 direction;
        private float speed;
        private int damage;
        private float lifetime;
        private float spawnTime;
    
        void Awake()
        {
            if (rb == null)
                rb = GetComponent<Rigidbody2D>();
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            if (trailRenderer == null)
                trailRenderer = GetComponent<TrailRenderer>();
        }
    
        void OnEnable()
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
            if (rb != null)
            {
                rb.linearVelocity = direction * speed;
            }
        }
    
        void Update()
        {
            // Deactivate after lifetime expires
            if (Time.time - spawnTime >= lifetime)
            {
                DeactivateBullet();
            }
        }
    
        void FixedUpdate()
        {
            // Keep velocity constant (in case of physics interference)
            if (rb != null)
            {
                rb.linearVelocity = direction * speed;
            }
        }
    
        void OnTriggerEnter2D(Collider2D collision)
        {
            // Check if hit an enemy
            if (((1 << collision.gameObject.layer) & enemyLayer) != 0)
            {
                // Deal damage to enemy
                var enemy = collision.GetComponent<Enemy.Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            
                // Spawn hit effect
                SpawnHitEffect(collision.transform.position);
            
                // Deactivate bullet
                DeactivateBullet();
            }
            // Check if hit wall/obstacle (optional)
            else if (collision.CompareTag("Wall") || collision.CompareTag("Obstacle"))
            {
                SpawnHitEffect(collision.transform.position);
                DeactivateBullet();
            }
        }
    
        void SpawnHitEffect(Vector3 position)
        {
            if (hitEffectPrefab != null)
            {
                GameObject effect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
                Destroy(effect, 1f); // Auto-destroy effect after 1 second
            }
        }
    
        void DeactivateBullet()
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